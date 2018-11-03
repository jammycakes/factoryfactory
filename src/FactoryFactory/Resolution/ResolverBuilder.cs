using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FactoryFactory.Util;

namespace FactoryFactory.Resolution
{
    public class ResolverBuilder
    {
        private readonly IList<IServiceDefinition> _definitions;
        private readonly Configuration _configuration;
        private IList<IResolver> _resolvers;
        private IResolver _enumerableResolver;
        private IResolver _instanceResolver;

        public Type InstanceType { get; }

        public Type EnumerableType { get; }

        public ResolverBuilder(Type type, IList<IServiceDefinition> definitions, Configuration configuration)
        {
            _definitions = definitions;
            _configuration = configuration;
            if (type.IsEnumerable()) {
                EnumerableType = type;
                InstanceType = type.GenericTypeArguments.Single();
            }
            else {
                EnumerableType = typeof(IEnumerable<>).MakeGenericType(type);
                InstanceType = type;
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
            if (_resolvers != null) return;
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

            if (!_definitions.Any() &&
                !InstanceType.IsAbstract &&
                !InstanceType.IsValueType &&
                !InstanceType.IsGenericTypeDefinition) {
                var definition = new ServiceDefinition(
                    InstanceType,
                    implementationType: InstanceType,
                    lifecycle: Lifecycle.Default
                );
                var resolver = CreateResolverByType(definition, InstanceType);
                if (resolver != null && resolver.CanResolve) {
                    _resolvers.Add(CreateResolverByType(definition, InstanceType));
                }
            }

            var enumerableResolverType = typeof(EnumerableResolver<>).MakeGenericType(InstanceType);

            _enumerableResolver =
                (IResolver)Activator.CreateInstance(enumerableResolverType, _resolvers);
            _instanceResolver = new SingleResolver(_resolvers, InstanceType).ActualResolver;
        }

        private IResolver CreateResolverByType(IServiceDefinition definition, Type implementationType)
        {
            var constructor = _configuration.Options.ConstructorSelector.SelectConstructor
                (implementationType, _configuration);
            if (constructor == null) return new NonResolver(InstanceType);
            var expression =
                _configuration.Options.Compiler.CreateExpressionFromDefaultConstructor(constructor);
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
                expression = _configuration.Options.Compiler
                    .CreateExpressionFromConstructorExpression(nex);
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
             *  2. Service cache resolver
             *  3. Service tracker resolver
             *  4. Decorator resolver
             *  5. Conditional resolver
             *  6. Enumerable resolver / Selector resolver
             */

            if (resolver is ExpressionResolver exr && definition.Lifecycle.Cached) {
                resolver = new ServiceCacheResolver(definition, exr);
            }

            if (isTracked && definition.Lifecycle.Tracked) {
                resolver = new ServiceTrackerResolver(definition, resolver);
            }

            var decoratorType = typeof(IDecorator<>).MakeGenericType(InstanceType);
            if (_configuration.CanResolveNew(decoratorType)) {
                var decoratorResolverType
                    = typeof(DecoratorResolver<>).MakeGenericType(InstanceType);
                resolver = (IResolver)Activator.CreateInstance
                    (decoratorResolverType, resolver, _configuration);
            }

            if (definition.Precondition != null) {
                resolver = new ConditionalResolver(definition, resolver);
            }

            return resolver;
        }
    }
}
