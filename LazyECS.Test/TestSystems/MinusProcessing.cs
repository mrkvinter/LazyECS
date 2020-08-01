using LazyECS.Component;
using LazyECS.System;
using LazyECS.Test.TestComponents;
using NotImplementedException = System.NotImplementedException;

namespace LazyECS.Test.TestSystems
{
    public class MinusProcessing : ISystemProcessing
    {
        [InjectComponent] public ValueMinusComponent[] ValueAddComponent;

        public void Execute()
        {
            for(var i = 0; i < ValueAddComponent.Length; i++)
                ValueAddComponent[i].Value--;
        }
    }
}