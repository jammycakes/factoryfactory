using System;

namespace Nooshka.Registration
{
    public interface IImplementation : IOptions
    {
        /// <summary>
        ///  Specifies the concrete class to provide services for this
        ///  registration.
        /// </summary>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        IOptions As(Type implementationType);

        /// <summary>
        ///  Specifies an already-created instance to provide services for this
        ///  registration.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        IOptions With(object instance);

        /// <summary>
        ///  Specifies a factory method to provide services for this registration.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IOptions From(Func<ServiceRequest, object> request);
    }

    public interface IImplementation<TService> : IOptions
    {
        /// <summary>
        ///  Specifies the concrete class to provide services for this
        ///  registration.
        /// </summary>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        IOptions As(Type implementationType);

        /// <summary>
        ///  Specifies the concrete class to provide services for this
        ///  registration.
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        IOptions As<TImplementation>() where TImplementation : TService;

        /// <summary>
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        IOptions With(TService instance);

        /// <summary>
        ///  Specifies a factory method to provide services for this registration.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IOptions From(Func<ServiceRequest, TService> request);
    }
}
