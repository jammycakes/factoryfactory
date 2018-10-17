using System;
using Nooshka.Impl;

namespace Nooshka
{
    public interface IServiceCache : IDisposable
    {
        void Store(ServiceDefinition serviceDefinition, object service);

        void Track(IDisposable service);

        object Retrieve(ServiceDefinition serviceDefinition);
    }
}
