using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly HashSet<IEntity> entities;
        private uint lastId;

        public ECSManager(IComponentAssignCreator componentAssignCreator)
        {
            this.componentAssignCreator = componentAssignCreator;
            entities = new HashSet<IEntity>();
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
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(CheckField);

                var assignInfo = new List<ComponentInfo>();
                foreach (var fieldInfo in fields)
                {
                    var expressionAssign = componentAssignCreator.GetExpression(type, fieldInfo);
                    assignInfo.Add(
                        new ComponentInfo(fieldInfo.FieldType, expressionAssign));
                }

                var instance = Activator.CreateInstance(type) as ISystemProcessing;
                  
                systemProcessings.Add(type, instance);


                systemInfos.Add(type,
                    new SystemProcessingInfo {SystemProcessing = instance, NeededComponents = assignInfo});
            }

            entitySystemsController = new EntitySystemsController(systemInfos);
            return entitySystemsController;
        }

        public void ReattachComponents<T>()
        {
            var processing = entitySystemsController.SystemInfos.Values.Where(e =>
                e.NeededComponents.Any(x => x.TypeComponent.GetElementType() == typeof(T)));
            foreach (var processingInfo in processing)
                EntitySystemsController.AttachComponents(processingInfo, processingInfo.AttachedEntity);
        }

        public void BindComponentTo(IEntity entity, IComponentData componentData, bool isAttach = true)
        {
            var componentType = componentData.GetType();
            var processing = entitySystemsController.SystemInfos.Values.Where(e =>
                e.NeededComponents.Any(x => x.TypeComponent.GetElementType() == componentType));
            foreach (var processingInfo in processing)
            {
                if (processingInfo.NeededComponents.All(e => entity.HasComponent(e.TypeComponent.GetElementType())))
                {
                    processingInfo.AttachedEntity.Add(entity);
                    if (isAttach)
                        EntitySystemsController.AttachComponents(processingInfo, processingInfo.AttachedEntity);
                }
            }
        }

        public void UnbindComponent(Entity entity, IComponentData componentData)
        {
            var componentType = componentData.GetType();
            var processing = entitySystemsController.SystemInfos.Values.Where(e =>
                e.NeededComponents.Any(x => x.TypeComponent.GetElementType() == componentType));
            foreach (var processingInfo in processing)
            {
                processingInfo.AttachedEntity.Remove(entity);
                EntitySystemsController.AttachComponents(processingInfo, processingInfo.AttachedEntity);
            }
        }

        public IEntity CreateEntity()
        {
            var entity = new Entity(this, ++lastId);
            entities.Add(entity);
            return entity;
        }

        public void RemoveEntity(IEntity entity)
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