using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FactoryFactory.Registration;
using FactoryFactory.Util;

namespace FactoryFactory.Resolution
{
    public class ResolverBuilder : IResolverBuilder
    {
        private readonly IList<IServiceDefinition> _definitions;
        private readonly Configuration _configuration;
        private IList<IResolver> _resolvers;
        private IResolver _enumerableResolver;
        private IResolver _instanceResolver;

        public Type InstanceType { get; }

        public Type EnumerableType { get; }

        public ResolverBuilder(Type type, IList<IServiceDefinition> definitions,
            Configuration configuration)
        {
            _definitions = definitions;
            _configuration = configuration;
            if (type.IsEnumerable()) {
                EnumerableType = type;
                InstanceType = type.GenericTypeArguments.Single();
            }
            else {
                InstanceType = type;
                if (type.IsPointer || type.IsValueType) {
                    EnumerableType = null;
                }
                else {
                    EnumerableType = typeof(IEnumerable<>).MakeGenericType(type);
                }
            }
        }

        public IResolver GetEnumerableResolver()
        {
            EnsureResolvers();
            return _enumerableResolver;
        }

        public IResolver GetInstanceResolver()
        {
            EnsureResolvers();
            return _instanceResolver;
        }

        private void EnsureResolvers()
        {
            if (_enumerableResolver != null || _instanceResolver != null) return;

            if (EnumerableType == null || InstanceType == null) {
                _enumerableResolver = new NonResolver(EnumerableType ?? InstanceType);
                _instanceResolver = new NonResolver(InstanceType ?? EnumerableType);
                return;
            }

            _resolvers = new List<IResolver>();
            foreach (var definition in _definitions) {
                foreach (var implementationType in definition.GetTypes(InstanceType)) {
                    _resolvers.Add(CreateResolverByType(definition, implementationType));
                }

                foreach (var implementation in definition.GetInstances(InstanceType)) {
                    _resolvers.Add(CreateResolverByInstance(definition, implementation));
                }

                foreach (var expression in definition.GetExpressions(InstanceType)) {
                    _resolvers.Add(CreateResolverByExpression(definition, expression));
                }
            }

            if (!_resolvers.Any()) {
                if (_configuration.Options.AutoResolve &&
                    !InstanceType.IsAbstract &&
                    !InstanceType.IsValueType &&
                    !InstanceType.IsGenericTypeDefinition) {
                    var definition = new ServiceDefinition(
                        InstanceType,
                        implementationType: InstanceType,
                        lifecycle: _configuration.Options.DefaultLifecycle
                    );
                    var resolver = CreateResolverByType(definition, InstanceType);
                    if (resolver != null && resolver.CanResolve) {
                        _resolvers.Add(CreateResolverByType(definition, InstanceType));
                    }
                }
                else {
                    _resolvers.Add(new NonResolver(InstanceType));
                }
            }

            if (EnumerableType != null) {
                var enumerableResolverType =
                    typeof(EnumerableResolver<>).MakeGenericType(InstanceType);

                _enumerableResolver =
                    (IResolver)Activator.CreateInstance(enumerableResolverType, _resolvers);
                _instanceResolver = new SingleResolver(_resolvers, InstanceType).ActualResolver;
            }
        }

        private IResolver CreateResolverByType(IServiceDefinition definition, Type implementationType)
        {
            var constructor = _configuration.Options.ConstructorSelector.SelectConstructor
                (implementationType, _configuration);
            if (constructor == null) return new NonResolver(InstanceType);
            var expression =
                _configuration.Options.ExpressionBuilder.CreateResolutionExpressionFromDefaultConstructor(constructor);
            var isTracked = typeof(IDisposable).IsAssignableFrom(implementationType);
            var resolver = new ExpressionResolver(definition, expression, InstanceType);
            return BuildUp(resolver, definition, isTracked);
        }

        private IResolver CreateResolverByInstance(IServiceDefinition definition, object implementation)
        {
            var resolver = new InstanceResolver(definition, implementation);
            return BuildUp(resolver, definition, implementation is IDisposable);
        }

        private IResolver CreateResolverByExpression
            (IServiceDefinition definition, Expression<Func<ServiceRequest, object>> expression)
        {
            if (expression.Body is NewExpression nex) {
                expression = _configuration.Options.ExpressionBuilder
                    .CreateResolutionExpressionFromConstructorExpression(nex);
            }
            var resolver = new ExpressionResolver(definition, expression, InstanceType);
            return BuildUp(resolver, definition, true);
        }

        private IResolver BuildUp(IResolver resolver, IServiceDefinition definition, bool isTracked)
        {
            /*
             * IMPORTANT:
             *
             * Resplvers must be decorated in the following order:
             *
             *  1. Instance resolver / Expression resolver
             *  2. Decorator resolver
             *  3. Service tracker resolver
             *  4. Service cache resolver
             *  5. Conditional resolver
             *  6. Enumerable resolver / Selector resolver
             */

            IResolver built = resolver;

            var interceptorType = typeof(IInterceptor<>).MakeGenericType(InstanceType);
            if (_configuration.CanResolve(interceptorType)) {
                var interceptorResolverType
                    = typeof(InterceptorResolver<>).MakeGenericType(InstanceType);
                built = (IResolver)Activator.CreateInstance
                    (interceptorResolverType, built, _configuration);
            }

            if (isTracked && definition.Lifecycle.Tracked) {
                built = new ServiceTrackerResolver(definition, built);
            }

            if (resolver is ExpressionResolver exr && definition.Lifecycle.Cached) {
                built = new ServiceCacheResolver(definition, built, exr.Key);
            }

            if (definition.Precondition != null) {
                built = new ConditionalResolver(definition, built);
            }

            return built;
        }
    }
}
