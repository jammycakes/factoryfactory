namespace Nooshka.Impl
{
    public class RegistrationResolver : Resolver
    {
        public RegistrationResolver(Registration registration)
            : base(registration)
        {
        }

        public override object GetService(ServiceRequest request)
        {
            return Registration.ImplementationFactory(request);
        }
    }
}
