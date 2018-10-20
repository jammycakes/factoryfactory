using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;

namespace FactoryFactory.Tests
{
    public class MicrosoftSpecificationFixture : DependencyInjectionSpecificationTests
    {
        protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            return Configuration.CreateContainer(serviceCollection);

        }
    }
}
