// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace FactoryFactory.Tests.MicrosoftTests.Fakes
{
    public class FakeOuterService : IFakeOuterService
    {
        public FakeOuterService(
            IFakeService singleService,
            IEnumerable<IFakeMultipleService> multipleServices)
        {
            SingleService = singleService;
            MultipleServices = multipleServices;
        }

        public IFakeService SingleService { get; }

        public IEnumerable<IFakeMultipleService> MultipleServices { get; }
    }
}