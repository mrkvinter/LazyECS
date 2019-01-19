using System;
using System.Collections.Generic;
using System.Linq;
using LazyECS.Component;

namespace LazyECS
{
    public class Entity
    {
        public uint Id { get; }

        private Dictionary<Type, IComponentData> componentDatas = new Dictionary<Type, IComponentData>();
        private ECSManager ecsManager;

        internal Entity(ECSManager ecsManager, uint id)
        {
            Id = id;
            this.ecsManager = ecsManager;
        }

        public void AddComponent<T>(T component)
            where T : IComponentData
        {
            componentDatas.Add(typeof(T), component);
            ecsManager.BindComponentTo(this, component);
        }

        public T GetComponent<T>()
            where T : IComponentData
        {
            IComponentData result;
            if (componentDatas.TryGetValue(typeof(T), out result))
                return (T)result;

            return default(T);
        }
        
        public IComponentData GetComponent(Type typeComponent)
        {
            return componentDatas.TryGetValue(typeComponent, out var result) ? result : default(IComponentData);
        }

        public IComponentData[] GetComponents()
        {
            return componentDatas.Values.ToArray();
        }

        public void RemoveComponent<T>()
        {
            var type = typeof(T);
            var component = componentDatas[type];
            componentDatas.Remove(type);
            ecsManager.UnbindComponent(this, component);
        }
        
        public void RemoveComponent(Type type)
        {
            var component = componentDatas[type];
            componentDatas.Remove(type);
            ecsManager.UnbindComponent(this, component);
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Id;
        }

        public bool Equals(Entity entity)
        {
            return Id == entity.Id;
        }
    }
}