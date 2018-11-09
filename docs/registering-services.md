---
title: "Registering services"
permalink: /registering-services/
sidebar: introduction
section: Developer guide
---

Registering services
====================

There are three basic aspects to an IOC container's functionality: service
registration, service resolution, and lifecycle management. This is known as the
[Register-Resolve-Release pattern](http://blog.ploeh.dk/2010/09/29/TheRegisterResolveReleasepattern/). This
section concerns the first of the three.

You register services in FactoryFactory by adding service definitions to a
**module**. A `Module` is simply a collection of `IServiceDefinition` instances
with a fluent interface to allow you to add them clearly and expressively.
You can define services by *type*, by *instance* or by *expression*.

This is what it looks like:

```c#
// Create a new module
var module = new Module();
// Define a service by type.
module.Define<IUserService>().As<UserService>();
// Define a service by instance.
module.Define<IClock>().As(new Clock());
// Define a service by expression.
module.Define<IOfferOfTheDay>().As(req =>
    req.Container.GetService<ICalendar>().OfferOfTheDay
);
```

It is also possible to register open generics. As of version 0.2, these can only
be registered by type:

```c#
module.Define(typeof(IRepository<>)).As(typeof(Repository<>));
```

Finally, you don't have to register every service that you need. If a service is
a concrete class, as opposed to an interface or an abstract base class, requests
for that specific class will be automatically resolved, even if they haven't
yet been registered with the container:

```c#
// This will work even if Program has not been explicitly registered.
var program = container.GetService<Program>();
```

If you prefer, you can subclass `Module` and define your services in the
constructor:

```c#
public class MyModule : Module
{
    public MyModule()
    {
        Define<IUserService>().As<UserService>();
        Define<IClock>().As(new Clock());
        Define<IOfferOfTheDay>().As(req =>
            req.Container.GetService<ICalendar>().OfferOfTheDay
        );
    }
}
```

Once you have done this, you can use the `Module` class to create a container by
passing it into the `Configuration` class. There are several ways of doing this;
this is the simplest:

```c#
var container = Configuration.CreateContainer(module);
```

You can provide multiple modules if you like:

```c#
var container = Configuration.CreateContainer(module1, module2);
```

Creating a container from a module
----------------------------------
You can create a container from a module by first passing them into a
`Configuration` constructor, then calling `CreateContainer()` on your
`Configuration` instance:

```c#
var configuration = new Configuration(myModule);
var container = configuration.CreateContainer();
```

Most of the time, you won't need direct access to the `Configuration` class,
however. Accordingly, to keep things simple, `Configuration` provides you with
some static methods to act as shortcuts:

```c#
// From a single module
var container = Configuration.CreateContainer(myModule);

// From multiple modules
var container = Configuration.CreateContainer(module1, module2, module3);

// from an IServiceCollection
var container = Configuration.CreateContainer(myServiceCollection);

// configuring a module in the constructor call itself:
var container = Configuration.CreateContainer(module => {
    module.Define<IUserService>().As<UserService>();
    module.Define<IClock>().As(new Clock());
    module.Define<IOfferOfTheDay>().As(req =>
        req.Container.GetService<ICalendar>().OfferOfTheDay
    );
});
```


