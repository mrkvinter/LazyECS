using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LazyECS.Component;
using LazyECS.Models;
using LazyECS.System;
using LazyECS.Tools;

namespace LazyECS
{
    public class ECSManager
    {
        private readonly IComponentAssignCreator componentAssignCreator;
        private EntitySystemsController entitySystemsController;
        private readonly HashSet<Entity> entities;
        private uint lastId;

        public ECSManager(IComponentAssignCreator componentAssignCreator)
        {
            this.componentAssignCreator = componentAssignCreator;
            entities = new HashSet<Entity>();
        }

        public EntitySystemsController Init()
        {
            var typeSystem = typeof(ISystemProcessing);
            var implementation =
                AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .SelectMany(e => e.GetTypes())
                    .Where(e => e.GetTypeInfo().IsClass)
                    .Where(e => typeSystem.IsAssignableFrom(e))
                    .ToArray();

            var systemProcessings = new Dictionary<Type, ISystemProcessing>(implementation.Length);
            var systemInfos = new Dictionary<Type, SystemProcessingInfo>();

            foreach (var type in implementation)
            {
                var instance = Activator.CreateInstance(type) as ISystemProcessing;
                systemProcessings.Add(type, instance);

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(CheckField);
                var assignInfo = new List<ComponentInfo>();
                foreach (var fieldInfo in fields)
                {
                    var expressionAssign = componentAssignCreator.GetExpression(type, fieldInfo);
                    assignInfo.Add(
                        new ComponentInfo(fieldInfo.FieldType, expressionAssign));
                }

                systemInfos.Add(type,
                    new SystemProcessingInfo {SystemProcessing = instance, NeededComponents = assignInfo});
            }

            entitySystemsController = new EntitySystemsController(systemInfos);
            return entitySystemsController;
        }

        public void BindComponentTo(Entity entity, IComponentData componentData)
        {
            var componentType = componentData.GetType();
            var processing = entitySystemsController.SystemInfos.Values.Where(e =>
                e.NeededComponents.Any(x => x.TypeComponent == componentType));
            foreach (var processingInfo in processing)
            {
                if (processingInfo.NeededComponents.All(e => entity.GetComponent(e.TypeComponent) != null))
                    processingInfo.AttachedEntity.Add(entity);
            }
        }

        public void UnbindComponent(Entity entity, IComponentData componentData)
        {
            var componentType = componentData.GetType();
            var processing = entitySystemsController.SystemInfos.Values.Where(e =>
                e.NeededComponents.Any(x => x.TypeComponent == componentType));
            foreach (var processingInfo in processing)
            {
                processingInfo.AttachedEntity.Remove(entity);
            }
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(this, ++lastId);
            entities.Add(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            foreach (var componentData in entity.GetComponents())
            {
                entity.RemoveComponent(componentData.GetType());
            }
            entities.Remove(entity);
        }

        private bool CheckField(FieldInfo fieldInfo)
        {
            var attribute = fieldInfo.GetCustomAttribute<InjectComponentAttribute>();
            return attribute != null;
        }
    }
}