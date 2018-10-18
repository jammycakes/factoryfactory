using System;
using System.Linq.Expressions;

namespace Nooshka.Impl
{
    public class RegistrationBuilder
    {
        protected ServiceDefinition State { get; }

        public RegistrationBuilder(ServiceDefinition state)
        {
            State = state;
        }

        /// <summary>
        ///  Specifies the concrete class to provide services for this registration.
        /// </summary>
        /// <param name="implementationType">
        ///  The concrete type to implement this service.
        /// </param>
        /// <returns></returns>
        public RegistrationOptions<object> As(Type implementationType)
        {
            State.ImplementationType = implementationType;
            State.ImplementationFactory = null;
            return new RegistrationOptions<object>(State);
        }

        /// <summary>
        ///  Specifies an already-created instance to provide services for this
        ///  registration.
        /// </summary>
        /// <param name="implementation">
        ///  The object that implements this service.
        /// </param>
        /// <returns></returns>
        public RegistrationOptions<object> As(object implementation)
        {
            return As(req => implementation);
        }

        /// <summary>
        ///  Specifies a factory method to provide services for this registration.
        /// </summary>
        /// <returns>
        ///  A factory method that creates the requested service.
        /// </returns>
        public RegistrationOptions<object> As(Expression<Func<ServiceRequest, object>> factory)
        {
            State.ImplementationFactory = factory;
            State.ImplementationType = null;
            return new RegistrationOptions<object>(State);
        }
    }

    public class RegistrationBuilder<TService> : RegistrationBuilder
    {
        public RegistrationBuilder(ServiceDefinition state)
            : base(state)
        {
        }

        /// <summary>
        ///  Specifies the concrete class to provide services for this registration.
        /// </summary>
        /// <returns></returns>
        /// <typeparam name="TImplementation">
        ///  The concrete type to implement this service.
        /// </typeparam>
        /// <returns></returns>
        public RegistrationOptions<TService> As<TImplementation>()
            where TImplementation : TService
        {
            State.ImplementationType = typeof(TImplementation);
            State.ImplementationFactory = null;
            return new RegistrationOptions<TService>(State);
        }

        /// <summary>
        ///  Specifies an already-created instance to provide services for this registration.
        /// </summary>
        /// <param name="implementation">
        ///  The object that implements this service.
        /// </param>
        /// <returns></returns>
        public RegistrationOptions<TService> As(TService implementation)
        {
            State.ImplementationFactory = req => implementation;
            State.ImplementationType = null;
            return new RegistrationOptions<TService>(State);
        }

        /// <summary>
        ///  Specifies a factory method to provide services for this registration.
        /// </summary>
        /// <param name="factory">
        ///  A factory method that creates the requested service.
        /// </param>
        /// <returns></returns>
        public RegistrationOptions<TService> As(Expression<Func<ServiceRequest, TService>> factory)
        {
            State.ImplementationFactory = Expression.Lambda<Func<ServiceRequest, object>>(
                factory.Body,
                factory.Parameters
            );
            State.ImplementationType = null;
            return new RegistrationOptions<TService>(State);
        }
    }
}
