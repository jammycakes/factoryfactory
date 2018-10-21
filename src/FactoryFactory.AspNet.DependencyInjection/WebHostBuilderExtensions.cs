using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.AspNet.DependencyInjection
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseFactoryFactory
            (this IWebHostBuilder builder, ConfigurationOptions options, params Module[] modules)
        {
            return builder.ConfigureServices
                (services => services.AddFactoryFactory(options, modules));
        }

        public static IWebHostBuilder UseFactoryFactory(this IWebHostBuilder builder, params Module[] modules)
        {
            return builder.ConfigureServices
                (services => services.AddFactoryFactory(new ConfigurationOptions(), modules));
        }

        public static IServiceCollection AddFactoryFactory
            (this IServiceCollection collection, ConfigurationOptions options, params Module[] modules)
        {
            collection.AddSingleton<IServiceProviderFactory<Module>>
                (sp => new ServiceProviderFactory(options, modules));
            collection.AddSingleton<IServiceProviderFactory<IServiceCollection>>
                (sp => new ServiceProviderFactory(options, modules));
            return collection;
        }

        public static IServiceCollection AddFactoryFactory
            (this IServiceCollection collection, params Module[] modules)
        {
            return collection.AddFactoryFactory(new ConfigurationOptions(), modules);
        }
    }
}
