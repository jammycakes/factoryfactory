using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public static class ModuleAndServiceCollectionExtensions
    {
        /// <summary>
        ///  Creates a root container from this module.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static IContainer CreateContainer
            (this IRegistry registry, ConfigurationOptions options = default)
        {
            options = options ?? new ConfigurationOptions();
            return new Configuration(options, registry).CreateContainer();
        }

        /// <summary>
        ///  Creates a FactoryFactory container from this <see cref="IServiceCollection"/>
        ///  instance.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>

        public static IContainer CreateFactoryFactory
            (this IServiceCollection collection, ConfigurationOptions options = default)
        {
            IRegistry registry = collection as Registry ?? new Registry(collection);
            return registry.CreateContainer(options);
        }
    }
}
