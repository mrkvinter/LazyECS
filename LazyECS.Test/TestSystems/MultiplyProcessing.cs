using LazyECS.Component;
using LazyECS.System;
using LazyECS.Test.TestComponents;
using NotImplementedException = System.NotImplementedException;

namespace LazyECS.Test.TestSystems
{
    public class MultiplyProcessing : ISystemProcessing
    {
        [InjectComponent] public MultiplayComponent[] MultiplayComponent;
        [InjectComponent] public ValueComponent[] ValueComponent;

        public void Execute()
        {
            for(var i = 0; i < ValueComponent.Length; i++)
                ValueComponent[i].Value *= MultiplayComponent[i].Value;
        }
    }
}