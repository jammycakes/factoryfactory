﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace FactoryFactory.Tests.MicrosoftTests
{
    internal class TestServiceCollection : List<ServiceDescriptor>, IServiceCollection
    {
    }
}
