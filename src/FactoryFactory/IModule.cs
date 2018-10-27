using System;
using System.Collections.Generic;

namespace FactoryFactory
{
    public interface IModule
    {
        void Add(ServiceDefinition serviceDefinition);

        IEnumerable<ServiceDefinition> GetServiceDefinitions();
//
//        IEnumerable<ServiceDefinition> GetDefinitions(Type type);
//
//        bool IsTypeRegistered(Type type);
    }
}
