using System;
using System.Reflection;
using FactoryFactory.Tests.Model;
using FactoryFactory.Tests.Model.Implementations;
using FactoryFactory.Tests.Model.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Registration
{
    public class ConventionByScanFixture
    {
        [Fact]
        public void ConventionsByScanWillResolve()
        {
            var container = Configuration.CreateContainer(module => {
                module
                    .Define(types => types
                        .Where(t => t.IsInterface)
                        .Where(t => t.Name.StartsWith("I")))
                    .Scanning(scan => { });
            });
            var service = container.GetService<IServiceWithDependencies>();
            /*
             * These should have been registered by the convention.
             * Note that when registering by scan, the order of registration of
             * multiple implementations is undefined. Hence we'll only test that
             * the results actually implement the interfaces we're asking for.
             */
            Assert.IsAssignableFrom<IServiceWithDependencies>(service);
            Assert.IsAssignableFrom<IServiceWithoutDependencies>(service.Dependency);
        }

        [Fact]
        public void ConventionsByScanWillResolveFromSpecifiedNamespace()
        {
            var container = Configuration.CreateContainer(module => {
                module
                    .Define(types => types
                        .Where(t => t.IsInterface)
                        .Where(t => t.Name.StartsWith("I")))
                    .Scanning(scan => scan
                        .FromNamespace(t => typeof(Foo).Namespace));
            });
            var service = container.GetService<IFoo[]>();
            Assert.Equal(2, service.Length);
            Assert.IsType<Foo>(service[0]);
            Assert.IsType<OtherFoo>(service[1]);
        }

        [Fact]
        public void ConventionsByScanWillResolveFromWildcardNamespace()
        {
            var container = Configuration.CreateContainer(module => {
                module
                    .Define(types => types
                        .Where(t => t.IsInterface)
                        .Where(t => t.Name.StartsWith("I")))
                    .Scanning(scan => scan
                        .FromNamespace(t => typeof(IServiceWithDependencies).Namespace + ".*"));
            });
            var service = container.GetService<IFoo[]>();
            Assert.Equal(2, service.Length);
            Assert.IsType<Foo>(service[0]);
            Assert.IsType<OtherFoo>(service[1]);
        }

        [Fact]
        public void ConventionsByScanWillResolveWithSpecifiedConditions()
        {
            var container = Configuration.CreateContainer(module => {
                module
                    .Define(types =>
                        types.Where(t => t.IsInterface).Where(t => t.Name.StartsWith("I")))
                    .Scanning(scan => scan
                        .FromNamespace(t => typeof(Foo).Namespace)
                        .Where((req, impl) => impl.GetCustomAttribute<SerializableAttribute>() != null)
                    );
            });
            var service = container.GetService<IFoo[]>();
            Assert.Single(service);
            Assert.IsType<Foo>(service[0]);
        }

        [Fact]
        public void ConventionsByScanWillResolveFromSpecifiedAssembly()
        {
            var container = Configuration.CreateContainer(module => {
                module
                    .Define(types =>
                        types.Where(t => t.IsInterface).Where(t => t.Name.StartsWith("I")))
                    .Scanning(scan => scan
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
