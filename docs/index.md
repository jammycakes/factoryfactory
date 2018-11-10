---
title: "FactoryFactory: yet another IOC container for .NET"
sidebar: introduction
section: Developer guide
---
Introduction
============

FactoryFactory is a new IOC container, intended either as a drop-in replacement
for the default container in ASP.NET Core, or as a standalone container for use
in other projects.

 * **Built from the ground up to support the ASP.NET Core abstractions.** Unlike
   other containers, which have had to be retro-fitted with complex abstraction
   layers, and in some cases breaking changes, to comply with the Microsoft
   specifications, FactoryFactory has been built to support them from the start.
 * **Just-in-time preparation.** Unlike many containers, which process and
   compile registrations at start-up, and which perform expensive assembly scans
   to implement conventions, FactoryFactory only compiles type resolvers as they
   are needed. This makes it ideal for use on serverless platforms such as AWS
   Lambda or Azure Functions, where "cold start" times are an important matter
   for consideration.
 * **Open source.** FactoryFactory is licensed under the MIT Licence. The source
   code is [freely available on GitHub](https://github.com/jammycakes/factoryfactory)
   and pull requests will be considered if they are of satisfactory quality and
   appropriate.

Getting started
---------------

To get started, add FactoryFactory to your application from NuGet:

```powershell
Install-Package FactoryFactory -Pre
```

A basic "Hello World" application might look something like this:

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
            Console.WriteLine($"Hello world, the time is {_clock.UtcNow}");
            Console.ReadLine();
        }
    }

    public interface IClock
    {
        DateTime UtcNow { get; }
    }

    public class Clock: IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
```

There are more complex approaches that involve separately creating modules and a
`Configuration` instance, but for many scenarios, the above simple approach
should suffice.
