using LazyECS.Test.TestComponents;
using LazyECS.Tools;
using NUnit.Framework;

namespace LazyECS.Test
{
    public class LazeEcsTests
    {
        private ECSManager ecsManager;
        private EntitySystemsController entitySystems;

        [SetUp]
        public void Setup()
        {
            ecsManager = new ECSManager(new ComponentAssignCreator());
            entitySystems = ecsManager.Init();
        }

        [Test]
        public void CorrectWorkWithOneEntityOneComponent()
        {
            var entity = ecsManager.CreateEntity();
            var valueAddComponent = new ValueAddComponent { Value = 0};
            ecsManager.EntityManager.AddComponent(entity, valueAddComponent);

            for(var i = 0; i < 5; i++)
                entitySystems.OnUpdate();
            
            Assert.AreEqual(5, valueAddComponent.Value);
        }
        
        [Test]
        public void CorrectWorkWithOneEntityManyComponent()
        {
            var entity = ecsManager.CreateEntity();
            var valueAddComponent = new ValueAddComponent { Value = 0};
            var valueMinusComponent = new ValueMinusComponent { Value = 0};
            ecsManager.EntityManager.AddComponent(entity, valueAddComponent);
            ecsManager.EntityManager.AddComponent(entity, valueMinusComponent);
            
            for(var i = 0; i < 5; i++)
                entitySystems.OnUpdate();
            
            Assert.AreEqual(5, valueAddComponent.Value);
            Assert.AreEqual(-5, valueMinusComponent.Value);
        }
        
        [Test]
        public void CorrectWorkWithTwoEntityOneComponent()
        {
            var entityA = ecsManager.CreateEntity();
            var entityB = ecsManager.CreateEntity();
            var valueAddComponentA = new ValueAddComponent { Value = 0};
            var valueAddComponentB = new ValueAddComponent { Value = 0};
            ecsManager.EntityManager.AddComponent(entityA, valueAddComponentA);
            ecsManager.EntityManager.AddComponent(entityB, valueAddComponentB);
            
            for(var i = 0; i < 5; i++)
                entitySystems.OnUpdate();
            
            Assert.AreEqual(5, valueAddComponentA.Value);
            Assert.AreEqual(5, valueAddComponentB.Value);
        }
        
        [Test]
        public void CorrectWorkWithTwoEntity_OneComponentAddedLater()
        {
            var entityA = ecsManager.CreateEntity();
            var entityB = ecsManager.CreateEntity();
            var valueAddComponentA = new ValueAddComponent { Value = 0};
            var valueAddComponentB = new ValueAddComponent { Value = 0};
            ecsManager.EntityManager.AddComponent(entityB, valueAddComponentB);
            
            for(var i = 0; i < 5; i++)
                entitySystems.OnUpdate();
            
            ecsManager.EntityManager.AddComponent(entityA, valueAddComponentA);

            for(var i = 0; i < 5; i++)
                entitySystems.OnUpdate();
            
            Assert.AreEqual(5, valueAddComponentA.Value);
            Assert.AreEqual(10, valueAddComponentB.Value);
        }
        
        [Test]
        public void CorrectWorkWithOneEntity_OneComponentRemoveLater()
        {
            var entity = ecsManager.CreateEntity();
            var valueAddComponent = new ValueAddComponent { Value = 0};
            ecsManager.EntityManager.AddComponent(entity, valueAddComponent);

            entitySystems.OnUpdate();
            ecsManager.EntityManager.RemoveComponent<ValueAddComponent>(entity);

            entitySystems.OnUpdate();
            
            Assert.AreEqual(1, valueAddComponent.Value);
        }
        
        [Test]
        public void CorrectWorkWithOneEntity_ProccesingRequireTwoComponent()
        {
            var entity = ecsManager.CreateEntity();
            var valueComponent = new ValueComponent() { Value = 2};
            ecsManager.EntityManager.AddComponent(entity, valueComponent);

            entitySystems.OnUpdate();
            ecsManager.EntityManager.AddComponent(entity, new MultiplayComponent { Value = 2});

            entitySystems.OnUpdate();
            
            Assert.AreEqual(4, valueComponent.Value);
        }
    }
}