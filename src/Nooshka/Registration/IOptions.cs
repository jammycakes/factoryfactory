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
        ///  Specifies the registration lifetime.
        /// </summary>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        IOptions WithLifetime(ILifetime lifetime);
    }
}
