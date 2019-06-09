using LazyECS.Component;
using LazyECS.System;
using LazyECS.Test.TestComponents;
using NotImplementedException = System.NotImplementedException;

namespace LazyECS.Test.TestSystems
{
    public class AddValueProcessing : ISystemProcessing
    {
        [InjectComponent] public ValueAddComponent[] ValueAddComponent;

        public void Start()
        {
            
        }

        public void Execute()
        {
            for (int i = 0; i < ValueAddComponent.Length ; i++)
            {
                ValueAddComponent[i].Value++;
            }
        }
    }
}