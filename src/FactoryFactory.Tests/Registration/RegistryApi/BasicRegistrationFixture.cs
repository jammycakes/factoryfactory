using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using FactoryFactory.Tests.Model;
using FactoryFactory.Tests.Model.Implementations;
using FactoryFactory.Tests.Model.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Registration.RegistryApi
{
    public class BasicRegistrationFixture
    {
        [Fact]
        public void SingletonShouldResolveOnlyOnce()
        {
            var registry = new Registry();
            registry
                .Define<IServiceWithoutDependencies>().Singleton().As<ServiceWithoutDependencies>()
                .Define<IServiceWithDependencies>().Singleton().As<ServiceWithDependencies>();
            using (var container = registry.CreateContainer()) {
                var foo = container.GetService<IServiceWithDependencies>();
                var foo2 = container.GetService<IServiceWithDependencies>();
                Assert.Same(foo, foo2);
            }
        }

        [Fact]
        public void ScopedShouldResolveOncePerScope()
        {
            var registry = new Registry();
            registry
                .Define<IServiceWithDependencies>().Scoped().As<ServiceWithDependencies>()
                .Define<IServiceWithoutDependencies>().Singleton().As<ServiceWithoutDependencies>();
            using (var container = registry.CreateContainer()) {
                var foo = container.GetService<IServiceWithDependencies>();
                IServiceWithDependencies foo2;
                using (var container2 = container.CreateChild()) {
                    foo2 = container2.GetService<IServiceWithDependencies>();
                    var foo3 = container2.GetService<IServiceWithDependencies>();
                    Assert.Same(foo2, foo3);
                }
                Assert.NotSame(foo, foo2);
            }
        }

        [Fact]
        public void TransientShouldResolveOncePerRequestAndBeDisosed()
        {
            var registry = new Registry();
            registry
                .Define<DisposableService>().Transient().As<DisposableService>();
            DisposableService foo;
            DisposableService foo2;
            using (var container = registry.CreateContainer()) {
                foo = container.GetService<DisposableService>();
                foo2 = container.GetService<DisposableService>();
            }

            Assert.NotSame(foo, foo2);
            Assert.True(foo.Disposed);
            Assert.True(foo2.Disposed);
        }


        [Fact]
        public void UntrackedShouldResolveOncePerRequestAndNotBeDisposed()
        {
            var registry = new Registry();
            registry
                .Define<DisposableService>().Untracked().As<DisposableService>();
            DisposableService foo;
            DisposableService foo2;
            using (var container = registry.CreateContainer()) {
                foo = container.GetService<DisposableService>();
                foo2 = container.GetService<DisposableService>();
            }

            Assert.NotSame(foo, foo2);
            Assert.False(foo.Disposed);
            Assert.False(foo2.Disposed);
        }

        [Fact]
        public void FactoryResolutionShouldResolveCorrectly()
        {
            var source = new Foo();
            var registry = new Registry();
            registry
                .Define<IFoo>().As(req => source);
            using (var container = registry.CreateContainer()) {
                var foo = container.GetService<IFoo>();
                Assert.Same(source, foo);
            }
        }

        [Fact]
        public void InstanceResolutionShouldResolveCorrectly()
        {
            var source = new Foo();
            var registry = new Registry()
                .Define<IFoo>().As(source);
            using (var container = registry.CreateContainer()) {
                var foo = container.GetService<IFoo>();
                Assert.Same(source, foo);
            }
        }

        [Fact]
        public void ConditionalTypeResolutionShouldResolveCorrectly()
        {
            var registry = new Registry();
            registry
                .Define<IFoo>().Precondition(req => false).As<Foo>()
                .Define<IFoo>().Precondition(req => true).As<OtherFoo>();
            using (var container = registry.CreateContainer()) {
                var foos = container.GetServices<IFoo>().ToList();
                Assert.Equal(1, foos.Count);
                Assert.IsType<OtherFoo>(foos.Single());
            }
        }

        [Fact]
        public void ConditionalInstanceResolutionShouldResolveCorrectly()
        {
            var source1 = new Foo();
            var source2 = new OtherFoo();
            var registry = new Registry()
                .Define<IFoo>().Precondition(req => false).As(source1)
                .Define<IFoo>().Precondition(req => true).As(source2);
            using (var container = registry.CreateContainer()) {
                var foo = container.GetServices<IFoo>().Single();
                Assert.Same(source2, foo);
            }
        }

        [Fact]
        public void ConditionalFactoryResolutionShouldResolveCorrectly()
        {
            var registry = new Registry()
                .Define<IFoo>().Precondition(req => false).As(req => new Foo())
                .Define<IFoo>().Precondition(req => true).As(req => new OtherFoo());
            using (var container = registry.CreateContainer()) {
                var foo = container.GetServices<IFoo>().Single();
                Assert.IsType<OtherFoo>(foo);
            }
        }
    }
}
