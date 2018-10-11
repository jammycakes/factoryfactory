using Microsoft.Extensions.DependencyInjection;

namespace Nooshka.Modules
{
    public class ServiceCollectionModule : IModule
    {
        private readonly IServiceCollection _serviceCollection;

        public ServiceCollectionModule(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }
    }
}
