using System;

namespace FactoryFactory
{
    public interface IServiceTracker : IDisposable
    {
        void Track(IDisposable service);
    }
}
