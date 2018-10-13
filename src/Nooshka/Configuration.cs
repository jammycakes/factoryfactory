using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Nooshka.Registration;
using Nooshka.Resolution;

namespace Nooshka
{
    public class Configuration
    {
        private List<IModule> _modules = new List<IModule>(new[] { new DefaultModule() });

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
            return CreateContainer(new Module(services));
        }

        public static Container CreateContainer(Action<IModule> moduleConfig)
        {
            var module = new Module();
            moduleConfig(module);
            return CreateContainer(module);
        }

        private class DefaultModule : Module
        {
            public DefaultModule()
            {
                this.Add<Container>().From(req => req.Container);
                this.Add<IServiceProvider>().From(req => req.Container);
                this.Add<IServiceScope>().From(req => req.Container.CreateChild());
            }
        }
    }
}
