using FactoryFactory.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Resolution.ResolutionTests
{
    public class RecursiveDependencyFixture
    {
        [Fact]
        public void CanResolveRecursiveLazyDependencyAsScoped()
        {
            var container = Configuration.CreateContainer(module => {
                module.Define<RecursiveServiceWithLazyDependency>().As<RecursiveServiceWithLazyDependency>().Scoped();
            });
            var service = container.GetService<RecursiveServiceWithLazyDependency>();
            Assert.NotNull(service);
            Assert.Same(service, service.Dependency);
        }

        [Fact]
        public void CanResolveRecursiveLazyDependencyAsTransient()
        {
            var container = Configuration.CreateContainer(module => {
                module.Define<RecursiveServiceWithLazyDependency>().As<RecursiveServiceWithLazyDependency>().Transient();
            });
            var service = container.GetService<RecursiveServiceWithLazyDependency>();
            Assert.NotNull(service);
            Assert.NotSame(service, service.Dependency);
        }

        [Fact]
        public void CanResolveRecursiveFuncDependencyAsScoped()
        {
            var container = Configuration.CreateContainer(module => {
                module.Define<RecursiveServiceWithFuncDependency>().As<RecursiveServiceWithFuncDependency>().Scoped();
            });
            var service = container.GetService<RecursiveServiceWithFuncDependency>();
            Assert.NotNull(service);
            Assert.Same(service, service.Dependency);
        }

        [Fact]
        public void CanResolveRecursiveFuncDependencyAsTransient()
        {
            var container = Configuration.CreateContainer(module => {
                module.Define<RecursiveServiceWithFuncDependency>().As<RecursiveServiceWithFuncDependency>().Transient();
            });
            var service = container.GetService<RecursiveServiceWithFuncDependency>();
            Assert.NotNull(service);
            Assert.NotSame(service, service.Dependency);
        }
    }
}
