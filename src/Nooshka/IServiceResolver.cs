using System;

namespace Nooshka
{
    public interface IServiceResolver
    {
        object GetService(ServiceRequest request);
    }
}
