using System;
using System.Collections.Generic;

namespace FactoryFactory
{
    public interface IModule
    {
        void Add(ServiceDefinition serviceDefinition);

        IEnumerable<ServiceDefinition> GetRegistrations(Type type);

        bool IsTypeRegistered(Type type);
    }
}
