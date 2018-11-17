using FactoryFactory.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Registration
{
    public class ConventionFixture
    {
        [Fact]
        public void ConventionsByNameWillResolve()
        {
            var container = Configuration.CreateContainer(module => {
                module
                    .Define(types => types.Where(t => t.IsInterface).Where(t => t.Name.StartsWith("I")))
                    .As(types => types.Named(t => t.Name.Substring(1)));
            });
            var service = container.GetService<IServiceWithDependencies>();
            // These should have been registered by the convention.
            Assert.IsType<ServiceWithDependencies>(service);
            Assert.IsType<ServiceWithoutDependencies>(service.Dependency);
        }
    }
}
