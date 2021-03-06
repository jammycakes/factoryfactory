#define NEW_CONTAINERS

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FactoryFactory.Expressions;
using FactoryFactory.Impl;
using FactoryFactory.Registration.Impl.ServiceDefinitions;
using FactoryFactory.Resolution;
using FactoryFactory.Util;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    /// <summary>
    ///  The base configuration class containing the service definitions and
    ///  from which containers are created in the first instance.
    /// </summary>
    public class Configuration
    {
        private readonly ISet<Type> _resolversBeingBuilt = new HashSet<Type>();
        private IList<IServiceDefinition> _definitions = new List<IServiceDefinition>();
        private IDictionary<Type, IResolver> _resolverCache = new Dictionary<Type, IResolver>();

        /* ====== Public properties ====== */

        public bool AutoResolve { get; }

        public IConstructorSelector ConstructorSelector { get; }

        public ILifecycle DefaultLifecycle { get; }

        public IExpressionBuilder ExpressionBuilder { get; }


        /* ====== Constructors ====== */

        private Configuration(ConfigurationOptions options)
        {
            options = options ?? new ConfigurationOptions();
            this.AutoResolve = options.AutoResolve;
            this.ConstructorSelector = options.ConstructorSelector;
            this.DefaultLifecycle = options.DefaultLifecycle;
            this.ExpressionBuilder = options.ExpressionBuilder;
        }


        /// <summary>
        ///  Creates a new <see cref="Configuration"/> instance configured with
        ///  the modules provided and a default set of options.
        /// </summary>
        /// <param name="modules">
        ///  The modules containing service definitions.
        /// </param>
        public Configuration(params IRegistry[] modules)
            : this(new ConfigurationOptions(), modules)
        {
        }

        /// <summary>
        ///  Creates a new <see cref="Configuration"/> instance configured with
        ///  the modules provided and the specified options.
        /// </summary>
        /// <param name="options">
        ///  The core options which control how the IOC container registers and
        ///  creates new services.
        /// </param>
        /// <param name="modules">
        ///  One or more modules containing service definitions.
        /// </param>
        public Configuration(ConfigurationOptions options, params IRegistry[] modules)
            : this(options)
        {
            AddModule(new DefaultDefinitions(this));
            foreach (var module in modules) {
                AddModule(module);
            }
        }


        /* ====== Public API: Instance methods ====== */

        /// <summary>
        ///  Creates a new <see cref="Container"/> instance.
        /// </summary>
        /// <returns>
        ///  The created container.
        /// </returns>
        public IContainer CreateContainer()
        {
            return new Container(this);
        }

        /// <summary>
        ///  Gets an <see cref="IResolver"/> instance which can service a request
        ///  for the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IResolver GetResolver(Type type)
        {
            if (!_resolverCache.TryGetValue(type, out var resolver)) {
                lock (_resolverCache) {
                    resolver = EnsureResolver(type);
                }
            }

            return resolver;
        }


        /* ====== Internal and private methods ====== */

        private void AddServiceDefinitions(IEnumerable<IServiceDefinition> defs)
        {
            foreach (var def in defs) {
                _definitions.Add(def);
            }
        }

        private void AddModule(IRegistry module)
        {
            AddServiceDefinitions(module.GetServiceDefinitions());
        }


        private IResolver EnsureResolver(Type type)
        {
            if (_resolverCache.TryGetValue(type, out IResolver resolver)) {
                return resolver;
            }

            IResolverBuilder builder = new ResolverBuilder(type, _definitions, this);
            if (_resolversBeingBuilt.Contains(builder.InstanceType)) return null;
            _resolversBeingBuilt.Add(builder.InstanceType);
            try {
                var enumerable = builder.GetEnumerableResolver();
                if (builder.EnumerableType != null) {
                    _resolverCache[builder.EnumerableType] = enumerable;
                }
                var instance = builder.GetInstanceResolver();
                if (builder.InstanceType != null) {
                    _resolverCache[builder.InstanceType] = instance;
                }
                return type.IsEnumerable() ? enumerable : instance;
            }
            finally {
                _resolversBeingBuilt.Remove(builder.InstanceType);
            }
        }

        internal bool CanResolve(Type type)
        {
            if (_resolversBeingBuilt.Contains(type)) return true;
            if (type.IsEnumerable()) return true;
            if (type.IsValueType) return false;
            var resolver = EnsureResolver(type);
            return resolver.CanResolve;
        }


        /* ====== DefaultDefinitions ====== */

        /// <summary>
        ///  Contains core default definitions registered by the IOC container.
        /// </summary>
        private class DefaultDefinitions : IRegistry
        {
            private readonly Configuration _configuration;

            private ServiceDefinition CreateDefinition(Type serviceType, Type implementationType)
                => new ServiceDefinition
                    (serviceType, implementationType: implementationType, lifecycle: Lifecycle.Untracked);

            private ServiceDefinition CreateDefinition(Type serviceType, object instance)
                => new ServiceDefinition
                    (serviceType, implementationInstance: instance, lifecycle: Lifecycle.Untracked);

            private ServiceDefinition CreateDefinition
                (Type serviceType, Expression<Func<ServiceRequest, object>> func)
                => new ServiceDefinition
                    (serviceType, implementationFactory: func, lifecycle: Lifecycle.Untracked);

            public IEnumerable<IServiceDefinition> GetServiceDefinitions()
            {
                return new IServiceDefinition[] {
                    new FuncServiceDefinition(),
                    new ArrayServiceDefinition(),
                    CreateDefinition(typeof(Lazy<>), typeof(Lazy<>)),
                    CreateDefinition(typeof(ICollection<>), typeof(List<>)),
                    CreateDefinition(typeof(IReadOnlyCollection<>), typeof(List<>)),
                    CreateDefinition(typeof(IList<>), typeof(List<>)),
                    CreateDefinition(typeof(List<>), typeof(List<>)),
                    CreateDefinition(typeof(ISet<>), typeof(HashSet<>)),
                    CreateDefinition(typeof(HashSet<>), typeof(HashSet<>)),
                    CreateDefinition(typeof(Configuration), _configuration),
                    CreateDefinition(typeof(IContainer), req => req.Container),
                    CreateDefinition(typeof(IServiceScope), req => req.Container),
                    CreateDefinition(typeof(IServiceProvider), req => req.Container),
                    CreateDefinition(typeof(IServiceScopeFactory), req => req.Container)
                };
            }

            public DefaultDefinitions(Configuration configuration)
            {
                _configuration = configuration;
            }
        }
    }
}
