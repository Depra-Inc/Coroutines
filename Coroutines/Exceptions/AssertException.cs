// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Coroutines.Exceptions
{
	internal sealed class AssertException : Exception
	{
		private const string MESSAGE = "Assert hit in Coroutine!";

		public AssertException() : base(MESSAGE) { }

		public AssertException(string message) : base(MESSAGE + " " + message) { }
	}
}