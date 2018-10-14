using System;
using Nooshka.Lifecycles;

namespace Nooshka.Registration
{
    public class RegistrationBuilder : IRegistration, IImplementation, IOptions
    {
        public Type ServiceType { get; private set; }

        public Func<ServiceRequest, bool> Precondition { get; private set; }
            = sr => true;

        public Func<ServiceRequest, object> ImplementationFactory { get; private set; }

        public Type ImplementationType { get; private set; }

        public Lifecycle Lifecycle { get; private set; }
            = new TransientLifecycle();

        public RegistrationBuilder(Type serviceType)
        {
            ServiceType = serviceType;
            if (!serviceType.IsAbstract) ImplementationType = serviceType;
        }

        /* ====== IOptions ====== */

        public IOptions When(Func<ServiceRequest, bool> precondition)
        {
            this.Precondition = precondition;
            return this;
        }

        public IOptions WithLifecycle(Lifecycle lifecycle)
        {
            this.Lifecycle = lifecycle;
            return this;
        }

        /* ====== IImplementation ====== */

        public IOptions As(Type implementationType)
        {
            this.ImplementationType = implementationType;
            this.ImplementationFactory = null;
            return this;
        }

        public IOptions With(object instance)
        {
            this.ImplementationFactory = req => instance;
            this.ImplementationType = null;
            return this;
        }

        public IOptions From(Func<ServiceRequest, object> request)
        {
            this.ImplementationFactory = request;
            this.ImplementationType = null;
            return this;
        }
    }


    public class RegistrationBuilder<TService>: RegistrationBuilder, IImplementation<TService>
    {
        public RegistrationBuilder() : base(typeof(TService))
        { }

        public IOptions As<TImplementation>() where TImplementation : TService
        {
            return base.As(typeof(TImplementation));
        }

        public IOptions With(TService instance)
        {
            return base.With(instance);
        }

        public IOptions From(Func<ServiceRequest, TService> request)
        {
            return base.From(req => request(req));
        }
    }
}
