using System;
using FactoryFactory.Lifecycles;
using FactoryFactory.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Resolution
{
    public class RecursiveDependencyFixture
    {
        [Theory]
        [InlineData(typeof(ScopedLifecycle), true)]
        [InlineData(typeof(SingletonLifecycle), true)]
        [InlineData(typeof(TransientLifecycle), false)]
        [InlineData(typeof(UntrackedLifecycle), false)]
        public void CanResolveRecursiveLazyDependency(Type lifecycleType, bool shouldBeSame)
        {
            var lifecycle = Activator.CreateInstance(lifecycleType) as ILifecycle;
            var container = new Registry()
                .Define<RecursiveServiceWithLazyDependency>()
                .Lifecycle(lifecycle)
                .As<RecursiveServiceWithLazyDependency>()
                .CreateContainer();
            var service = container.GetService<RecursiveServiceWithLazyDependency>();
            Assert.NotNull(service);
            if (shouldBeSame) {
                Assert.Same(service, service.Dependency);
            }
            else {
                Assert.NotSame(service, service.Dependency);
            }
        }

        [Theory]
        [InlineData(typeof(ScopedLifecycle), true)]
        [InlineData(typeof(SingletonLifecycle), true)]
        [InlineData(typeof(TransientLifecycle), false)]
        [InlineData(typeof(UntrackedLifecycle), false)]
        public void CanResolveRecursiveFuncDependency(Type lifecycleType, bool shouldBeSame)
        {
            var lifecycle = Activator.CreateInstance(lifecycleType) as ILifecycle;
            var container = new Registry()
                .Define<RecursiveServiceWithFuncDependency>()
                .Lifecycle(lifecycle)
                .As<RecursiveServiceWithFuncDependency>()
                .CreateContainer();
            var service = container.GetService<RecursiveServiceWithFuncDependency>();
            Assert.NotNull(service);
            if (shouldBeSame) {
                Assert.Same(service, service.Dependency);
            }
            else {
                Assert.NotSame(service, service.Dependency);
            }
        }
    }
}
