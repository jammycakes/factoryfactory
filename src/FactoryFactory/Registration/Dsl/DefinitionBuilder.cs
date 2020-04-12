using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FactoryFactory.Registration.Dsl.Decriptors;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.Registration.Dsl
{
    public class DefinitionBuilder : IDefinitionClause
    {
        private readonly Registry _registry;
        private readonly Type _serviceType;

        private ILifecycle _lifecycle;
        private ServiceLifetime? _lifetime;
        private Func<ServiceRequest, bool> _precondition;
        private Type _implementationType;
        private object _implementationInstance;
        private Expression<Func<ServiceRequest, object>> _implementationFactory;

        public DefinitionBuilder(Registry registry, Type serviceType)
        {
            _registry = registry;
            _serviceType = serviceType;
        }

        public IDefinitionClause Lifecycle(ILifecycle lifecycle)
        {
            _lifecycle = lifecycle;
            _lifetime = null;
            return this;
        }

        public IDefinitionClause Lifecycle(ServiceLifetime lifetime)
        {
            _lifetime = lifetime;
            _lifecycle = null;
            return this;
        }

        IDefinitionClause ILifecycleClause.Singleton() => Lifecycle(ServiceLifetime.Singleton);

        IDefinitionClause ILifecycleClause.Scoped() => Lifecycle(ServiceLifetime.Scoped);

        IDefinitionClause ILifecycleClause.Transient() => Lifecycle(ServiceLifetime.Transient);

        IDefinitionClause ILifecycleClause.Untracked() => Lifecycle(FactoryFactory.Lifecycle.Untracked);

        public IDefinitionClause Precondition(Func<ServiceRequest, bool> precondition)
        {
            _precondition = precondition;
            return this;
        }

        public Registry As(Type implementationType)
        {
            _implementationType = implementationType;
            _implementationFactory = null;
            _implementationInstance = null;
            _registry.Add(Build());
            return _registry;
        }

        public Registry As(object instance)
        {
            _implementationInstance = instance;
            _implementationFactory = null;
            _implementationType = null;
            _registry.Add(Build());
            return _registry;
        }

        public Registry As(Expression<Func<ServiceRequest, object>> factory)
        {
            _implementationFactory = factory;
            _implementationInstance = null;
            _implementationType = null;
            _registry.Add(Build());
            return _registry;
        }

        private ServiceDescriptor Build()
        {
            /*
             * Return a basic .NET Core ServiceDescriptor if we can.
             */

            if (_implementationFactory == null && _precondition == null && _lifetime != null) {
                if (_implementationType != null) {
                    return new ServiceDescriptor(_serviceType, _implementationType, _lifetime.Value);
                }
                else if (_implementationInstance != null) {
                    return new ServiceDescriptor(_serviceType, _implementationInstance);
                }
            }

            /*
             * For FactoryFactory-specific stuff, return a derived service descriptor that
             * implements IServiceDefinition.
             */
            if (_implementationType != null) {
                return new DefinitionServiceDescriptor
                    (_serviceType, _implementationType, _lifetime, _lifecycle, _precondition);
            }
            else if (_implementationInstance != null) {
                return new DefinitionServiceDescriptor
                    (_serviceType, _implementationInstance, _precondition);
            }
            else if (_implementationFactory != null) {
                return new DefinitionServiceDescriptor
                    (_serviceType, _implementationFactory, _lifetime, _lifecycle, _precondition);
            }
            else {
                throw new InvalidOperationException(
                    $"The service descriptor for {_serviceType.FullName} is in an invalid state. " +
                    $"No implementation details could be found."
                );
            }
        }
    }

    public class DefinitionBuilder<TService> : DefinitionBuilder, IDefinitionClause<TService>
        where TService : class
    {
        public DefinitionBuilder(Registry registry)
            : base(registry, typeof(TService))
        {
        }

        public new IDefinitionClause<TService> Lifecycle(ILifecycle lifecycle)
        {
            base.Lifecycle(lifecycle);
            return this;
        }

        public new IDefinitionClause<TService> Lifecycle(ServiceLifetime lifetime)
        {
            base.Lifecycle(lifetime);
            return this;
        }

        public IDefinitionClause<TService> Singleton() => Lifecycle(ServiceLifetime.Singleton);

        public IDefinitionClause<TService> Scoped() => Lifecycle(ServiceLifetime.Scoped);

        public IDefinitionClause<TService> Transient() => Lifecycle(ServiceLifetime.Transient);

        public IDefinitionClause<TService> Untracked() =>
            Lifecycle(FactoryFactory.Lifecycle.Untracked);

        public new IDefinitionClause<TService> Precondition(Func<ServiceRequest, bool> precondition)
        {
            base.Precondition(precondition);
            return this;
        }

        public Registry As<TImplementation>() where TImplementation : TService
        {
            return base.As(typeof(TImplementation));
        }

        public Registry As(TService instance)
        {
            return base.As(instance);
        }

        public Registry As(Expression<Func<ServiceRequest, TService>> factory)
        {
            var untypedFactory = Expression.Lambda<Func<ServiceRequest, object>>(
                factory.Body,
                factory.Parameters
            );

            return base.As(untypedFactory);
        }
    }
}
