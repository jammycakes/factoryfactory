using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FactoryFactory
{
    /// <summary>
    ///  Provides a definition for one or more registered services. Service
    ///  definitions can be for a single type and implementation, or for an
    ///  open generic, or for a convention.
    /// </summary>

    public interface IServiceDefinition
    {
        /// <summary>
        ///  Gets all the types that can provide the requested service.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetTypes(Type requestedType);

        /// <summary>
        ///  Gets all the directly registered instances that can provide the
        ///  requested service.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IEnumerable<object> GetInstances(Type requestedType);

        /// <summary>
        ///  Gets all the registration expressions that can provide the
        ///  requested service.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IEnumerable<Expression<Func<ServiceRequest, object>>> GetExpressions(Type requestedType);

        /// <summary>
        ///  Gets the precondition for this service definition.
        /// </summary>
        Func<ServiceRequest, bool> Precondition { get; }

        /// <summary>
        ///  Gets the lifecycle for this service definition.
        /// </summary>
        ILifecycle Lifecycle { get; }

        /// <summary>
        ///  Indicates which service takes precedence when resolving a single
        ///  instance. Single instances (priority 3000) take precedence over
        ///  open generics (priority 2000) and convention-based registrations
        ///  (priority 1000).
        /// </summary>
        int Priority { get; }
    }
}
