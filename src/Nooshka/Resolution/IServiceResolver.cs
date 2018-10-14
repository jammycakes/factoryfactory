namespace Nooshka.Resolution
{
    // TODO: WE NEED TO MAKE SURE WE'RE USING THE CORRECT CONTAINER!
    // Currently all requests are going to the requesting container, and
    // lifecycles are not being handled. This needs to be fixed!

    public interface IServiceResolver
    {
        bool PreconditionMet(ServiceRequest request);

        object GetService(ServiceRequest request);
    }
}
