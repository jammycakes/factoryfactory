using System;
using System.Collections.Generic;
using Nooshka.Registration;

namespace Nooshka
{
    public interface IModule
    {
        void Add(Registration.ServiceRegistration serviceRegistration);

        IEnumerable<Registration.ServiceRegistration> GetRegistrations(Type type);

        bool IsTypeRegistered(Type type);
    }
}
