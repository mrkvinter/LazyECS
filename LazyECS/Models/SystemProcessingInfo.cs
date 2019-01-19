using System.Collections.Generic;
using LazyECS.System;

namespace LazyECS.Models
{
    public class SystemProcessingInfo
    {
        public ISystemProcessing SystemProcessing;

        public List<ComponentInfo> NeededComponents = new List<ComponentInfo>();

        public List<Entity> AttachedEntity = new List<Entity>();
    }
}