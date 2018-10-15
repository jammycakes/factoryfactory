using System;
using System.ComponentModel;

namespace Nooshka.Registration
{
    public class RegistrationBuilder
    {
        protected Registration State { get; }

        public RegistrationBuilder(Registration state)
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
        public RegistrationOptions<object> With(object implementation)
        {
            return From(req => implementation);
        }

        /// <summary>
        ///  Specifies a factory method to provide services for this registration.
        /// </summary>
        /// <returns>
        ///  A factory method that creates the requested service.
        /// </returns>
        public RegistrationOptions<object> From(Func<ServiceRequest, object> factory)
        {
            State.ImplementationFactory = factory;
            State.ImplementationType = null;
            return new RegistrationOptions<object>(State);
        }
    }

    public class RegistrationBuilder<TService> : RegistrationBuilder
    {
        public RegistrationBuilder(Registration state)
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
        public RegistrationOptions<TService> With(TService implementation)
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
        public RegistrationOptions<TService> From(Func<ServiceRequest, TService> factory)
        {
            State.ImplementationFactory = req => factory(req);
            State.ImplementationType = null;
            return new RegistrationOptions<TService>(State);
        }
    }
}
