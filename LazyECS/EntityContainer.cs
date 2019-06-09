using System;
using System.Collections.Generic;
using System.Linq;
using LazyECS.Component;

namespace LazyECS
{
    public class EntityContainer
    {
        public Dictionary<Entity, Dictionary<Type, IComponentData>> ComponentsContainer = new Dictionary<Entity, Dictionary<Type, IComponentData>>();
        
        public bool HasComponent(Entity entity, Type typeComponent)
        {
            return ComponentsContainer.TryGetValue(entity, out var components) && components.ContainsKey(typeComponent);
        }

        public void AddEntity(Entity entity)
        {
            ComponentsContainer.Add(entity, new Dictionary<Type, IComponentData>());
        }
        
        public void RemoveEntity(Entity entity)
        {
            ComponentsContainer.Remove(entity);
        }

        public IComponentData[] GetComponents(Entity entity)
        {
            return ComponentsContainer[entity].Select(e => e.Value).ToArray();
        }
        
        public T GetComponent<T>(Entity entity)
            where T : IComponentData
        {
            return (T)ComponentsContainer[entity][typeof(T)];
        }
        
        public IComponentData GetComponent(Entity entity, Type type)
        {
            return ComponentsContainer[entity][type];
        }

        public void AddComponent(Entity entity, IComponentData componentData)
        {
            ComponentsContainer[entity].Add(componentData.GetType(), componentData);
        }
        
        public void RemoveComponent(Entity entity, Type type)
        {
            ComponentsContainer[entity].Remove(type);
        }
    }
}