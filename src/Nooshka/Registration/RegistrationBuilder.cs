using System;
using Nooshka.Lifecycle;
using Nooshka.Registration.Fluent;

namespace Nooshka.Registration
{
    public class RegistrationBuilder : IRegistration, IImplementation, IOptions
    {
        public Type ServiceType { get; private set; }

        public Func<ServiceRequest, bool> Precondition { get; private set; }
            = sr => true;

        public Func<ServiceRequest, object> ImplementationFactory { get; private set; }

        public Type ImplementationType { get; private set; }

        public ILifetime Lifetime { get; private set; }
            = new TransientLifetime();

        public RegistrationBuilder(Type serviceType)
        {
            ServiceType = serviceType;
            if (!serviceType.IsAbstract) ImplementationType = serviceType;
        }

        /* ====== IImplementation ====== */

        public IOptions When(Func<ServiceRequest, bool> precondition)
        {
            this.Precondition = precondition;
            return this;
        }

        public IOptions WithLifetime(ILifetime lifetime)
        {
            this.Lifetime = lifetime;
            return this;
        }

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
    }
}
