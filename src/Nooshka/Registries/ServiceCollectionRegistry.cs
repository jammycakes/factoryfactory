using Microsoft.Extensions.DependencyInjection;

namespace Nooshka.Registries
{
    public class ServiceCollectionRegistry : IRegistry
    {
        private readonly IServiceCollection _serviceCollection;

        public ServiceCollectionRegistry(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }
    }
}
