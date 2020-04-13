using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FactoryFactory.Registration.Dsl.Decriptors;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.Registration.Dsl
{
    public class InterceptionDefinitionBuilder<TService> : IInterceptionDefinitionClause<TService>
        where TService : class
    {
        private readonly Registry _registry;
        private readonly Type _interceptorType;

        private ILifecycle _lifecycle;
        private ServiceLifetime? _lifetime;
        private Func<ServiceRequest, bool> _precondition;
        private Type _implementationType;
        private object _implementationInstance;
        private Expression<Func<ServiceRequest, object>> _implementationFactory;

        public InterceptionDefinitionBuilder(Registry registry)
        {
            _registry = registry;
            _interceptorType = typeof(IInterceptor<TService>);
        }

        public IInterceptionDefinitionClause<TService> Lifecycle(ILifecycle lifecycle)
        {
            _lifecycle = lifecycle;
            _lifetime = null;
            return this;
        }

        public IInterceptionDefinitionClause<TService> Lifecycle(ServiceLifetime lifetime)
        {
            _lifetime = lifetime;
            _lifecycle = null;
            return this;
        }

        public IInterceptionDefinitionClause<TService> Singleton()
            => Lifecycle(ServiceLifetime.Singleton);

        public IInterceptionDefinitionClause<TService> Scoped()
            => Lifecycle(ServiceLifetime.Scoped);

        public IInterceptionDefinitionClause<TService> Transient()
            => Lifecycle(ServiceLifetime.Transient);

        public IInterceptionDefinitionClause<TService> Untracked()
            => Lifecycle(FactoryFactory.Lifecycle.Untracked);

        public IInterceptionDefinitionClause<TService> Precondition
            (Func<ServiceRequest, bool> precondition)
        {
            _precondition = precondition;
            return this;
        }

        private ServiceDescriptor Build()
        {
            /*
             * Return a basic .NET Core ServiceDescriptor if we can.
             */

            if (_implementationFactory == null && _precondition == null && _lifetime != null) {
                if (_implementationType != null) {
                    return new ServiceDescriptor(_interceptorType, _implementationType, _lifetime.Value);
                }
                else if (_implementationInstance != null) {
                    return new ServiceDescriptor(_interceptorType, _implementationInstance);
                }
            }

            /*
             * For FactoryFactory-specific stuff, return a derived service descriptor that
             * implements IServiceDefinition.
             */
            if (_implementationType != null) {
                return new DefinitionServiceDescriptor
                    (_interceptorType, _implementationType, _lifetime, _lifecycle, _precondition);
            }
            else if (_implementationInstance != null) {
                return new DefinitionServiceDescriptor
                    (_interceptorType, _implementationInstance, _precondition);
            }
            else if (_implementationFactory != null) {
                return new DefinitionServiceDescriptor
                    (_interceptorType, _implementationFactory, _lifetime, _lifecycle, _precondition);
            }
            else {
                throw new InvalidOperationException(
                    $"The service descriptor for {_interceptorType.FullName} is in an invalid state. " +
                    $"No implementation details could be found."
                );
            }
        }

        public Registry With<TImplementation>() where TImplementation : IInterceptor<TService>
        {
            _implementationType = typeof(TImplementation);
            _implementationFactory = null;
            _implementationInstance = null;
            _registry.Add(Build());
            return _registry;
        }

        public Registry With(IInterceptor<TService> implementation)
        {
            _implementationFactory = null;
            _implementationType = null;
            _implementationInstance = implementation;
            Untracked();
            _registry.Add(Build());
            return _registry;
        }

        public Registry With(Expression<Func<ServiceRequest, IInterceptor<TService>>> factory)
        {
            _implementationFactory = Expression.Lambda<Func<ServiceRequest, object>>(
                factory.Body,
                factory.Parameters
            );
            _implementationType = null;
            _implementationInstance = null;
            _registry.Add(Build());
            return _registry;
        }

        public Registry By(Func<ServiceRequest, Func<TService>, TService> decoratorFunc)
        {
            return Untracked().With(new Interceptor<TService>(decoratorFunc));
        }

        public Registry By(Func<Func<TService>, TService> decoratorFunc)
        {
            return Untracked()
                .With(new Interceptor<TService>
                    ((req, svc) => decoratorFunc(svc)));
        }
    }
}
