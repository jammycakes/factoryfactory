using System;
using System.Collections.Generic;

namespace Nooshka
{
    public interface IModule
    {
        void Add(ServiceDefinition serviceDefinition);

        IEnumerable<ServiceDefinition> GetRegistrations(Type type);

        bool IsTypeRegistered(Type type);
    }
}
