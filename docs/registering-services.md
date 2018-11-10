---
title: "Registering services"
permalink: /registering-services/
sidebar: introduction
section: Developer guide
---

Registering services
====================

At the most basic level, FactoryFactory works in the same way as every other IOC
container:

 1. Register the services that your application needs.
 2. Resolve your services as needed.
 3. Release your container once you're finished with it, which will implicitly
    release any services that it has created for you.

THis three-step process is called the
[Register-Resolve-Release pattern](http://blog.ploeh.dk/2010/09/29/TheRegisterResolveReleasepattern/).

Service definitions and modules
-------------------------------
In FactoryFactory, you register services by creating **service definitions**. A
service definition is simply a rule that tells the container: "Given that I have
requested a service of type `TService`, when any preconditions that I have
specified here are met, then I should receive a service of such-and-such a type,
constructed by such-and-such a method."

Service definitions are often simple mappings from an interface or a base class
to a type that implements or extends them. They can also be more complex
mappings, for example, specifying conventions.

Service definitions are registered in the first instance with **modules**.
FactoryFactory's `Module` class provides you with a fluent interface to create
service definitions. This looks like this:

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

Implicit registration
---------------------
You don't have to register every service that you need. If a service is a
concrete class, as opposed to an interface or an abstract base class, requests
for that specific class will be automatically resolved, even if they haven't
yet been registered with the container:

```c#
// This will work even if Program has not been explicitly registered.
var program = container.GetService<Program>();
```

Creating a container
--------------------
The easiest way to create a container is to call the `CreateContainer` extension
method on your module:

```c#
var container = myModule.CreateContainer();
```

You can also create a FactoryFactory container from an `IServiceCollection`
interface in the ASP.NET IOC abstractions:

```c#
var container = myServiceCollection.CreateFactoryFactory();
```

**Note:** Containers are immutable. Once you have created a container,
reconfiguring the module from which you created it will have no effect.

Advanced container creation
---------------------------
If you want to create a container from multiple modules, or to specify certain
advanced options to customise its behaviour, you should create it via the
`Configuration` class:

```c#
var configuration = new Configuration(module1, module2, module3);
var container = configuration.CreateContainer();
```
