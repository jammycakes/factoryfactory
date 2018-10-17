using System;
using Nooshka.Impl;

namespace Nooshka
{
    public interface ILifecycleManager : IDisposable
    {
        void Cache(ServiceDefinition serviceDefinition, object service);

        void Track(IDisposable service);

        object GetExisting(ServiceDefinition serviceDefinition);
    }
}
