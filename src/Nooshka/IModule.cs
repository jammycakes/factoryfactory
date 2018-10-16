using System;
using System.Collections.Generic;
using Nooshka.Impl;

namespace Nooshka
{
    public interface IModule
    {
        void Add(ServiceRegistration serviceRegistration);

        IEnumerable<ServiceRegistration> GetRegistrations(Type type);

        bool IsTypeRegistered(Type type);
    }
}
