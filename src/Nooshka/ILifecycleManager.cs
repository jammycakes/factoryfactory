using System;

namespace Nooshka
{
    public interface ILifecycleManager : IDisposable
    {
        void Add(IDisposable managedService);

        Container Container { get; }
    }
}
