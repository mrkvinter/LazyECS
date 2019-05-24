using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LazyECS.Component;
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

        public void OnStart()
        {
            foreach (var processing in SystemInfos)
                processing.Value.SystemProcessing.Start();
        }

        public void OnUpdate()
        {
            foreach (var processing in SystemInfos)
            {
                var processingInfo = processing.Value;
                if (processing.Value.AttachedEntity.Count > 0)
                    processingInfo.SystemProcessing.Execute();
            }
        }

        public static void AttachComponents(SystemProcessingInfo processingInfo, List<IEntity> entities)
        {
            foreach (var field in processingInfo.NeededComponents)
            {
                var components = Array.CreateInstance(field.TypeComponent.GetElementType(), entities.Count);
                for (var i = 0; i < entities.Count; i++)
                    components.SetValue(
                        entities[i].GetComponent(field.TypeComponent.GetElementType()), i);
                field.AttachToSystem(processingInfo.SystemProcessing, components as IComponentData[]);
            }
        }
    }
}