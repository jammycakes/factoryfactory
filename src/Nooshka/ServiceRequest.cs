using System;

namespace Nooshka
{
    /// <summary>
    ///  Encapsulates a request for a service. This may be a direct, root-level
    ///  request to the IOC container itself, or it may be a request for a
    ///  dependency to be injected into another service.
    /// </summary>
    public class ServiceRequest
    {
        private ServiceRequest _root;

        /// <summary>
        ///  The <see cref="Container"/> instance to which the request was
        ///  originally made. This may or may not be the container that
        ///  ultimately creates and manages the service.
        /// </summary>
        public Container Container { get; }

        /// <summary>
        ///  The type of object that is being requested. This is not necessarily
        ///  the required service itself; it may be a Func, Lazy or some kind of
        ///  collection.
        /// </summary>
        public Type RequestedType { get; }

        /// <summary>
        ///  The <see cref="ServiceRequest"/> instance for the service into
        ///  which this service is being injected. For root-level requests,
        ///  this will be null.
        /// </summary>
        public ServiceRequest Receiver { get; }

        /// <summary>
        ///  Creates a new instance of the <see cref="ServiceRequest"/> class.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="requestedType"></param>
        /// <param name="receiver"></param>
        public ServiceRequest(Container container, Type requestedType, ServiceRequest receiver)
        {
            Container = container;
            RequestedType = requestedType;
            Receiver = receiver;
            _root = receiver?._root;
        }

        public ServiceRequest CreateDependencyRequest(Type dependencyType)
        {
            return new ServiceRequest(Container, dependencyType, this);
        }

        public object ResolveDependency(Type dependencyType)
        {
            return Container.GetService(CreateDependencyRequest(dependencyType));
        }
    }
}
