using System;

namespace Nooshka.Registration
{
    public interface IOptions
    {
        /// <summary>
        ///  Sets a precondition on this registration.
        /// </summary>
        /// <param name="precondition"></param>
        /// <returns></returns>
        IOptions When(Func<ServiceRequest, bool> precondition);

        /// <summary>
        ///  Specifies the registration lifecycle.
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        IOptions WithLifecycle(ILifecycle lifecycle);
    }
}
