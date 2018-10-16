namespace Nooshka.Impl
{
    // TODO: WE NEED TO MAKE SURE WE'RE USING THE CORRECT CONTAINER!
    // Currently all requests are going to the requesting container, and
    // lifecycles are not being handled. This needs to be fixed!

    public abstract class Resolver
    {
        public Resolver(Registration registration)
        {
            Registration = registration;
        }

        public Registration Registration { get; }

        public bool PreconditionMet(ServiceRequest request) =>
            Registration.Precondition(request);

        public abstract object GetService(ServiceRequest request);
    }
}
