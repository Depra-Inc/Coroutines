// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Coroutines.Exceptions
{
	internal sealed class UnexpectedTypeFromCoroutine : Exception
	{
		private const string MESSAGE_FORMAT = "Unexpected type returned from coroutine! Expected '{0}' and found '{1}'!";

		public UnexpectedTypeFromCoroutine(string expected, string fact) :
			base(string.Format(MESSAGE_FORMAT, expected, fact)) { }
	}
}