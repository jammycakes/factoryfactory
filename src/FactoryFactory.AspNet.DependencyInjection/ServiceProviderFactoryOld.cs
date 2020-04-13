using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.AspNet.DependencyInjection
{
    public class ServiceProviderFactoryOld : IServiceProviderFactory<Module>, IServiceProviderFactory<IServiceCollection>
    {
        private readonly ConfigurationOptions _options;
        private readonly Module[] _modules;

        public ServiceProviderFactoryOld(ConfigurationOptions options, params Module[] modules)
        {
            _options = options ?? new ConfigurationOptions();
            _modules = modules;
        }

        public Module CreateBuilder(IServiceCollection services)
        {
            return new Module(services);
        }

        public IServiceProvider CreateServiceProvider(Module containerBuilder)
        {
            return new Configuration(_options, _modules.Append(containerBuilder).ToArray()).CreateContainer();
        }

        IServiceCollection IServiceProviderFactory<IServiceCollection>.CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        IServiceProvider IServiceProviderFactory<IServiceCollection>.CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return CreateServiceProvider(new Module(containerBuilder));
        }
    }
}
