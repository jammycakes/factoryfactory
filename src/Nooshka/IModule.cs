using System;
using System.Collections.Generic;
using Nooshka.Registration;

namespace Nooshka
{
    public interface IModule
    {
        void Add(IRegistration registration);

        IEnumerable<IRegistration> GetRegistrations(Type type);
    }
}
