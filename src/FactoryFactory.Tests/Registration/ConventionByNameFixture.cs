using System;
using System.Reflection;
using FactoryFactory.Tests.Model;
using FactoryFactory.Tests.Model.Implementations;
using FactoryFactory.Tests.Model.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Registration
{
    public class ConventionByNameFixture
    {
        [Fact]
        public void ConventionsByNameWillResolve()
        {
            var container = Configuration.CreateContainer(module => {
                module
                    .Define(types => types
                        .Where(t => t.IsInterface)
                        .Where(t => t.Name.StartsWith("I")))
                    .As(types => types
                        .Named(t => t.Name.Substring(1)));
            });
            var service = container.GetService<IServiceWithDependencies>();
            // These should have been registered by the convention.
            Assert.IsType<ServiceWithDependencies>(service);
            Assert.IsType<ServiceWithoutDependencies>(service.Dependency);
        }

        [Fact]
        public void ConventionsByNameWillResolveFromSpecifiedNamespace()
        {
            var container = Configuration.CreateContainer(module => {
                module
                    .Define(types =>
                        types.Where(t => t.IsInterface).Where(t => t.Name.StartsWith("I")))
                    .As(types => types
                        .Named(t => t.Name.Substring(1))
                        .Named(t => "Other" + t.Name.Substring(1))
                        .FromNamespace(t => typeof(Foo).Namespace)
                    );
            });
            var service = container.GetService<IFoo[]>();
            Assert.Equal(2, service.Length);
            Assert.IsType<Foo>(service[0]);
            Assert.IsType<OtherFoo>(service[1]);
        }

        [Fact]
        public void ConventionsByNameWillResolveWithSpecifiedConditions()
        {
            var container = Configuration.CreateContainer(module => {
                module
                    .Define(types =>
                        types.Where(t => t.IsInterface).Where(t => t.Name.StartsWith("I")))
                    .As(types => types
                        .Named(t => t.Name.Substring(1))
                        .Named(t => "Other" + t.Name.Substring(1))
                        .FromNamespace(t => typeof(Foo).Namespace)
                        .Where((req, impl) => impl.GetCustomAttribute<SerializableAttribute>() != null)
                    );
            });
            var service = container.GetService<IFoo[]>();
            Assert.Single(service);
            Assert.IsType<Foo>(service[0]);
        }

        [Fact]
        public void ConventionsByNameWillResolveFromSpecifiedAssembly()
        {
            var container = Configuration.CreateContainer(module => {
                module
                    .Define(types =>
                        types.Where(t => t.IsInterface).Where(t => t.Name.StartsWith("I")))
                    .As(types => types
                        .Named(t => t.Name.Substring(1))
                        .Named(t => "Other" + t.Name.Substring(1))
                        .FromNamespace(t => typeof(Foo).Namespace)
                        .FromAssembly(t => t.Assembly)
                    );
            });
            var service = container.GetService<IFoo[]>();
            Assert.Equal(2, service.Length);
            Assert.IsType<Foo>(service[0]);
            Assert.IsType<OtherFoo>(service[1]);
        }
    }
}
