FactoryFactory: an IOC container
================================
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
