---
title: "Lifecycle management"
permalink: /lifecycle-management/
sidebar: introduction
section: Developer guide
---

Lifecycle management
====================
As well as constructing services, IOC containers are responsible for managing
their lifetimes as well, and calling `Dispose()` on any services that implement
`IDisposable`.

The ASP.NET Core abstractions specify three default lifecycles: `Scoped`,
`Singleton` and `Transient`. FactoryFactory supports all three of these out of
the box, together with a fourth, `Untracked`. Additionally, you can create your
own custom lifecycles if need be.

Lifecycles can be specified as follows:

```c#
module.Define<IService1>().As<Service1>().Scoped();
module.Define<IService2>().As<Service2>().Singleton();
module.Define<IService3>().As<Service3>().Transient();
module.Define<IService4>().As<Service4>().Untracked();
```

## What the different lifecycles do

Most IOC containers manage lifecycles by creating a hierarchical system of
containers, or nested scopes in ASP.NET parlance. At the root of your
application, you have a root-level scope, from which your framework creates
additional scopes as needed. In ASP.NET Core, you get a new scope for each web
request, with the scope being disposed when the web request is disposed.

There are two separate aspects to lifecycle management by your IOC containers:
FactoryFactory refers to these as caching and tracking. **Caching** controls how
often a service is created, and under what contexts an existing one is re-used.
It is mediated by `IServiceCache` instances. **Tracking** controls when a
service is disposed, if it implements `IDisposable`. It is mediated by
`IServiceTracker` instances. Each scope contains its own `IServiceCache` and
`IServiceTracker`.

The lifecycles provided by FactoryFactory are as follows:

| Lifecycle   | Creation/caching | Tracking | Notes |
| ----------- | ---------------- | -------- | ----- |
| `Scoped`    | **One per scope:** The same instance is shared by all objects created within the same scope (e.g. the same web request). | The instance is disposed when the scope from which it was requested is disposed. | This is as expected by the .NET Core `Scoped` lifetime. |
| `Singleton` | **One per root-level scope:** One instance is created by the root-level container and shared by all subsequent requests to that container and all other scopes generated from it. | The instance is disposed when the root-level container is disposed. | This is as expected by the .NET Core `Singleton` lifetime. |
| `Transient` | **One per injection:** A new service is created every time it is requested, even within the same object graph. | The instance is disposed when the container from which it was requested is disposed. | This is as expected by the .NET Core `Transient` lifetime. |
| `Untracked` | **One per injection:** A new service is created every time it is requested, even within the same object graph. | The instance is not disposed. | |

**Best practice:** The general rule is that services should be tracked and
disposed by the part of your application that requested them to be created in
the first place. For example, if your container creates an `IRepository`
instance, your container should track it and dispose it. On the other hand, if
you register an `IRepository` instance that you created independently of your
container, then you need to track it yourself. For this reason, if you are
registering services by value rather than by type or by expression, you should
register them as `Untracked`:

```c#
static IRandomNumberGenerator _rng = new RandomNumberGenerator();

// Correct:
module.Define<IRandomNumberGenerator>().As(_rng).Untracked();
// Incorrect:
module.Define<IRandomNumberGenerator>().As(_rng).Singleton();
module.Define<IRandomNumberGenerator>().As(_rng).Scoped();
module.Define<IRandomNumberGenerator>().As(_rng).Transient();

// Correct:
module.Define<IRandomNumberGenerator>().As<RandomNumberGenerator>().Singleton();
// Incorrect:
module.Define<IRandomNumberGenerator>().As<RandomNumberGenerator>().Untracked();
```

## Implementing your own lifecycle

Lifecycles are derived from the `Lifecycle` class, which defines two abstract
methods: `GetTracker` and `GetCache`. Both these methods take a `ServiceRequest`
instance, which you can then use to locate the tracker and cache respectively.
For an example of how this works, see the source code for the lifecycles in the
`FactoryFactory.Lifecycles` namespace. The transient and untracked lifecycles
also act as null trackers and caches (which don't do anything), while the others
fetch the default tracker and cache implementations from the respective
containers.
