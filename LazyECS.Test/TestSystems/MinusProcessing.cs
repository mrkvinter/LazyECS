using LazyECS.Component;
using LazyECS.System;
using LazyECS.Test.TestComponents;

namespace LazyECS.Test.TestSystems
{
    public class MinusProcessing : ISystemProcessing
    {
        [InjectComponent] public ValueMinusComponent ValueAddComponent;
            
        public void Execute()
        {
            ValueAddComponent.Value--;
        }
    }
}