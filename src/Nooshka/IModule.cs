using System;
using System.Collections.Generic;
using Nooshka.Registration;

namespace Nooshka
{
    public interface IModule
    {
        void Add(Registration.Registration registration);

        IEnumerable<Registration.Registration> GetRegistrations(Type type);

        bool IsTypeRegistered(Type type);
    }
}
