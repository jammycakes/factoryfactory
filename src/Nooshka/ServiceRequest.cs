using System;

namespace Nooshka
{
    /// <summary>
    ///  Encapsulates a request for a service. This may be a direct, root-level
    ///  request to the IOC container itself, or it may be a request for a
    ///  dependency to be injected into another service.
    /// </summary>
    public struct ServiceRequest
    {
        /// <summary>
        ///  The <see cref="IServiceResolver"/> instance to which the request was
        ///  originally made. This may or may not be the service resolver that
        ///  ultimately creates and manages the service.
        /// </summary>
        public IServiceResolver Resolver { get; }

        /// <summary>
        ///  The type of object that is being requested.
        /// </summary>
        public Type RequestedType { get; }

        /// <summary>
        ///  The type of object into which this service is being injected.
        ///  For root-level requests, this will be null.
        /// </summary>
        public Type ReceivingType { get; }

        /// <summary>
        ///  Creates a new instance of the <see cref="ServiceRequest"/> class.
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="requestedType"></param>
        /// <param name="receivingType"></param>
        public ServiceRequest(IServiceResolver resolver, Type requestedType, Type receivingType)
        {
            Resolver = resolver;
            RequestedType = requestedType;
            ReceivingType = receivingType;
        }
    }
}
