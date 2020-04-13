using FactoryFactory.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.LifecycleTests
{
    public class LifecycleFixture
    {
        [Fact]
        public void ServicesRegisteredByInstanceShouldBeUntracked()
        {
            var svc = new DisposableService();

            using (var container = Configuration.CreateContainer
                    (module => module.Define<DisposableService>().As(svc))
            ) {
                var svc2 = container.GetService<DisposableService>();
                Assert.Same(svc, svc2);
            }

            Assert.False(svc.Disposed);
        }

        [Fact]
        public void ServicesRegisteredByInstanceFromServiceCollectionShouldBeUntracked()
        {
            var svc = new DisposableService();

            var services = new Registry();
            services.AddSingleton(svc);

            using (var container = Configuration.CreateContainer(services)) {
                var svc2 = container.GetService<DisposableService>();
                Assert.Same(svc, svc2);
            }

            Assert.False(svc.Disposed);
        }
    }
}
