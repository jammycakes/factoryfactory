using System;

namespace Nooshka.Registration.Fluent
{
    public interface IImplementation : IOptions
    {
        IOptions As(Type implementationType);

        IOptions With(object instance);

        IOptions From(Func<ServiceRequest, object> request);
    }

    public interface IImplementation<TService> : IImplementation
    {
        IOptions As<TImplementation>() where TImplementation : TService;
    }
}
