using System;
using System.Linq.Expressions;
using FactoryFactory.Expressions;
using FactoryFactory.Lifecycles;
using FactoryFactory.Registration.Impl.ServiceDefinitions;
using FactoryFactory.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Resolution.ResolverBuilderTests
{
    public class ExpressionFixture
    {
        ConfigurationOptions _options = new ConfigurationOptions(
            constructorSelector: new DefaultConstructorSelector(),
            expressionBuilder: new ExpressionBuilder()
        );

        [Fact]
        public void CanCreateServiceResolutionExpressionFromDefaultConstructor()
        {
            var definition = new ServiceDefinition(
                typeof(IServiceWithDependencies),
                implementationType: typeof(ServiceWithDependencies)
            );

            var registry = new Registry()
                .Define<IServiceWithDependencies>().As<ServiceWithDependencies>()
                .Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>();
            var configuration = new Configuration(_options, registry);
            var constructor =
                _options.ConstructorSelector.SelectConstructor
                    (definition.ImplementationType, configuration);
            var expression = _options.ExpressionBuilder.CreateResolutionExpressionFromDefaultConstructor(constructor);

            var expressionBody = expression.Body;
            Assert.IsType<NewExpression>(expressionBody);
            Assert.Equal(typeof(ServiceWithDependencies), expressionBody.Type);
        }

        [Fact]
        public void CanCreateServiceResolutionExpressionFromConstructorExpression()
        {
            var container = new Registry()
                .Define<IServiceWithDependencies>()
                .As(req => new ServiceWithDependencies(
                    Resolve.From<IServiceWithoutDependencies>(),
                    "Hello world"
                ))
                .Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>()
                .CreateContainer();
            var service = container.GetService<IServiceWithDependencies>();
            Assert.Equal("Hello world", service.Message);
            Assert.IsType<ServiceWithoutDependencies>(service.Dependency);
        }

        [Fact]
        public void CanCreateServiceResolutionExpressionFromFluentConstructorExpression()
        {
            var registry = new Registry()
                .Define<IServiceWithDependencies>().As(req =>
                new ServiceWithDependencies(
                    Resolve.From<IServiceWithoutDependencies>(),
                    "Hello world"
                ))
                .Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>();

            var configuration = new Configuration(_options, registry);
            var container = configuration.CreateContainer();
            var service = container.GetService<IServiceWithDependencies>();
            Assert.Equal("Hello world", service.Message);
            Assert.IsType<ServiceWithoutDependencies>(service.Dependency);
        }


        [Theory]
        [InlineData(typeof(ScopedLifecycle))]
        [InlineData(typeof(SingletonLifecycle))]
        [InlineData(typeof(TransientLifecycle))]
        [InlineData(typeof(UntrackedLifecycle))]
        public void CanResolveFuncDependencies(Type lifecycleType)
        {
            var lifecycle = Activator.CreateInstance(lifecycleType) as ILifecycle;
            var service = new Registry()
                .Define<IServiceWithDependencies>().Lifecycle(lifecycle).As<ServiceWithFuncDependencies>()
                .Define<IServiceWithoutDependencies>().Lifecycle(lifecycle).As<ServiceWithoutDependencies>()
                .CreateContainer()
                .GetService<IServiceWithDependencies>();
            Assert.IsType<ServiceWithoutDependencies>(service.Dependency);
        }

        [Theory]
        [InlineData(typeof(ScopedLifecycle))]
        [InlineData(typeof(SingletonLifecycle))]
        [InlineData(typeof(TransientLifecycle))]
        [InlineData(typeof(UntrackedLifecycle))]
        public void CanResolveLazyDependencies(Type lifecycleType)
        {
            var lifecycle = Activator.CreateInstance(lifecycleType) as ILifecycle;
            var service = new Registry()
                .Define<IServiceWithDependencies>().Lifecycle(lifecycle).As<ServiceWithLazyDependencies>()
                .Define<IServiceWithoutDependencies>().Lifecycle(lifecycle).As<ServiceWithoutDependencies>()
                .CreateContainer()
                .GetService<IServiceWithDependencies>();
            Assert.IsType<ServiceWithoutDependencies>(service.Dependency);
        }
    }
}
