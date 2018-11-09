---
title: "Interception and decoration"
permalink: /interception/
sidebar: introduction
section: Advanced topics
---

Interception and Decoration
===========================
FactoryFactory has flexible and powerful support for **interception**.
Interception allows you to modify services that you have requested, performing
various actions on them (such as calling methods or setting properties),
wrapping them in a decorator class that implements the same interface (such as a
dynamic proxy, for example), or even replacing them altogether.

In contrast to some IOC containers, which only allow you to intercept a service
after it has been created, FactoryFactory also allows you to intercept the
resolution process beforehand. This would allow you to defer it to a later time
or even to short-circuit it altogether. You could use this, for example, to
instantiate services lazily using a dynamic proxy, which would be one way of
working with circular dependencies.

Creating an interceptor
-----------------------
You can intercept a service of type `TService` simply by creating and
registering a corresponding service of type `IInterceptor<TService>`. The
`IInterceptor` interface defines one method:

```c#
TService Intercept(ServiceRequest request, Func<TService> serviceFunc);
```

A simple interceptor might look something like this:

```c#
public class AuthenticatingInterceptor<TService> : IInterceptor<TService>
    where TService : IAuthenticatedService
{
    private ILoginSession _session;

    /*
     * Interceptors are resolved in exactly the same way as any other
     * service. This means that they can have dependencies injected into them
     * in their constructors as well.
     */

    public AuthenticatingInterceptor(ILoginSession session)
    {
        _session = session;
    }

    public TService Intercept(ServiceRequest request, Func<TService> serviceFunc)
    {
        /*
         * Invoking serviceFunc() creates the TService instance.
         * If you have more than one interceptor, it may do so through the other
         * Intercept(...) methods of those interceptors as well.
         */
        var service = serviceFunc();
        service.SessionKey = _session.Key;
        return service;
    }
}
```

You can then register it with the `Intercept` method on your module:

```c#
module.Intecept<IBookingService>()
    .With<AuthenticatingInterceptor<BookingService>>();
```

Alternatively you can register the interceptor directly:

```c#
module.Define<IInterceptor<IbookingService>>()
    .As<AuthenticatingInterceptor<BookingService>>();
```

Interceptors can be defined as open generics:

```c#
module.Define(typeof(IInterceptor<>))
    .As(typeof(AuthenticatingInterceptor<>));
```

By default, interceptors defined in this way will be applied to all services
right across the board. If that is not what you want, apply a generic
constraint to your implementation of `IInterceptor<T>`, as is demonstrated in
the example above.