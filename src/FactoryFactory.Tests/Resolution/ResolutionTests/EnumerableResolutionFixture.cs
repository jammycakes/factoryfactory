using FactoryFactory.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Resolution.ResolutionTests
{
    public class EnumerableResolutionFixture
    {
        [Fact]
        public void UnresolvableDependenciesShouldResolveAsEmptyEnumerable()
        {
            /*
             * ASP.NET Core requires this test to pass (it fails in 0.1.0), but
             * this case is not covered by the standard tests in the Microsoft
             * specification fixture.
             */
            var container = new Configuration().CreateContainer();
            var service = container.GetService<ServiceWithEnumerableDependencies>();
            Assert.Empty(service.Dependencies);
        }
    }
}
