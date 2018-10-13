using System;

namespace Nooshka
{
    public interface ILifetimeManager : IDisposable
    {
        void Add(IDisposable managedService);
    }
}
