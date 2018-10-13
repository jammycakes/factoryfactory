using System;

namespace Nooshka
{
    public interface IServiceResolver
    {
        object GetService(ServiceRequest request);

        ILifetimeManager LifetimeManager { get; }

        IServiceResolver Parent { get; }

        IServiceResolver Root { get; }

        IServiceResolver CreateChild();
    }
}
