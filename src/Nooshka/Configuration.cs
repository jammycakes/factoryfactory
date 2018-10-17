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


        /* ====== Resolver cache ====== */

        /// <summary>
        ///  Gets the resolvers for the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<IServiceBuilder> GetResolvers(Type type)
        {
            _resolverLock.EnterUpgradeableReadLock();
            try {
                List<IServiceBuilder> result;
                if (!_resolvers.TryGetValue(type, out result)) {
                    _resolverLock.EnterWriteLock();
                    try {
                        if (!_resolvers.TryGetValue(type, out result)) {
                            var builtResolvers =
                                from module in _modules
                                from registration in module.GetRegistrations(type)
                                select Options.ResolverCompiler.Build(registration, this);
                            result = builtResolvers.ToList();
                            _resolvers.Add(type, result);
                        }
                    }
                    finally {
                        _resolverLock.ExitWriteLock();
                    }
                }

                return result;
            }
            finally {
                _resolverLock.ExitUpgradeableReadLock();
            }
        }

        public bool IsTypeRegistered(Type type)
            => _modules.Any(m => m.IsTypeRegistered(type));

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
                Resolve<Configuration>().With(configuration).Untracked();
                Resolve<Container>().From(req => req.Container).Untracked();
                Resolve<IServiceProvider>().From(req => req.Container).Untracked();
                Resolve<IServiceScopeFactory>().From(req => req.Container).Untracked();
            }
        }
    }
}
