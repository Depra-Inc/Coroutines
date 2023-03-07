// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;

namespace Depra.Coroutines.Domain.Exceptions
{
    public sealed class AssertException : Exception
    {
        public AssertException(string message) : base(message) { }
    }
}