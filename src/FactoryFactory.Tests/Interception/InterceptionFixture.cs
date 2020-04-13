using FactoryFactory.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Interception
{
    public class InterceptionFixture
    {
        [Fact]
        public void DecorationCanReplaceService()
        {
            var registry = new Registry()
                .Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>()
                .Intercept<IServiceWithoutDependencies>()
                .By((req, svc) => new AlternateServiceWithoutDependencies());

            using (var container = registry.CreateContainer()) {
                var service = container.GetService<IServiceWithoutDependencies>();
                Assert.IsType<AlternateServiceWithoutDependencies>(service);
            }
        }
    }
}
