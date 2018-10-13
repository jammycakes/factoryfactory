using System;

namespace Nooshka.Registration.Fluent
{
    public interface IOptions
    {
        IOptions When(Func<ServiceRequest, bool> precondition);

        IOptions WithLifetime(ILifetime lifetime);
    }
}
