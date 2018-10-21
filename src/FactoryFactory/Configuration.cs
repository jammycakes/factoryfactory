using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FactoryFactory.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    /// <summary>
    ///  The base configuration class containing the service definitions and
    ///  from which containers are created in the first instance.
    /// </summary>
    public class Configuration
    {
        private List<IModule> _modules = new List<IModule>();
        private ConcurrentDictionary<Type, List<IServiceResolver>> _resolvers
            = new ConcurrentDictionary<Type, List<IServiceResolver>>();

        public ConfigurationOptions Options { get; }

        /* ====== Constructors ====== */

        /// <summary>
        ///  Creates a new <see cref="Configuration"/> instance configured with
        ///  the modules provided and a default set of options.
        /// </summary>
        /// <param name="modules">
        ///  The modules containing service definitions.
        /// </param>
        public Configuration(params IModule[] modules)
        {
            Options = new ConfigurationOptions();
            _modules.Add(new DefaultModule(this));
            _modules.AddRange(modules);
        }

        /// <summary>
        ///  Creates anew <see cref="Configuration"/> instance configured with
        ///  the modules provided and the specified options.
        /// </summary>
        /// <param name="options">
        ///  The core options which control how the IOC container registers and
        ///  creates new services.
        /// </param>
        /// <param name="modules">
        ///  One or more modules containing service definitions.
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
            return Options.AutoResolve && !type.IsValueType && !type.IsAbstract && type.GetConstructors().Any();
        }


        /* ====== Resolver cache ====== */

        /// <summary>
        ///  Gets the resolvers for the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<IServiceResolver> GetResolvers(Type type)
        {
            return _resolvers.GetOrAdd(type, t => {
                var definitions =
                    from module in _modules
                    from definition in module.GetDefinitions(type)
                    select definition;

                if (definitions.Any()) {
                    var builtResolvers =
                        from definition in definitions
                        let builder = Options.Compiler.Build(definition, this)
                        where builder != null
                        select (IServiceResolver)new ServiceResolver
                            (definition, builder);
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
                            var resolver = new ServiceResolver
                                (definition, Options.Compiler.Build(definition, this));
                            result.Add(resolver);
                        }
                    }
                    return result;
                }
            });
        }

        public bool CanResolve(Type type)
        {
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
        public static Container CreateContainer(Action<Module> moduleConfig)
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
