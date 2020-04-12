using System;
using System.Linq.Expressions;
using FactoryFactory.Expressions;
using FactoryFactory.Lifecycles;
using FactoryFactory.Registration;
using FactoryFactory.Registration.ServiceDefinitions;
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
            var module = new Module();
            var definition = new ServiceDefinition(
                typeof(IServiceWithDependencies),
                implementationType: typeof(ServiceWithDependencies)
            );
            module.Add(definition);
            module.Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>();
            var configuration = new Configuration(_options, module);
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
            var module = new Module();
            var definition = new ServiceDefinition(
                typeof(IServiceWithDependencies),
                implementationFactory: req => new ServiceWithDependencies(
                    Resolve.From<IServiceWithoutDependencies>(),
                    "Hello world"
                )
            );

            module.Add(definition);
            module.Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>();
            var configuration = new Configuration(_options, module);
            var container = configuration.CreateContainer();
            var service = container.GetService<IServiceWithDependencies>();
            Assert.Equal("Hello world", service.Message);
            Assert.IsType<ServiceWithoutDependencies>(service.Dependency);
        }

        [Fact]
        public void CanCreateServiceResolutionExpressionFromFluentConstructorExpression()
        {
            var module = new Module();
            module.Define<IServiceWithDependencies>().As(req =>
                new ServiceWithDependencies(
                    Resolve.From<IServiceWithoutDependencies>(),
                    "Hello world"
                )
            );
            module.Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>();

            var configuration = new Configuration(_options, module);
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
            var module = new Module();
            module.Define<IServiceWithDependencies>().As<ServiceWithFuncDependencies>().Lifecycle(lifecycle);
            module.Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>().Lifecycle(lifecycle);
            var service = Configuration.CreateContainer(module)
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
            var module = new Module();
            module.Define<IServiceWithDependencies>().As<ServiceWithLazyDependencies>().Lifecycle(lifecycle);
            module.Define<IServiceWithoutDependencies>().As<ServiceWithoutDependencies>().Lifecycle(lifecycle);
            var container = Configuration.CreateContainer(module);
            var service = container.GetService<IServiceWithDependencies>();
            Assert.IsType<ServiceWithoutDependencies>(service.Dependency);
        }
    }
}
