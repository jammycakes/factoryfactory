#define NEW_CONTAINERS

using System;
using System.Collections.Generic;
using FactoryFactory.Impl;
using FactoryFactory.Registration;
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

        /// <summary>
        ///  Gets the <see cref="ConfigurationOptions"/> instance containing
        ///  options which have been specified for this configuration.
        /// </summary>
        public ConfigurationOptions Options { get; }

        /* ====== Constructors ====== */

        /// <summary>
        ///  Creates a new <see cref="Configuration"/> instance configured with
        ///  the modules provided and a default set of options.
        /// </summary>
        /// <param name="modules">
        ///  The modules containing service definitions.
        /// </param>
        public Configuration(params Module[] modules)
        {
            Options = new ConfigurationOptions();
            AddModule(new DefaultModule(this));
            foreach (var module in modules) {
                AddModule(module);
            }
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
        public Configuration(ConfigurationOptions options, params Module[] modules)
            : this(modules)
        {
            Options = options;
        }


        /* ====== Public API: static convenience methods ====== */

        /// <summary>
        ///  Creates a new container, initialised and configured from the
        ///  provided modules.
        /// </summary>
        /// <param name="modules">
        ///  One or more modules containing service definitions.
        /// </param>
        /// <returns>
        ///  The configured container.
        /// </returns>
        public static IContainer CreateContainer(params Module[] modules)
        {
            return new Configuration(modules).CreateContainer();
        }

        /// <summary>
        ///  Creates a new container, initialised and configured from the
        ///  provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        ///  The services being registered.
        /// </param>
        /// <returns>
        ///  The configured container.
        /// </returns>
        public static IContainer CreateContainer(IServiceCollection services)
        {
            return CreateContainer(new Module(services));
        }

        /// <summary>
        ///  Creates a new container from a new module, initialised and
        ///  configured by the supplied action.
        /// </summary>
        /// <param name="moduleConfig">
        ///  A lambda function that initialises the module.
        /// </param>
        /// <returns>
        ///  The configured container.
        /// </returns>
        public static IContainer CreateContainer(Action<Module> moduleConfig)
        {
            var module = new Module();
            moduleConfig(module);
            return CreateContainer(module);
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

        private void AddModule(Module module)
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


        /* ====== DefaultModule ====== */

        /// <summary>
        ///  Contains core default definitions registered by the IOC container.
        /// </summary>
        private class DefaultModule : Module
        {
            public DefaultModule(Configuration configuration)
            {
                Add(new FuncServiceDefinition());
                Add(new ArrayServiceDefinition());
                Define(typeof(Lazy<>)).As(typeof(Lazy<>)).Untracked();
                Define(typeof(ICollection<>)).As(typeof(List<>)).Untracked();
                Define(typeof(IReadOnlyCollection<>)).As(typeof(List<>)).Untracked();
                Define(typeof(IList<>)).As(typeof(List<>)).Untracked();
                Define(typeof(List<>)).As(typeof(List<>)).Untracked();
                Define(typeof(ISet<>)).As(typeof(HashSet<>)).Untracked();
                Define(typeof(HashSet<>)).As(typeof(HashSet<>)).Untracked();
                Define<Configuration>().As(configuration).Untracked();
                Define<IContainer>().As(req => req.Container).Untracked();
                Define<IServiceScope>().As(req => req.Container).Untracked();
                Define<IServiceProvider>().As(req => req.Container).Untracked();
                Define<IServiceScopeFactory>().As(req => req.Container).Untracked();
            }
        }
    }
}
