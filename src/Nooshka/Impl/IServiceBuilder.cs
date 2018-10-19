using System;
using System.Collections.Generic;

namespace Nooshka.Impl
{
    public interface IServiceBuilder
    {
        object GetService(ServiceRequest request);
    }
}
