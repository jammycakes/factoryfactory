




Under the hood, FactoryFactory stores service registrations as a collection of
`IServiceDefinition` instances in one or more `Module` classes. These are then
bundled together in a `Configuration` class, which compiles and optimises them
for improved performance, and then allows you to create a container based on
them. In practice, FactoryFactory gives you a number of convenience methods and
a fluent interface to make it easier to read and follow.

Each service definition contains all the information needed for your container
to resolve a service given its type. This information includes:

 * The type of the service that you are asking for (usually the `IService`
   interface)
 * The class, instance or factory method that will provide that service
 * Information about which constructor to use and which arguments to pass into
   it
 * Any precondition on when this definition should be used
 * The service's lifecycle: when new services should be created, when existing
   services should be reused, and, if they implement `IDisposable`, how long
   they hang around for before they are disposed.

## Basic registration

Services can be registered in one of five ways. First, by specifying a base
class or interface, and a concrete class that implements it:

```
module.Define<IPaymentService>().As<PaymentService>();
```

Second, by specifying an existing instance that will use this service:

```
module.Define<IClock>().As(myClock);
```

Third, by specifying a factory method that will be called to instantiate the
service:

```
module.Define<IClock>().As(req => new Clock(DateTime.UtcNow));
```

Fourth, open generic classes can specify a generic implementation:

```
module.Define(typeof(IRepository<>)).As(typeof(Repository<>));
```

Fifth, if a service is a concrete class (as opposed to an abstract class or an
interface) with a public constructor, it does not need to be explicitly
registered at all, but will be automatically resolved when requested:

```
// Program has not been explicitly registered.
container.GetService<Program>().Run();
```

There are one or two restrictions on what can and can not be registered. These
are as follows:

 * Services must be reference types (`class` rather than `struct`).
 * Open generics can only be registered by type, not by instance or factory
   method.
 * Services that are registered by type must have a public constructor whose
   non-optional arguments all (recursively) meet these criteria.

Finally, you can register one or more services from a .NET Core `IServiceCollection`:

```
module.Add(serviceCollection);
```

## Specifying a constructor

When a service is registered by type, if its implementation has multiple public
constructors, FactoryFactory will choose the one to use "greedily" -- i.e. the
one with the most arguments that it knows how to resolve. This is as expected by
the ASP.NET Core specifications. If two or more constructors have the same
number of arguments, the result is undefined.

It is possible to override this choice using expression-based registration. You
can use `Resolve.From<T>()` to specify services that need to be resolved further
by the IOC container, or you can specify any other values that you want to pass
directly to the constructor. The result looks like this:

```c#
var passwordHasher = new BcryptHasher();
module.Define<IUsersService>().As(req => new UsersService(
    Resolve.From<IUsersRepository>(),
    passwordHasher,
    Environment.GetEnvironmentVariable("APPLICATION_SECRET_KEY")
));
```
