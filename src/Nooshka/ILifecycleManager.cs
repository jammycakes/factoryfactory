using System;

namespace Nooshka
{
    public interface ILifecycleManager : IDisposable
    {
        void Add(IDisposable managedService);

        IServiceResolver ServiceResolver { get; }
    }
}
