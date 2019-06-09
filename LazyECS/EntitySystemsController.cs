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
        private readonly EntityManager entityManager;


        public EntitySystemsController(Dictionary<Type, SystemProcessingInfo> systemInfos, EntityManager entityManager)
        {
            SystemInfos = systemInfos;
            this.entityManager = entityManager;
        }

        public void OnStart()
        {
            foreach (var processing in SystemInfos)
                processing.Value.SystemProcessing.Start();
        }

        public void OnUpdate()
        {
            entityManager.ExecuteEvents();

            foreach (var processing in SystemInfos)
            {
                var processingInfo = processing.Value;
                if (processing.Value.AttachedEntity.Count > 0)
                    processingInfo.SystemProcessing.Execute();
            }
        }
    }
}