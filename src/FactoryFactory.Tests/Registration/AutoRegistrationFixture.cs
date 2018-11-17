using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FactoryFactory.Tests.Registration
{
    public class AutoRegistrationFixture
    {
        private class Foo { }

        [Fact]
        public void SettingAutoResolveToFalseShouldPreventAutoResolve()
        {
            var configuration = new Configuration(
                new ConfigurationOptions(autoResolve: false)
            );
            var container = configuration.CreateContainer();
            Assert.Null(container.GetService<Foo>());
        }

        [Fact]
        public void SettingAutoResolveToTrueShouldEnableAutoResolve()
        {
            var configuration = new Configuration(
                new ConfigurationOptions(autoResolve: true)
            );
            var container = configuration.CreateContainer();
            Assert.IsType<Foo>(container.GetService<Foo>());
        }
    }
}
