using System;

namespace Nooshka.Registration
{
    public interface IRegistration
    {
        /// <summary>
        ///  The type of service being registered.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        ///  The precondition for this service registration to be activated.
        /// </summary>
        Func<ServiceRequest, bool> Precondition { get; }

        /// <summary>
        ///  The factory method that instantiates the service, or null if a
        ///  specific type is specified in the ServiceResolution property.
        /// </summary>
        Func<ServiceRequest, object> ImplementationFactory { get; }

        /// <summary>
        ///  The type that will be constructed which implements the service type
        ///  specified in ServiceType.
        /// </summary>
        Type ImplementationType { get; }

        /// <summary>
        /// </summary>
        ILifecycle Lifecycle { get; }
    }
}
