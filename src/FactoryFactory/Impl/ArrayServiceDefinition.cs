using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FactoryFactory.Impl
{
    public class ArrayServiceDefinition : IServiceDefinition
    {
        public IEnumerable<Type> GetTypes(Type requestedType)
        {
            yield break;
        }

        public IEnumerable<object> GetInstances(Type requestedType)
        {
            yield break;
        }

        public IEnumerable<Expression<Func<ServiceRequest, object>>> GetExpressions(
            Type requestedType)
        {
            if (!requestedType.IsArray) yield break;
            if (requestedType.GetArrayRank() != 1) yield break;
            var instanceType = requestedType.GetElementType();
            if (instanceType.IsValueType) yield break;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(instanceType);
            var req = Expression.Parameter(typeof(ServiceRequest), "req");
            var subRequestExpression = Expression.Call(
                req,
                // ReSharper disable once AssignNullToNotNullAttribute
                typeof(ServiceRequest).GetMethod(nameof(ServiceRequest.CreateDependencyRequest)),
                Expression.Constant(enumerableType)
            );
            var containerExpression = Expression.Property(
                req,
                // ReSharper disable once AssignNullToNotNullAttribute
                typeof(ServiceRequest).GetProperty(nameof(ServiceRequest.Container))
            );

            var method = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray));
            method = method.MakeGenericMethod(instanceType);

            var getServiceExpression =
                Expression.Call(
                    method,
                    Expression.Convert(
                        Expression.Call(
                            containerExpression,
                            // ReSharper disable once AssignNullToNotNullAttribute
                            typeof(IContainer).GetMethod(nameof(IContainer.GetService),
                                new[] {typeof(ServiceRequest)}),
                            subRequestExpression
                        ),
                        enumerableType
                    )
                );

            yield return Expression.Lambda<Func<ServiceRequest, object>>(
                getServiceExpression,
                req
            );
        }

        public Func<ServiceRequest, bool> Precondition => null;

        public ILifecycle Lifecycle => FactoryFactory.Lifecycle.Untracked;

        public int Priority => 0;
    }
}
