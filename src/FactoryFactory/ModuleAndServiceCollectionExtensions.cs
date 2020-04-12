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
        public static IContainer CreateContainer(this IRegistry module)
        {
            return new Configuration(module).CreateContainer();
        }

        /// <summary>
        ///  Creates a FactoryFactory container from this <see cref="IServiceCollection"/>
        ///  instance.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>

        public static IContainer CreateFactoryFactory(this IServiceCollection collection)
        {
            return new Configuration(new Module(collection)).CreateContainer();
        }
    }
}
