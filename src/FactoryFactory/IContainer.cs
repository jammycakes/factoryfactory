using System;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory
{
    public interface IContainer : IServiceProvider, IServiceScope, IServiceScopeFactory
    {
        IServiceCache ServiceCache { get; }
        IServiceTracker ServiceTracker { get; }
        IContainer Parent { get; }
        IContainer Root { get; }
        object GetService(ServiceRequest request);
        IContainer CreateChild();
    }
}
