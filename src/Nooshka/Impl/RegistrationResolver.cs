namespace Nooshka.Impl
{
    public class RegistrationResolver : Resolver
    {
        public RegistrationResolver(Registration registration)
            : base(registration)
        {
        }

        protected override object Resolve(ServiceRequest request)
        {
            return Registration.ImplementationFactory(request);
        }
    }
}
