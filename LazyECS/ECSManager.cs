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
        public EntityManager EntityManager;
        
        private readonly IComponentAssignCreator componentAssignCreator;
        private EntitySystemsController entitySystemsController;
        private readonly HashSet<Entity> entities;
        private uint lastId;
        public EntityContainer EntityContainer;

        private HashSet<Type> registeredSystemsProcessing = new HashSet<Type>();

        public ECSManager(IComponentAssignCreator componentAssignCreator)
        {
            this.componentAssignCreator = componentAssignCreator;
            entities = new HashSet<Entity>();
            EntityContainer = new EntityContainer();
        }

        public void Register<T>()
            where T : ISystemProcessing
        {
            registeredSystemsProcessing.Add(typeof(T));
        }

        public EntitySystemsController Init()
        {
            EntityManager = new EntityManager(this);

            var systemInfos = new Dictionary<Type, SystemProcessingInfo>();

            foreach (var type in registeredSystemsProcessing)
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

                systemInfos.Add(type,
                    new SystemProcessingInfo {SystemProcessing = instance, NeededComponents = assignInfo});
            }

            entitySystemsController = new EntitySystemsController(systemInfos, EntityManager);
            return entitySystemsController;
        }

        public void ReattachComponents(Type type)
        {
            var processing = entitySystemsController.SystemInfos.Values.Where(e =>
                e.NeededComponents.Any(x => x.TypeComponent.GetElementType() == type));
            foreach (var processingInfo in processing)
                AttachComponents(processingInfo, processingInfo.AttachedEntity);
        }

        public void BindComponentTo(Entity entity, IComponentData componentData, bool isAttach = true)
        {
            var componentType = componentData.GetType();
            var processing = entitySystemsController.SystemInfos.Values.Where(e =>
                e.NeededComponents.Any(x => x.TypeComponent.GetElementType() == componentType));
            foreach (var processingInfo in processing)
            {
                if (processingInfo.NeededComponents.All(e =>
                    EntityContainer.HasComponent(entity, e.TypeComponent.GetElementType())))
                {
                    processingInfo.AttachedEntity.Add(entity);
                    if (isAttach)
                        AttachComponents(processingInfo, processingInfo.AttachedEntity);
                }
            }
        }

        public void UnbindComponent(Entity entity, Type componentType, bool isAttach = true)
        {
            var processing = entitySystemsController.SystemInfos.Values.Where(e =>
                e.NeededComponents.Any(x => x.TypeComponent.GetElementType() == componentType));
            foreach (var processingInfo in processing)
            {
                processingInfo.AttachedEntity.Remove(entity);
                if (isAttach)
                    AttachComponents(processingInfo, processingInfo.AttachedEntity);
            }
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(++lastId);
            entities.Add(entity);
            EntityContainer.AddEntity(entity);
            return entity;
        }

        public void AttachComponents(SystemProcessingInfo processingInfo, List<Entity> entities)
        {
            foreach (var field in processingInfo.NeededComponents)
            {
                var components = Array.CreateInstance(field.TypeComponent.GetElementType(), entities.Count);
                for (var i = 0; i < entities.Count; i++)
                    components.SetValue(
                        EntityContainer.GetComponent(entities[i], field.TypeComponent.GetElementType()), i);
                field.AttachToSystem(processingInfo.SystemProcessing, components as IComponentData[]);
            }
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
            EntityContainer.RemoveEntity(entity);
        }

        private bool CheckField(FieldInfo fieldInfo)
        {
            var attribute = fieldInfo.GetCustomAttribute<InjectComponentAttribute>();
            return attribute != null;
        }
    }
}