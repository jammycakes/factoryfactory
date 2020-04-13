using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FactoryFactory.Util;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.Registration.Dsl.Descriptors
{
    public class DefinitionServiceDescriptor : ServiceDescriptor, IServiceDefinition
    {
        private Expression<Func<ServiceRequest, object>> _implementationFactory;

        public DefinitionServiceDescriptor(
            Type serviceType,
            Type implementationType,
            ServiceLifetime? lifetime,
            ILifecycle lifecycle,
            Func<ServiceRequest, bool> precondition)
            : base(serviceType, implementationType, lifetime ?? ServiceLifetime.Transient)
        {
            Lifecycle = lifecycle ??
                        (lifetime.HasValue
                            ? FactoryFactory.Lifecycle.Get(lifetime.Value)
                            : FactoryFactory.Lifecycle.Default);
            Precondition = precondition;
        }

        public DefinitionServiceDescriptor(
            Type serviceType,
            object implementation,
            Func<ServiceRequest, bool> precondition)
            : base(serviceType, implementation)
        {
            Precondition = precondition;
            Lifecycle = FactoryFactory.Lifecycle.Untracked;
        }


        public DefinitionServiceDescriptor(
            Type serviceType,
            Expression<Func<ServiceRequest, object>> implementationFactory,
            ServiceLifetime? lifetime,
            ILifecycle lifecycle,
            Func<ServiceRequest, bool> precondition)
            : base(serviceType, svc => null, lifetime ?? ServiceLifetime.Transient)
        {
            _implementationFactory = implementationFactory;
            Lifecycle = lifecycle ??
                        (lifetime.HasValue
                            ? FactoryFactory.Lifecycle.Get(lifetime.Value)
                            : FactoryFactory.Lifecycle.Default);
            Precondition = precondition;
        }


        public IEnumerable<Type> GetTypes(Type requestedType)
        {
            if (ImplementationType != null) {
                if (requestedType == ServiceType) {
                    yield return ImplementationType;
                }
                else if (requestedType.IsGenericType && ServiceType.IsGenericTypeDefinition) {
                    var openedRequest = requestedType.GetGenericTypeDefinition();
                    if (openedRequest == ServiceType) {
                        Type closedRequest;
                        if (ImplementationType.TryMakeGenericType
                            (requestedType.GenericTypeArguments, out closedRequest)) {
                            yield return closedRequest;
                        }
                    }
                }
            }
        }

        public IEnumerable<object> GetInstances(Type requestedType)
        {
            if (ImplementationInstance != null && requestedType == ServiceType) {
                yield return ImplementationInstance;
            }
        }

        public IEnumerable<Expression<Func<ServiceRequest, object>>> GetExpressions(Type requestedType)
        {
            if (ImplementationFactory != null && requestedType == ServiceType) {
                yield return _implementationFactory;
            }
        }

        public Func<ServiceRequest, bool> Precondition { get; }

        public ILifecycle Lifecycle { get; }

        public int Priority => ServiceType.IsGenericTypeDefinition ? 1000 : 2000;
    }
}
