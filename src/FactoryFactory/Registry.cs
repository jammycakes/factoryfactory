using System.Collections.Generic;
using System.Linq;
using FactoryFactory.Registration.ServiceDefinitions;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public class Registry : List<ServiceDescriptor>, IServiceCollection, IRegistry
    {
        public IEnumerable<IServiceDefinition> GetServiceDefinitions() =>
            this.Select(desc => desc as IServiceDefinition ?? new ServiceDefinition(desc));
    }
}
