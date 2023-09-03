// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Coroutines.Exceptions
{
	public sealed class AssertException : Exception
	{
		public AssertException(string message) : base(message) { }
	}
}