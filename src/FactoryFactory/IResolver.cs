using System;

namespace FactoryFactory
{
    /// <summary>
    ///  Provides resolution services for a request for a specific type.
    /// </summary>
    /// <remarks>
    ///  FactoryFactory generates a single, central implementation of IResolver
    ///  for every type that it is asked for. This includes separate IResolver
    ///  instances for TService, IEnumerable<TService>, Func<TService> and
    ///  TService[]. It also includes IResolver instances for requests for
    ///  services that FactoryFactory is unable to resolve, e.g. value types,
    ///  unregistered interfaces or abstract classes, or open generics.
    ///
    ///  IResolver instances are responsible for locating and instantiating the
    ///  requested service, including retrieving it from a lifecycle cache and
    ///  storing it in a lifecycle tracker if appropriate. As much of the
    ///  process as possible is determined and cached in the IResolver instances
    ///  which can be tightly optimised for specific use cases if necessary e.g.
    ///  singletons, services registered by instance, or services that do not
    ///  implement IDisposable.
    ///
    ///  IResolver instances can -- and should -- be nested, with the result
    ///  from one resolver being passed to other resolvers. This means, for
    ///  example, that we can wrap a singleton resolver in a tracker resolver,
    ///  a cache resolver, a conditional resolver and a decorator resolver.
    ///
    ///  By doing it this way, we can make basic calls to Container.GetService()
    ///  very, very fast: in the best case, they amount to a couple of method
    ///  calls, a direct constructor invocation, and just one or two hashtable
    ///  lookups. The target here is to get the best case GetService() calls
    ///  down to within 100 nanoseconds; if I can get it down even further to 50
    ///  nanoseconds, it would be up there with the best of the best.
    ///
    ///  Obviously, when we have to handle features such as conditions,
    ///  decorators and the like, it won't quite be as fast as that...
    /// </remarks>
    public interface IResolver
    {
        /// <summary>
        ///  Indicates whether this is a fully functional service resolver, or a
        ///  placeholder indicating types that can not be resolved.
        /// </summary>
        bool CanResolve { get; }

        /// <summary>
        ///  Indicates that this resolver is a conditional resolver; that it
        ///  may or may not apply depending on the request.
        /// </summary>
        bool Conditional { get; }

        /// <summary>
        ///  Gets the priority code for the resolver.
        /// </summary>
        int Priority { get; }

        /// <summary>
        ///  The type of service being resolved.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///  Checks whether this resolver's precondition has been met.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool IsConditionMet(ServiceRequest request);

        /// <summary>
        ///  Resolves and returns the requested service, or null if the service
        ///  can not be resolved.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        object GetService(ServiceRequest request);
    }
}
