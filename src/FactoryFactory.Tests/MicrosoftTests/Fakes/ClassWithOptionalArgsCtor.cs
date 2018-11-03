// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace FactoryFactory.Tests.MicrosoftTests.Fakes
{
    public class ClassWithOptionalArgsCtor
    {
        public ClassWithOptionalArgsCtor(string whatever = "BLARGH")
        {
            Whatever = whatever;
        }

        public string Whatever { get; set; }
    }
}
