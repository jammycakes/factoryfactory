using System;

namespace Nooshka
{
    public interface IServiceTracker : IDisposable
    {
        void Track(IDisposable service);
    }
}
