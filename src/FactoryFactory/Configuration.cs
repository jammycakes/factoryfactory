using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FactoryFactory.Impl;
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
        private readonly DictionaryOfLists<Type, ServiceDefinition> _definitionsByType
            = new DictionaryOfLists<Type, ServiceDefinition>();
        private readonly DictionaryOfLists<Type, IServiceResolver> _resolvers
            = new DictionaryOfLists<Type, IServiceResolver>();
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly ISet<Type> _resolversBeingBuilt = new HashSet<Type>();

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

        /* ====== Methods ====== */

        private Type GetKey(Type type)
        {
            if (!type.IsGenericType) return type;
            if (type.IsGenericTypeDefinition) return type;
            return type.GetGenericTypeDefinition();
        }

        public IEnumerable<ServiceDefinition> GetDefinitions(Type type)
        {
            var key = GetKey(type);
            if (_definitionsByType.TryGetValue(key, out var definitions)) {
                return
                    from definition in definitions
                    where definition.ServiceType.IsGenericTypeDefinition
                          || definition.ServiceType == type
                    select definition.GetGenericDefinition(type);
            }
            else {
                return Enumerable.Empty<ServiceDefinition>();
            }
        }


        public bool IsTypeRegistered(Type type)
            => _definitionsByType.ContainsKey(GetKey(type));


        private void AddServiceDefinitions(IEnumerable<ServiceDefinition> defs)
        {
            foreach (var def in defs) {
                var key = GetKey(def.ServiceType);
                _definitionsByType.AddOne(key, def);
            }
        }

        private void AddModule(Module module)
        {
            AddServiceDefinitions(module.GetServiceDefinitions());
        }

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


        private bool CanAutoResolve(Type type)
        {
            return Options.AutoResolve && !type.IsValueType && !type.IsAbstract && type.GetConstructors().Any();
        }


        /* ====== Resolver cache ====== */

        private IServiceResolver CreateServiceResolver(ServiceDefinition definition, IServiceBuilder builder)
        {
            var resolverType = typeof(ServiceResolver<>).MakeGenericType(definition.ServiceType);
            return (IServiceResolver)Activator.CreateInstance(resolverType, definition, builder);
        }


        /// <summary>
        ///  Gets the resolvers for the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<IServiceResolver> GetResolvers(Type type)
        {
            _lock.EnterUpgradeableReadLock();
            try {
                if (!_resolvers.TryGetValue(type, out var result)) {
                    _lock.EnterWriteLock();
                    try {
                        if (!_resolvers.ContainsKey(type)) {
                            result = CreateResolvers(type);
                            _resolvers.Add(type, result);
                        }
                    }
                    finally {
                        _lock.ExitWriteLock();
                    }
                }
                return result;
            }
            finally {
                _lock.ExitUpgradeableReadLock();
            }
        }

        private IList<IServiceResolver> CreateResolvers(Type type)
        {
            _resolversBeingBuilt.Add(type);
            try {
                var definitions = GetDefinitions(type);

                if (definitions.Any()) {
                    var builtResolvers =
                        from definition in definitions
                        let builder = Options.Compiler.Build(definition, this)
                        where builder != null
                        select CreateServiceResolver(definition, builder);
                    return builtResolvers.ToList();
                }
                else {
                    var result = new List<IServiceResolver>();
                    if (CanAutoResolve(type)) {
                        var definition = new ServiceDefinition(type,
                            implementationType: type,
                            lifecycle: Options.DefaultLifecycle);
                        var builder = Options.Compiler.Build(definition, this);
                        if (builder != null) {
                            var resolver = CreateServiceResolver(definition, Options.Compiler.Build(definition, this));
                            result.Add(resolver);
                        }
                    }

                    return result;
                }
            }
            finally {
                _resolversBeingBuilt.Remove(type);
            }
        }

        public bool CanResolve(Type type)
        {
            if (_lock.IsWriteLockHeld && _resolversBeingBuilt.Contains(type)) {
                return true;
            }
            return GetResolvers(type).Any();
        }


        /* ====== Static convenience methods ====== */

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

        /* ====== DefaultModule ====== */

        /// <summary>
        ///  Contains core default definitions registered by the IOC container.
        /// </summary>
        private class DefaultModule : Module
        {
            public DefaultModule(Configuration configuration)
            {
                Add(new FuncServiceDefinition());
                Define(typeof(ICollection<>)).As(typeof(List<>)).Untracked();
                Define(typeof(IReadOnlyCollection<>)).As(typeof(List<>)).Untracked();
                Define(typeof(IList<>)).As(typeof(List<>)).Untracked();
                Define<Configuration>().As(configuration).Untracked();
                Define<IContainer>().As(req => req.Container).Untracked();
                Define<IServiceProvider>().As(req => req.Container).Untracked();
                Define<IServiceScopeFactory>().As(req => req.Container).Untracked();
            }
        }
    }
}
