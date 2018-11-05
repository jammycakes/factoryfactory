using FactoryFactory.Tests.Model;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Interception
{
    public class DecoratorFixture
    {
        [Fact]
        public void DecoratedServiceShouldBeDecorated()
        {
            var decorator = A.Fake<IDecorator<IServiceWithoutDependencies>>();
            IServiceWithoutDependencies original = null;
            IServiceWithoutDependencies decorated = null;
            A.CallTo(
                () => decorator.Decorate(
                    A<ServiceRequest>.Ignored,
                    A<IServiceWithoutDependencies>.Ignored))
                .ReturnsLazily(
                    (ServiceRequest req, IServiceWithoutDependencies svc) => {
                        original = svc;
                        decorated = A.Fake<IServiceWithoutDependencies>();
                        return decorated;
                    });
            var module = new Module();
            module.Decorate<IServiceWithoutDependencies>().With(decorator);
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
            var decorator = A.Fake<IDecorator<IServiceWithoutDependencies>>();
            A.CallTo(() =>
                decorator.Decorate(
                    A<ServiceRequest>.Ignored,
                    A<IServiceWithoutDependencies>.Ignored))
                .ReturnsLazily((ServiceRequest req, IServiceWithoutDependencies svc) => svc);
            var module = new Module();
            module.Decorate<IServiceWithoutDependencies>().With(decorator);
            module.Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>()
                .Singleton();
            var container = Configuration.CreateContainer(module);
            var service1 = container.GetService<IServiceWithoutDependencies>();
            var service2 = container.GetService<IServiceWithoutDependencies>();
            Assert.Same(service1, service2);
            A.CallTo(() => decorator.Decorate(
                A<ServiceRequest>.Ignored,
                A<IServiceWithoutDependencies>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void TransientShouldBeDecoratedOncePerInstance()
        {
            var decorator = A.Fake<IDecorator<IServiceWithoutDependencies>>();
            A.CallTo(() =>
                    decorator.Decorate(
                        A<ServiceRequest>.Ignored,
                        A<IServiceWithoutDependencies>.Ignored))
                .ReturnsLazily((ServiceRequest req, IServiceWithoutDependencies svc) => svc);
            var module = new Module();
            module.Decorate<IServiceWithoutDependencies>().With(decorator);
            module.Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>()
                .Transient();
            var container = Configuration.CreateContainer(module);
            var service1 = container.GetService<IServiceWithoutDependencies>();
            var service2 = container.GetService<IServiceWithoutDependencies>();
            Assert.NotSame(service1, service2);
            A.CallTo(() => decorator.Decorate(
                A<ServiceRequest>.Ignored,
                A<IServiceWithoutDependencies>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }
    }
}
