---
title: "FactoryFactory: yet another IOC container for .NET"
sidebar: introduction
section: Developer guide
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
