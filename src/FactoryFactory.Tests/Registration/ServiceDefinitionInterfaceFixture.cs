using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FactoryFactory.Tests.Registration
{
    public class ServiceDefinitionInterfaceFixture
    {
        [Fact]
        public void CanDefineOpenGeneric()
        {
            IServiceDefinition definition = new ServiceDefinition(
                typeof(IList<>),
                implementationType: typeof(List<>)
            );

            var types = definition.GetTypes(typeof(IList<string>)).ToArray();
            Assert.Equal(new[] {typeof(List<string>)}, types);
        }
    }
}
