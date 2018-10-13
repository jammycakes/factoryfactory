using Nooshka.Registration;

namespace Nooshka.Resolution
{
    public class RegistrationServiceResolver : IServiceResolver
    {
        private readonly IRegistration _registration;

        public RegistrationServiceResolver(IRegistration registration)
        {
            _registration = registration;
        }

        public bool PreconditionMet(ServiceRequest request)
        {
            return _registration.Precondition(request);
        }

        public object GetService(ServiceRequest request)
        {
            return _registration.ImplementationFactory(request);
        }
    }
}
