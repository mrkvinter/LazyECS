using System;

namespace LazyECS.Component
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectComponentAttribute : Attribute
    {
    }
}