FactoryFactory: an IOC container
================================
FactoryFactory is a new IOC container for .NET. It has been built from the
ground up to support the .NET Core abstractions out of the box, and it features
a fluent interface, preconditions, custom lifecycles, and whatever other bits
and pieces I see fit to introduce as I feel like it.

Its current status is experimental. Since this is still in the pre-alpha stage,
there is no NuGet package available yet. If you want to play with it, you'll
have to compile it yourself, sorry!

Usage:
------
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
