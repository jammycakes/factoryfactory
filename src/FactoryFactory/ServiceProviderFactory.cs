using System;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public class ServiceProviderFactory :
        IServiceProviderFactory<Registry>,
        IServiceProviderFactory<IServiceCollection>
    {
        private readonly ConfigurationOptions _options;

        public ServiceProviderFactory(ConfigurationOptions options = default)
        {
            _options = options;
        }

            Registry IServiceProviderFactory<Registry>.CreateBuilder(IServiceCollection services)
        {
            return new Registry(services);
        }

        IServiceProvider IServiceProviderFactory<Registry>.CreateServiceProvider(Registry containerBuilder)
        {
            return containerBuilder.CreateContainer(_options);
        }

        IServiceCollection IServiceProviderFactory<IServiceCollection>.CreateBuilder(IServiceCollection services)
        {
            return new Registry(services);
        }

        IServiceProvider IServiceProviderFactory<IServiceCollection>.CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return containerBuilder.CreateFactoryFactory(_options);
        }
    }
}
