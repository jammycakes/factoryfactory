using Nooshka.Registration;

namespace Nooshka.Resolution
{
    public class RegistrationServiceResolver : IServiceResolver
    {
        private readonly Registration.ServiceRegistration _serviceRegistration;

        public RegistrationServiceResolver(Registration.ServiceRegistration serviceRegistration)
        {
            _serviceRegistration = serviceRegistration;
        }

        public bool PreconditionMet(ServiceRequest request)
        {
            return _serviceRegistration.Precondition(request);
        }

        public object GetService(ServiceRequest request)
        {
            return _serviceRegistration.ImplementationFactory(request);
        }
    }
}
