---
title: "Resolving services"
permalink: /resolving-services/
sidebar: introduction
section: Developer guide
---

Resolving services
==================

The second responsibility of an IOC container, in the Register-Resolve-Release
pattern, is to resolve a service. There are two ways of doing so:

 1. By requesting it directly from the container.
 2. By having it injected into another service that the container has provided.

The first option is called **service location** while the second option is
called **dependency injection**.

The best practice for using IOC containers is to request only your topmost
service from the container -- or even better still, to leave your application
framework to do so for you -- and use dependency injection to handle the rest.
For example, in a console application, you would only want to perform a service
location in your program's `Main` method, while in ASP.NET Core, all that is
handled for you behind the scenes, and any services that you register are
injected directly into your controllers.

To get your root service, call the `GetService` method:

```c#
static int Main(string[] args)
{
    var myModule = CreateModule();
    using (var container = Configuration.CreateContainer(myModule))
    {
        myProgram = container.GetService<Program>();
        return myProgram.Run(args);
    }
}
```

For anything else, you should just leave FactoryFactory to inject dependencies
into your service's constructor. For example, your `Program` class might look
like this:

```c#
public class Program
{
    private IClock _clock;

    public Program(IClock clock)
    {
        _clock = clock;
    }

    public int Run(string[] args)
    {
        Console.WriteLine($"Hello world, the time is {_clock.UtcNow}");
    }
}
```

What you **shouldn't** do is pepper your codebase with calls to FactoryFactory
all over the place. For example, don't do this:

```c#
public class Program
{
    private IClock _clock;

    public Program()
    {
        _clock = Global.Container.GetService<IClock>();
    }

    public int Run(string[] args)
    {
        Console.WriteLine($"Hello world, the time is {_clock.UtcNow}");
    }
}
```

Resolving collections
---------------------
It is possible to define multiple implementations of a service:

```c#
module.Define<IPasswordHashProvider>().As<Argon2HashProvider>();
module.Define<IPasswordHashProvider>().As<SCryptHashProvider>();
module.Define<IPasswordHashProvider>().As<BCryptHashProvider>();
module.Define<IPasswordHashProvider>().As<PBKDF2HashProvider>();
```

You can ask FactoryFactory for all of them, by requesting `IEnumerable<T>`:

```c#
public class UsersService
{
    private IUsersRepsoitory _usersRepository;
    private List<IPasswordHashProvider> _hashProviders;

    public UsersService(IUsersRepository usersRepository,
        IEnumerable<IPasswordHashProvider> hashProviders)
    {
        _usersRepository = usersRepository;
        _hashProviders = hashProviders.ToList();
    }
}
```

Services in this case will be returned in the order in which they were
registered. You can also request a collection of services by asking for
`ICollection<T>`, `IReadOnlyCollection<T>`, `IList<T>` or `List<T>`.

If no services have been registered for a particular type, the collection will
be empty, even if the service requested has a public constructor and could be
resolved automatically as a single instance.

Resolving services lazily
-------------------------
It frequently happens that you have a service which relies on serveral
dependencies, some of which might not be used. For example, a controller action
won't need to use any of its dependent services if a user is not logged in.

In this case, you can request your services as a `Lazy<TDependency>`. For
example:

```c#
public class AdminService
{
    private ISecurityContext _context;
    private Lazy<IUserManagementRepository> _userManagementRepository;

    public AdminService(ISecurityContext context,
        Lazy<IUserManagementRepository> userManagementRepository)
    {
        _context = context;
        _userManagementRepository = userManagementRepository;
    }

    public IEnumerable<User> GetAllUsers()
    {
        if (!_context.IsAdmin) {
            throw new AccessDeniedException("Admins only!");

            /*
             * _userManagementRepository won't get created if the
             * user is not an admin.
             */
        }

        return _userManagementRepository.GetAllUsers();
    }
}
```
