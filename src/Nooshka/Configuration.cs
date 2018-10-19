using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Impl;

namespace Nooshka
{
    /// <summary>
    ///  The base configuration class containing the service registrations and
    ///  from which containers are created in the first instance.
    /// </summary>
    public class Configuration
    {
        private List<IModule> _modules = new List<IModule>();
        private ReaderWriterLockSlim _resolverLock = new ReaderWriterLockSlim();
        private IDictionary<Type, List<IServiceBuilder>> _resolvers
            = new Dictionary<Type, List<IServiceBuilder>>();

        public ConfigurationOptions Options { get; }

        /* ====== Constructors ====== */

        /// <summary>
        ///  Creates a new <see cref="Configuration"/> instance configured with
        ///  the modules provided and a default set of options.
        /// </summary>
        /// <param name="modules">
        ///  The modules containing service registration definitions.
        /// </param>
        public Configuration(params IModule[] modules)
        {
            Options = new ConfigurationOptions();
            _modules.Add(new DefaultModule(this));
            _modules.AddRange(modules);
        }

        /// <summary>
        ///  Creates anew <see cref="Configuration"/> instance configured with
        ///  the modules provided and
        /// </summary>
        /// <param name="options">
        ///  The core options which control how the IOC container registers and
        ///  creates new services.
        /// </param>
        /// <param name="modules">
        ///  One or more modules containing service registration definitions.
        /// </param>
        public Configuration(ConfigurationOptions options, params IModule[] modules)
            : this(modules)
        {
            Options = options;
        }

        /* ====== Methods ====== */

        /// <summary>
        ///  Creates a new <see cref="Container"/> instance.
        /// </summary>
        /// <returns>
        ///  The created container.
        /// </returns>
        public Container CreateContainer()
        {
            return new Container(this);
        }


        private bool CanAutoResolve(Type type)
        {
            return Options.AutoResolve && !type.IsAbstract && type.GetConstructors().Any();
        }


        /* ====== Resolver cache ====== */

        /// <summary>
        ///  Gets the resolvers for the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<IServiceBuilder> GetResolvers(Type type)
        {
            List<IServiceBuilder> result;

            _resolverLock.EnterUpgradeableReadLock();
            try {
                if (!_resolvers.TryGetValue(type, out result)) {
                    _resolverLock.EnterWriteLock();
                    try {
                        if (!_resolvers.TryGetValue(type, out result)) {
                            var registrations =
                                from module in _modules
                                from registration in module.GetRegistrations(type)
                                select registration;
                            if (registrations.Any()) {
                                var builtResolvers =
                                    from registration in registrations
                                    select Options.ResolverCompiler.Build(registration, this);
                                result = builtResolvers.ToList();
                            }
                            else {
                                result = new List<IServiceBuilder>();
                                if (CanAutoResolve(type)) {
                                    var resolver = Options.ResolverCompiler.Build(
                                        new ServiceDefinition(type) {
                                            ImplementationType = type,
                                            Lifecycle = Options.DefaultLifecycle,
                                            Precondition = req => true
                                        },
                                        this
                                    );
                                    result.Add(resolver);
                                }
                            }
                            _resolvers.Add(type, result);
                        }
                    }
                    finally {
                        _resolverLock.ExitWriteLock();
                    }
                }

            }
            finally {
                _resolverLock.ExitUpgradeableReadLock();
            }
            return result;
        }

        public bool CanResolve(Type type)
        {
            var result =
                CanAutoResolve(type) ||
                   _resolvers.ContainsKey(type) && _resolvers[type].Any() ||
                   _modules.Any(m => m.IsTypeRegistered(type));
            return result;
        }


        /* ====== Static convenience methods ====== */

        /// <summary>
        ///  Creates a new container, initialised and configured from the
        ///  provided modules.
        /// </summary>
        /// <param name="modules">
        ///  One or more modules containing service registration definitions.
        /// </param>
        /// <returns>
        ///  The configured container.
        /// </returns>
        public static Container CreateContainer(params IModule[] modules)
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
        public static Container CreateContainer(IServiceCollection services)
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
        public static Container CreateContainer(Action<IModule> moduleConfig)
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
                Define<Configuration>().As(configuration).Untracked();
                Define<Container>().As(req => req.Container).Untracked();
                Define<IServiceProvider>().As(req => req.Container).Untracked();
                Define<IServiceScopeFactory>().As(req => req.Container).Untracked();
            }
        }
    }
}
