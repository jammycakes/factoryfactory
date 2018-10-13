using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Modules;

namespace Nooshka
{
    public class Configuration
    {
        private List<IModule> _modules = new List<IModule>();

        /* ====== Register ====== */

        public void AddModule(IModule module)
        {
            _modules.Add(module);
        }

        public Configuration()
        {}

        public Container CreateContainer()
        {
            return new Container(_modules.ToArray());
        }

        /* ====== Static convenience methods ====== */

        public static Container CreateContainer(params IModule[] modules)
        {
            var configuration = new Configuration();
            configuration._modules.AddRange(modules);
            return configuration.CreateContainer();
        }

        public static Container CreateContainer(IServiceCollection services)
        {
            return CreateContainer(new ServiceCollectionModule(services));
        }
    }
}
