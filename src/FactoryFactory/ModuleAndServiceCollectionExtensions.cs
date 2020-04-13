using System;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public static class ModuleAndServiceCollectionExtensions
    {
        /// <summary>
        ///  Creates a root container from the current <see cref="IRegistry"/> instance.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="options">
        ///  An optional <see cref="ConfigurationOptions"/> instance to be used
        ///  to configure the container.
        /// </param>
        /// <returns>
        ///  The root container.
        /// </returns>
        public static IContainer CreateContainer
            (this IRegistry registry, ConfigurationOptions options = default)
        {
            options = options ?? new ConfigurationOptions();
            return new Configuration(options, registry).CreateContainer();
        }

        /// <summary>
        ///  Creates a root container from the curremmt <see cref="IRegistry"/> instance.
        /// </summary>
        /// <param name="registry">The registry instance containing our type definitions.</param>
        /// <param name="configure">
        ///  An action to configure the container.
        /// </param>
        /// <returns>
        ///  The root container.
        /// </returns>
        public static IContainer CreateContainer
            (this IRegistry registry, Action<ConfigurationOptions> configure)
        {
            var options = new ConfigurationOptions();
            configure(options);
            return registry.CreateContainer(options);
        }

        /// <summary>
        ///  Creates a FactoryFactory container from this <see cref="IServiceCollection"/>
        ///  instance.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>
        ///  The root container.
        /// </returns>
        public static IContainer CreateFactoryFactory
            (this IServiceCollection collection, ConfigurationOptions options = default)
        {
            IRegistry registry = collection as Registry ?? new Registry(collection);
            return registry.CreateContainer(options);
        }

        /// <summary>
        ///  Creates a FactoryFactory root container from this <see cref="IServiceCollection" />
        ///  instance.
        /// </summary>
        /// <param name="collection">
        ///  The <see cref="IServiceCollection"/> instance containing our type
        ///  definitions.
        /// </param>
        /// <param name="configure">
        ///  An action to configure the container options.
        /// </param>
        /// <returns>
        ///  The root container.
        /// </returns>
        public static IContainer CreateFactoryFactory
            (this IServiceCollection collection, Action<ConfigurationOptions> configure)
        {
            var options = new ConfigurationOptions();
            configure(options);
            return collection.CreateFactoryFactory(options);
        }
    }
}
