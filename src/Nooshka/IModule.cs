using System;
using System.Collections.Generic;
using Nooshka.Registration;

namespace Nooshka
{
    public interface IModule
    {
        IEnumerable<IRegistration> GetServiceRegistrations(Type type);
    }
}
