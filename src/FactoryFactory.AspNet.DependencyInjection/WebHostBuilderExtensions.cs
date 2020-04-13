using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FactoryFactory.AspNet.DependencyInjection
{
    public static class WebHostBuilderExtensions
    {
        public static IHostBuilder UseFactoryFactory
            (this IHostBuilder hostBuilder, Action<ConfigurationOptions> configure = null)
        {
            var options = new ConfigurationOptions();
            configure?.Invoke(options);
            var serviceProviderFactory = new ServiceProviderFactory(options);
            return hostBuilder
                .UseServiceProviderFactory<Registry>(serviceProviderFactory)
                .UseServiceProviderFactory<IServiceCollection>(serviceProviderFactory);
        }

        public static IWebHostBuilder UseFactoryFactory
            (this IWebHostBuilder webHostBuilder, Action<ConfigurationOptions> configure = null)
        {
            var options = new ConfigurationOptions();
            configure?.Invoke(options);
            var serviceProviderFactory = new ServiceProviderFactory(options);
            return webHostBuilder.ConfigureServices(services => 
                services
                    .AddSingleton<IServiceProviderFactory<Registry>>(serviceProviderFactory)
                    .AddSingleton<IServiceProviderFactory<IServiceCollection>>(serviceProviderFactory)
            );
        }
    }
}
