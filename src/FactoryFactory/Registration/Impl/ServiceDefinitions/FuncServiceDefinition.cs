using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FactoryFactory.Registration.ServiceDefinitions
{
    internal class FuncServiceDefinition : IServiceDefinition
    {
        public IEnumerable<Type> GetTypes(Type requestedType)
        {
            yield break;
        }

        public IEnumerable<object> GetInstances(Type requestedType)
        {
            yield break;
        }

        public IEnumerable<Expression<Func<ServiceRequest, object>>> GetExpressions(Type requestedType)
        {
            if (!requestedType.IsGenericType) yield break;
            if (requestedType.GetGenericTypeDefinition() != typeof(Func<>)) yield break;

            var instanceType = requestedType.GetGenericArguments().Last();
            var req = Expression.Parameter(typeof(ServiceRequest), "req");

            var subRequestExpression = Expression.Call(
                req,
                // ReSharper disable once AssignNullToNotNullAttribute
                typeof(ServiceRequest).GetMethod(nameof(ServiceRequest.CreateDependencyRequest)),
                Expression.Constant(instanceType)
            );

            var containerExpression = Expression.Property(
                req,
                // ReSharper disable once AssignNullToNotNullAttribute
                typeof(ServiceRequest).GetProperty(nameof(ServiceRequest.Container))
            );

            var getServiceExpression =
                Expression.Convert(
                    Expression.Call(
                        containerExpression,
                        // ReSharper disable once AssignNullToNotNullAttribute
                        typeof(IContainer).GetMethod(nameof(IContainer.GetService),
                            new [] {typeof(ServiceRequest)}),
                        subRequestExpression
                    ),
                    instanceType
                );

            var requestedExpression = Expression.Lambda(
                requestedType,
                getServiceExpression
            );

            yield return Expression.Lambda<Func<ServiceRequest, object>>(
                requestedExpression,
                req
            );
        }

        public Func<ServiceRequest, bool> Precondition => null;
        public ILifecycle Lifecycle => FactoryFactory.Lifecycle.Untracked;
        public int Priority => 0;
    }
}
