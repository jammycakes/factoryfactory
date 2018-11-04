---
title: "FactoryFactory: yet another IOC container for .NET"
sidebar: introduction
---
Introduction
============

FactoryFactory is a new IOC container, built from the ground up to support the
standard DI abstractions in .NET Core, while at the same time aiming to be as
extensible and flexible as possible. Unlike older containers (StructureMap,
Ninject, AutoFac, Unity, Castle Windsor etc), which have had to struggle with
[complex abstraction layers and internal behavioural changes](https://jamesmckay.net/2018/10/the-state-of-ioc-containers-in-asp-net-core/)
to conform to the Microsoft specifications while maintaining support for legacy
codebases, FactoryFactory supports the new standard out of the box, making it an
ideal drop-in replacement for the comparatively rudimentary container provided
with ASP.NET Core.

Getting started
---------------
To get started, add FactoryFactory to your application from NuGet:

```powershell
Install-Package FactoryFactory -Pre
```

Once you have installed FactoryFactory, you can get up and running quickly in
two easy steps:

 1. Configure and create a container.
 2. Call `GetService()` on your container to fetch your service.

You can do this simply like this:

```c#
using System;
using FactoryFactory;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var container = Configuration.CreateContainer(m => {
                // Define your services here like this:
                m.Define<IClock>().As<Clock>().Singleton();
            }))
            {
                // Program will resolve to itself implicitly.
                container.GetService<Program>().Run();
            }
        }

        private readonly IClock _clock;

        public Program(IClock clock)
        {
            _clock = clock;
        }

        public void Run()
        {
            Console.WriteLine(_clock.UtcNow);
            Console.ReadLine();
        }
    }
}
```

There are more complex approaches that involve separately creating modules and a
`Configuration` instance, but for many scenarios, the above simple approach
should suffice.

In ASP.NET Core
---------------
If you want to use FactoryFactory in an ASP.NET Core application, add the
[FactoryFactory.AspNet.DependencyInjection](https://www.nuget.org/packages/FactoryFactory.AspNet.DependencyInjection/)
to your project from NuGet:

```powershell
Install-Package FactoryFactory.AspNet.DependencyInjection -Pre
```

You can then swap out the basic container for FactoryFactory simply by adding a
call to `.UseFactoryFactory()` to your web host builder in `Program.cs`:

```c#
using FactoryFactory.AspNet.DependencyInjection;

/* snip */

public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseFactoryFactory()
        .UseStartup<Startup>();
```

You can then make use of the FactoryFactory-specific registration features by adding a `ConfigureContainer(Module...)` method to your `Startup.cs` file:

```c#
public void ConfigureContainer(Module module)
{
    module.Define<IClock>().As<Clock>().Singleton();
}
```

Note that you can register services either in the usual
`ConfigureServices(IServiceCollection)` method or in your new
`ConfigureContainer(Module)` method. `ConfigureServices` will be called first,
followed by `ConfigureContainer`.
