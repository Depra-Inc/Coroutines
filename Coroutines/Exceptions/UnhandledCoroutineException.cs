// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Coroutines.Exceptions
{
	public sealed class UnhandledCoroutineException : CoroutineException
	{
		private const string MESSAGE_FORMAT = "Unhandled Coroutine Exception";

		public UnhandledCoroutineException(Exception innerException) :
			base(MESSAGE_FORMAT, innerException) { }
	}
}