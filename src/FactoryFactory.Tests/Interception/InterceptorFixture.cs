using System;
using FactoryFactory.Tests.Model;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Interception
{
    public class InterceptorFixture
    {
        [Fact]
        public void DecoratedServiceShouldBeDecorated()
        {
            var interceptor = A.Fake<IInterceptor<IServiceWithoutDependencies>>();
            IServiceWithoutDependencies original = null;
            IServiceWithoutDependencies decorated = null;
            A.CallTo(
                () => interceptor.Intercept(
                    A<ServiceRequest>.Ignored,
                    A<Func<IServiceWithoutDependencies>>.Ignored))
                .ReturnsLazily(
                    (ServiceRequest req, Func<IServiceWithoutDependencies> svc) => {
                        original = svc();
                        decorated = A.Fake<IServiceWithoutDependencies>();
                        return decorated;
                    });
            var module = new Module();
            module.Intercept<IServiceWithoutDependencies>().With(interceptor);
            module.Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>()
                .Singleton();
            var container = Configuration.CreateContainer(module);
            var service = container.GetService<IServiceWithoutDependencies>();
            Assert.Same(decorated, service);
            Assert.IsType<ServiceWithoutDependencies>(original);
        }

        [Fact]
        public void SingletonShouldBeDecoratedOnlyOnce()
        {
            var interceptor = A.Fake<IInterceptor<IServiceWithoutDependencies>>();
            A.CallTo(() =>
                interceptor.Intercept(
                    A<ServiceRequest>.Ignored,
                    A<Func<IServiceWithoutDependencies>>.Ignored))
                .ReturnsLazily((ServiceRequest req, Func<IServiceWithoutDependencies> svc) => svc());
            var module = new Module();
            module.Intercept<IServiceWithoutDependencies>().With(interceptor);
            module.Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>()
                .Singleton();
            var container = Configuration.CreateContainer(module);
            var service1 = container.GetService<IServiceWithoutDependencies>();
            var service2 = container.GetService<IServiceWithoutDependencies>();
            Assert.Same(service1, service2);
            A.CallTo(() => interceptor.Intercept(
                A<ServiceRequest>.Ignored,
                A<Func<IServiceWithoutDependencies>>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void TransientShouldBeDecoratedOncePerInstance()
        {
            var interceptor = A.Fake<IInterceptor<IServiceWithoutDependencies>>();
            A.CallTo(() =>
                    interceptor.Intercept(
                        A<ServiceRequest>.Ignored,
                        A<Func<IServiceWithoutDependencies>>.Ignored))
                .ReturnsLazily((ServiceRequest req, Func<IServiceWithoutDependencies> svc) => svc());
            var module = new Module();
            module.Intercept<IServiceWithoutDependencies>().With(interceptor);
            module.Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>()
                .Transient();
            var container = Configuration.CreateContainer(module);
            var service1 = container.GetService<IServiceWithoutDependencies>();
            var service2 = container.GetService<IServiceWithoutDependencies>();
            Assert.NotSame(service1, service2);
            A.CallTo(() => interceptor.Intercept(
                A<ServiceRequest>.Ignored,
                A<Func<IServiceWithoutDependencies>>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }
    }
}
