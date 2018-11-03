#define USE_OWN_TESTS

using System;
using Microsoft.Extensions.DependencyInjection;
#if USE_OWN_TESTS
using FactoryFactory.Tests.MicrosoftTests;
#else
using Microsoft.Extensions.DependencyInjection.Specification;
#endif

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
