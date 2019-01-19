using LazyECS.Component;
using LazyECS.System;
using LazyECS.Test.TestComponents;

namespace LazyECS.Test.TestSystems
{
    public class AddValueProcessing : ISystemProcessing
    {
        [InjectComponent] public ValueAddComponent ValueAddComponent;
            
        public void Execute()
        {
            ValueAddComponent.Value++;
        }
    }
}