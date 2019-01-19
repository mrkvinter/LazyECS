using LazyECS.Component;
using LazyECS.System;
using LazyECS.Test.TestComponents;
using NotImplementedException = System.NotImplementedException;

namespace LazyECS.Test.TestSystems
{
    public class MultiplyProcessing : ISystemProcessing
    {
        [InjectComponent] public MultiplayComponent MultiplayComponent;
        [InjectComponent] public ValueComponent ValueComponent;
        
        public void Execute()
        {
            ValueComponent.Value *= MultiplayComponent.Value;
        }
    }
}