using System;
using FactoryFactory.Util;

namespace FactoryFactory
{
    /// <summary>
    ///  Encapsulates a request for a service. This may be a direct, root-level
    ///  request to the IOC container itself, or it may be a request for a
    ///  dependency to be injected into another service.
    /// </summary>
    public class ServiceRequest
    {
        /// <summary>
        ///  The <see cref="Container"/> instance to which the request was
        ///  originally made. This may or may not be the container that
        ///  ultimately provides lifecycle management services: that is
        ///  determined by the ILifecycle instances on the service definition.
        /// </summary>
        public IContainer Container { get; }

        /// <summary>
        ///  The type of object that is being requested.
        /// </summary>
        public Type RequestedType { get; }

        /// <summary>
        ///  The request for the service into which this dependency is being
        ///  injected, if any.
        /// </summary>
        public ServiceRequest Receiver { get; }


        /// <summary>
        ///  Creates a new instance of the <see cref="ServiceRequest"/> class.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="requestedType"></param>
        /// <param name="receiver"></param>
        internal ServiceRequest(IContainer container, Type requestedType, ServiceRequest receiver)
        {
            Container = container;
            RequestedType = requestedType;
            Receiver = receiver;
        }

        internal ServiceRequest CreateDependencyRequest(Type dependencyType)
        {
            return new ServiceRequest(Container, dependencyType, this);
        }

        internal object ResolveDependency(Type dependencyType)
        {
            return Container.GetService(CreateDependencyRequest(dependencyType));
        }

        public override string ToString() => $"ServiceRequest for {RequestedType}";
    }
}
