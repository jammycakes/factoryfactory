using FactoryFactory.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Resolution.ResolutionTests
{
    public class DecorationFixture
    {
        [Fact]
        public void DecorationCanReplaceService()
        {
            var container = Configuration.CreateContainer(module => {
                module.Define<IServiceWithoutDependencies>()
                    .As<ServiceWithoutDependencies>()
                    .Proxy((req, svc) => new AlternateServiceWithoutDependencies());
            });

            var service = container.GetService<IServiceWithoutDependencies>();
            Assert.IsType<AlternateServiceWithoutDependencies>(service);
        }
    }
}
