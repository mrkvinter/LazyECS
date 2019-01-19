using System;
using System.Collections.Generic;
using LazyECS.Models;

namespace LazyECS
{
    public class EntitySystemsController
    {
        internal readonly Dictionary<Type, SystemProcessingInfo> SystemInfos;


        public EntitySystemsController(Dictionary<Type, SystemProcessingInfo> systemInfos)
        {
            SystemInfos = systemInfos;
        }

        public void OnUpdate()
        {
            foreach (var processing in SystemInfos)
            {
                var processingInfo = processing.Value;
                foreach (var entity in processingInfo.AttachedEntity)
                {
                    AttachComponents(processingInfo, entity);
                    processingInfo.SystemProcessing.Execute();
                }
            }
        }

        private static void AttachComponents(SystemProcessingInfo processingInfo, Entity entity)
        {
            foreach (var field in processingInfo.NeededComponents)
            {
                var component = entity.GetComponent(field.TypeComponent);
                field.AttachToSystem(processingInfo.SystemProcessing, component);
            }
        }
    }
}