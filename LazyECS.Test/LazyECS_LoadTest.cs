using System;
using System.Diagnostics;
using LazyECS.Test.TestComponents;
using LazyECS.Test.TestSystems;
using LazyECS.Tools;
using NUnit.Framework;

namespace LazyECS.Test
{
    public class LazyECS_LoadTest
    {
        private ECSManager ecsManager;
        private EntitySystemsController entitySystems;

        [SetUp]
        public void Setup()
        {
            ecsManager = new ECSManager(new ComponentAssignCreator());
            ecsManager.Register<AddValueProcessing>();
            entitySystems = ecsManager.Init();
        }

        [Test]
        public void LoadTest_WithManyEntity()
        {
            var countEntity = 200000;
            for (var i = 0; i < countEntity; i++)
            {
                var entity = ecsManager.CreateEntity();
                ecsManager.EntityManager.AddComponent(entity, new ValueAddComponent { Value = 0});
            }
            ecsManager.EntityManager.ExecuteEvents();
            
            var sw = Stopwatch.StartNew();
            entitySystems.OnUpdate();
            sw.Stop();
            
            Console.WriteLine($"Time execute: {sw.ElapsedMilliseconds} ms. (FPS: {1000/sw.ElapsedMilliseconds})");
            Assert.That(sw.ElapsedMilliseconds, Is.LessThanOrEqualTo(1000/60));
        }
    }
}