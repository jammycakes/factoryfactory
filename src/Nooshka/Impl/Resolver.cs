using System;

namespace Nooshka.Impl
{
    public abstract class Resolver
    {
        public Resolver(Registration registration)
        {
            Registration = registration;
        }

        public Registration Registration { get; }

        public bool PreconditionMet(ServiceRequest request) =>
            Registration.Precondition(request);

        public object GetService(ServiceRequest request)
        {
            if (!PreconditionMet(request)) return null;
            var servicingContainer =
                Registration.Lifecycle.GetServicingContainer(request);
            var lifecycleManager = servicingContainer.LifecycleManager;
            var service = lifecycleManager.GetExisting(Registration);
            if (service == null) {
                service = Resolve(request);
                lifecycleManager.Cache(Registration, service);
                if (Registration.Lifecycle.IsTracked && service is IDisposable disposable) {
                    lifecycleManager.Track(disposable);
                }
            }

            return service;
        }

        protected abstract object Resolve(ServiceRequest request);
    }
}
