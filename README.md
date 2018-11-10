FactoryFactory: an IOC container
================================

[![Build status](https://ci.appveyor.com/api/projects/status/rrgswl1ta25twbsh?svg=true)](https://ci.appveyor.com/project/jammycakes/factoryfactory)
![NuGet Pre Release](https://img.shields.io/nuget/vpre/FactoryFactory.svg?style=plastic)
[![Join the chat at https://gitter.im/factoryfactory/Lobby](https://badges.gitter.im/factoryfactory/Lobby.svg)](https://gitter.im/factoryfactory/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

FactoryFactory is a new IOC container for .NET. It has been built from the
ground up to support the .NET Core abstractions out of the box, and it features
a fluent interface, preconditions, custom lifecycles, and whatever other bits
and pieces I see fit to introduce as I feel like it.

Usage:
------
Get it from NuGet:

```
Install-Package FactoryFactory
```

Create one or more modules to contain your service definitions:

```c#
using FactoryFactory;

public class MyModule: Module
{
    public Module(string[] args)
    {
        Define<IBlogRepository>().As<BlogRepository>();
        Define<IBlogService>().As<BlogService>();
        Define<DbContext>().As<DbContext>();
        Define<Program>().As<Program>(req => new Program(args));
    }
} 
```

Create a container from a Configuration, then get your root service:

```c#
var cfg = new Configuration(new MyModule());
using (var container = cfg.CreateContainer()) {
    container.GetService<Program>().Run();
}
```

## In ASP.NET Core:

Install FactoryFactory.AspNet.DependencyInjection:

```
Install-Package FactoryFactory.AspNet.DependencyInjection
```

In Program.cs, add a call to `.UseFactoryFactory()` to your `WebHostBuilder`:

```c#
using FactoryFactory.AspNet.DependencyInjection;

/* snip */

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseFactoryFactory()
                .UseStartup<Startup>();
```

In Startup.cs, add a `ConfigureContainer(Module)` method:

```c#
public void ConfigureContainer(Module module)
{
    // Add your registrations to Module here
}
```

## Roadmap:

 * **0.1:**
   * Fully functional drop-in replacement for the ASP.NET Core container in
     `Microsoft.Extensions.DependencyInjection`.
   * Preconditions (untested)
   * Custom lifecycles
 * **0.2:**
   * Interceptors
   * Documentation
 * **0.3:**
   * Conventions
   * Direct registration of decorators
 * **0.4:**
   * Feature switches
   * Automatic lazy resolution by dynamic proxy (separate project)
 * **1.0:**
   * Validation
   * `Resolve.From<T>()` anywhere in any expression