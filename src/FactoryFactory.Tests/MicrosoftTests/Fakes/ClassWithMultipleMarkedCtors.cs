// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.Tests.MicrosoftTests.Fakes
{
    public class ClassWithMultipleMarkedCtors
    {
        [ActivatorUtilitiesConstructor]
        public ClassWithMultipleMarkedCtors(string data)
        {
        }

        [ActivatorUtilitiesConstructor]
        public ClassWithMultipleMarkedCtors(IFakeService service, string data)
        {
        }
    }
}