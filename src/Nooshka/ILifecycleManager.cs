using System;
using Nooshka.Impl;

namespace Nooshka
{
    public interface ILifecycleManager : IDisposable
    {
        void Cache(Registration registration, object service);

        void Track(IDisposable service);

        object GetExisting(Registration registration);
    }
}
