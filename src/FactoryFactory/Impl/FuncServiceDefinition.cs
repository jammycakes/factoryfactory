using System;
using System.Linq;
using System.Linq.Expressions;

namespace FactoryFactory.Impl
{
    internal class FuncServiceDefinition : ServiceDefinition
    {
        public FuncServiceDefinition()
            : base(typeof(Func<>), implementationType: typeof(Func<>), lifecycle: Lifecycle.Untracked)
        {
        }

        protected override ServiceDefinition Close(Type requestedType)
        {
            var instanceType = requestedType.GetGenericArguments().Last();
            var req = Expression.Parameter(typeof(ServiceRequest), "req");

            var subRequestExpression = Expression.Call(
                req,
                typeof(ServiceRequest).GetMethod(nameof(ServiceRequest.CreateDependencyRequest)),
                Expression.Constant(instanceType)
            );

            var containerExpression = Expression.Property(
                req,
                typeof(ServiceRequest).GetProperty(nameof(ServiceRequest.Container))
            );

            var getServiceExpression =
                Expression.Convert(
                    Expression.Call(
                        containerExpression,
                        typeof(Container).GetMethod(nameof(Container.GetService),
                            new [] {typeof(ServiceRequest)}),
                        subRequestExpression
                    ),
                    instanceType
                );

            var requestedExpression = Expression.Lambda(
                requestedType,
                getServiceExpression
            );

            var factoryExpression = Expression.Lambda<Func<ServiceRequest, object>>(
                requestedExpression,
                req
            );

        return new ServiceDefinition(
                requestedType,
                implementationFactory: factoryExpression,
                lifecycle: this.Lifecycle,
                precondition: this.Precondition
            );
        }
    }
}
