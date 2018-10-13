using System;
using System.Collections.Generic;

namespace Nooshka
{
    public interface IModule
    {
        IEnumerable<ServiceRegistration> GetServiceRegistrations(Type type);
    }
}
