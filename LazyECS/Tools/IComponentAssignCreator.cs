using System;
using System.Linq.Expressions;
using System.Reflection;
using LazyECS.Component;
using LazyECS.System;

namespace LazyECS.Tools
{
    public interface IComponentAssignCreator
    {
        Action<ISystemProcessing, IComponentData[]> GetExpression(Type tProcessing, FieldInfo fieldComponentInfo);
    }

    public class ComponentAssignCreator : IComponentAssignCreator
    {
        public Action<ISystemProcessing, IComponentData[]> GetExpression(Type tProcessing, FieldInfo fieldComponentInfo)
        {
            var processingParameterEx = Expression.Parameter(typeof(ISystemProcessing), "p");
            var componentsParameterEx = Expression.Parameter(typeof(IComponentData[]), "c");

            var field = Expression.Field(Expression.Convert(processingParameterEx, tProcessing), fieldComponentInfo);
            var memberAssignment = Expression.Assign(field,
                Expression.Convert(componentsParameterEx, fieldComponentInfo.FieldType));
            var lambda = Expression
                .Lambda<Action<ISystemProcessing, IComponentData[]>>(memberAssignment, processingParameterEx,
                    componentsParameterEx).Compile();

            return lambda;
        }
    }
}