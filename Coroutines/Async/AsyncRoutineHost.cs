// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections;
using Depra.Coroutines.Entities;

namespace Depra.Coroutines.Async
{
	public sealed class AsyncRoutineHost : ICoroutineHost
	{
		private readonly ICoroutineProcessor _processor;

		public AsyncRoutineHost(ICoroutineProcessor processor) =>
			_processor = processor ?? throw new ArgumentNullException(nameof(processor));

		public ICoroutine StartCoroutine(IEnumerator process) =>
			_processor.Process(process);

		public void StopCoroutine(ICoroutine coroutine) =>
			_processor.Stop(coroutine);
	}
}