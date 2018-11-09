---
title: "Resolving services"
permalink: /resolving-services/
sidebar: introduction
section: Developer guide
---

Resolving services
==================

The second responsibility of an IOC container, in the Register-Resolve-Release
pattern, is to resolve a service. In other words:

 * **Given** that I have registered `AmazonCognitoUsersRepository` as the
   implementation of the `IUsersRepository` interface that I want to use,
 * **When** I request an `IUsersRepository` from the container,
 * **Then** I should get an instance of `AmazonCognitoUsersRepository` back.

There are two ways to request a service from a container. One is to request it
directly. You do this using the `GetService(serviceType)` method:

```c#
static int Main(string[] args) {
    var myModule = CreateModule();
    using (var container = Configuration.CreateContainer(myModule)) {
        myProgram = container.GetService<Program>();
        return myProgram.Run(args);
    }
}
```

The other way is by having FactoryFactory inject additional dependencies into
your service's constructor. For example, your `Program` class might look like
this:

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

This is the approach that you should take most of the time. Normally, you would
only call FactoryFactory directly from the topmost level of your code (in this
example, in the `Main()` method of a command line application), and allow the
constructor injection to do the rest.

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

This approach is called [Service Location](http://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/),
and it is generally regarded as a bad practice.

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
be empty, even if the service requested has a public constructor and could be resolved
automatically as a single instance.
