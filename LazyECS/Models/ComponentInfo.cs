using System;
using LazyECS.Component;
using LazyECS.System;

namespace LazyECS.Models
{
    public class ComponentInfo
    {
        public Type TypeComponent { get; }
        public Action<ISystemProcessing, IComponentData> AttachToSystem { get; }

        public ComponentInfo(Type typeComponent, Action<ISystemProcessing, IComponentData> attachToSystem)
        {
            TypeComponent = typeComponent;
            AttachToSystem = attachToSystem;
        }
    }
}