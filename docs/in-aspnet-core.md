---
title: "Using FactoryFactory with ASP.NET Core"
sidebar: introduction
section: Developer guide
permalink: /in-aspnet-core/
---
Using FactoryFactory with ASP.NET Core
======================================

Because FactoryFactory implements Microsoft's abstractions, you can use it as a
drop-in replacement for the bare-bones Microsoft container with minimal effort.
All you need to do is to add the
[FactoryFactory.AspNet.DependencyInjection](https://www.nuget.org/packages/FactoryFactory.AspNet.DependencyInjection/)
to your project from NuGet:

```powershell
Install-Package FactoryFactory.AspNet.DependencyInjection -Pre
```

Then add a call to `.UseFactoryFactory()` to your web host builder in
`Program.cs`:

```c#
using FactoryFactory.AspNet.DependencyInjection;

/* snip */

public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseFactoryFactory()            // <-- Add this line here
        .UseStartup<Startup>();
```

For getting started, that is all you need to do. FactoryFactory will
automatically pick up on any services that you have registered in the usual way
in the `ConfigureServices(IServiceCollection)` method in `Startup.cs`.

Using FactoryFactory-specific features
--------------------------------------
If you want to use any of the advanced functionality that FactoryFactory offers,
such as decorators, interceptors, custom lifecycles or conditional injection,
you will need to add an extra method to your `Startup.cs` class to expose the
FactoryFactory API. To do this, add a method `ConfigureContainer(Module module)`
to the class as follows:

```c#
public void ConfigureContainer(Module module)
{
    module.Define<IClock>().As<Clock>().Singleton();
    module.Decorate<IClock>().With<TimeZoneDecorator>();
}
```

You can have both `ConfigureServices` and `ConfigureContainer` methods in your
`Startup.cs` class: you don't have to choose between one and the other. If you
have both, FactoryFactory will call `ConfigureServices(IServiceCollection)`
first, then your new `ConfigureContainer(Module)` method second.
