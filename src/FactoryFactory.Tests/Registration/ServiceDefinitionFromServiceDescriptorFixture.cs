using System.Collections;
using System.Linq;
using FactoryFactory.Registration.Impl.ServiceDefinitions;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Registration
{
    public class ServiceDefinitionFromServiceDescriptorFixture
    {
        [Fact]
        public void CanCreateServiceDefinitionByInstance()
        {
            var instance = A.Fake<IEqualityComparer>();
            var descriptor = new ServiceDescriptor(typeof(IEqualityComparer), instance);
            var definition = new ServiceDefinition(descriptor);
            Assert.Equal(definition.ImplementationInstance, instance);
            Assert.Null(definition.ImplementationFactory);
            Assert.Null(definition.ImplementationType);

            var idef = (IServiceDefinition)definition;
            Assert.Empty(idef.GetExpressions(typeof(IEqualityComparer)));
            Assert.Empty(idef.GetTypes(typeof(IEqualityComparer)));
            Assert.Equal(instance, idef.GetInstances(typeof(IEqualityComparer)).Single());
        }
    }
}
