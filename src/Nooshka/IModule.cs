using System;
using System.Collections.Generic;
using Nooshka.Impl;

namespace Nooshka
{
    public interface IModule
    {
        void Add(ServiceDefinition serviceDefinition);

        IEnumerable<ServiceDefinition> GetRegistrations(Type type);

        bool IsTypeRegistered(Type type);
    }
}
