using System;

namespace FactoryFactory.Tests.Model
{
    public class DisposableService : IDisposable
    {
        public bool Disposed { get; private set; } = false;

        public void Dispose()
        {
            Disposed = true;
        }
    }
}
