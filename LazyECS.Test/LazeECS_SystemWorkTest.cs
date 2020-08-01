using LazyECS.Test.TestComponents;
using LazyECS.Test.TestSystems;
using LazyECS.Tools;
using NUnit.Framework;

namespace LazyECS.Test
{
    public class LazeEcsTests
    {
        private ECSManager ecsManager;

        [SetUp]
        public void Setup()
        {
            ecsManager = new ECSManager(new ComponentAssignCreator());
        }

        [Test]
        public void CorrectWorkWithOneEntityOneComponent()
        {
            ecsManager.Register<AddValueProcessing>();
            var entitySystems = ecsManager.Init();

            var entity = ecsManager.CreateEntity();
            var valueAddComponent = new ValueAddComponent { Value = 0 };
            ecsManager.EntityManager.AddComponent(entity, valueAddComponent);

            for(var i = 0; i < 5; i++)
                entitySystems.OnUpdate();
            
            Assert.AreEqual(5, valueAddComponent.Value);
        }
        
        [Test]
        public void CorrectWorkWithOneEntityManyComponent()
        {
            ecsManager.Register<AddValueProcessing>();
            ecsManager.Register<MinusProcessing>();
            var entitySystems = ecsManager.Init();

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
            ecsManager.Register<AddValueProcessing>();
            var entitySystems = ecsManager.Init();

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
            ecsManager.Register<AddValueProcessing>();
            var entitySystems = ecsManager.Init();

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
            ecsManager.Register<AddValueProcessing>();
            var entitySystems = ecsManager.Init();

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
            ecsManager.Register<MultiplyProcessing>();
            var entitySystems = ecsManager.Init();

            var entity = ecsManager.CreateEntity();
            var valueComponent = new ValueComponent { Value = 2};
            ecsManager.EntityManager.AddComponent(entity, valueComponent);

            entitySystems.OnUpdate();
            ecsManager.EntityManager.AddComponent(entity, new MultiplayComponent { Value = 2});

            entitySystems.OnUpdate();
            
            Assert.AreEqual(4, valueComponent.Value);
        }
    }
}