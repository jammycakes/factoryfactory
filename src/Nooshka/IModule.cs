using System;
using System.Collections.Generic;
using Nooshka.Impl;

namespace Nooshka
{
    public interface IModule
    {
        void Add(Registration registration);

        IEnumerable<Registration> GetRegistrations(Type type);

        bool IsTypeRegistered(Type type);
    }
}
