using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace FactoryFactory.Tests.Registration
{
    public class ServiceDefinitionFixture
    {
        [Fact]
        public void ServiceTypeMustBeSpecified()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(serviceType: null);
            });
        }

        [Fact]
        public void ImplementationTypeOrImplementationExpressionMustBeSpecified()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(object));
            });
        }

        [Fact]
        public void ImplementationMustNotSpecifyBothTypeAndExpression()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(object), req => "Hello world", typeof(object));
            });
        }

        [Fact]
        public void ImplementationByValueMustNotBeAValueType()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(object), req => 3);
            });
        }

        [Fact]
        public void ImplementationByExpressionMustNotBeAValueType()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(object), req => req.RequestedType.IsAbstract ? 3 : 0);
            });
        }


        [Fact]
        public void ImplementationTypeMustNotBeAValueType()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(object), null, typeof(int));
            });
        }

        [Fact]
        public void ImplementationByTypeMustNotBeAnInterface()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(IEnumerable), null, typeof(IEnumerable<string>));
            });
        }

        [Fact]
        public void ImplementationByTypeMustNotBeAnAbstractClass()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(IList), null, typeof(CollectionBase));
            });
        }

        [Fact]
        public void OpenGenericMustBeRegisteredByType()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(IEnumerable<>), req => new object());
            });
        }

        [Fact]
        public void OpenGenericMustBeRegisteredAsOpenGeneric()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(IEnumerable<>), null, typeof(IEnumerable<string>));
            });
        }

        [Fact]
        public void NonOpeGenericMustNotBeRegisteredAsOpenGeneric()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(IEnumerable<string>), null, typeof(List<>));
            });
        }

        class List2<T> : List<T> {}

        class NonList<T> {}

        [Fact]
        public void OpenGenericImplementationsMustImplementInterfaceOrClass()
        {
            Assert.Throws<IoCException>(() => {
                new ServiceDefinition(typeof(List<>), null, typeof(NonList<>));
            });
        }
    }
}
