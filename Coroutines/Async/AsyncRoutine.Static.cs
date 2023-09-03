// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Depra.Coroutines.Exceptions;
using TimeoutException = Depra.Coroutines.Exceptions.TimeoutException;

namespace Depra.Coroutines.Async
{
	public sealed partial class AsyncRoutine
	{
		// This can be used to make an untyped coroutine typed
		// (it's nice sometimes to work with untyped coroutines so you can yield other coroutines)
		public static IEnumerator<T> Wrap<T>(IEnumerator runner)
		{
			var coroutine = new AsyncRoutine(runner);

			while (coroutine.Pump())
			{
				yield return default;
			}

			if (coroutine.ReturnValue != null)
			{
				if (coroutine.ReturnValue is not T)
				{
					throw new UnexpectedTypeFromCoroutine(typeof(T).Name, coroutine.ReturnValue.GetType().Name);
				}
			}

			yield return (T) coroutine.ReturnValue;
		}

		/// <summary>
		/// Block synchronously until the given coroutine completes
		/// </summary>
		public static void SyncWait(IEnumerator runner)
		{
			var coroutine = new AsyncRoutine(runner);

			while (coroutine.Pump()) { }
		}

		/// <summary>
		/// Block synchronously until the given coroutine completes and give up after a timeout.
		/// </summary>
		public static void SyncWaitWithTimeout(IEnumerator runner, float timeout)
		{
			var startTime = DateTime.UtcNow;
			var coroutine = new AsyncRoutine(runner);

			while (coroutine.Pump())
			{
				if ((DateTime.UtcNow - startTime).TotalSeconds > timeout)
				{
					throw new TimeoutException();
				}
			}
		}

		public static T SyncWaitGet<T>(IEnumerator<T> runner)
		{
			var coroutine = new AsyncRoutine(runner);

			while (coroutine.Pump()) { }

			return (T) coroutine.ReturnValue;
		}

		public static T SyncWaitGet<T>(IEnumerator runner)
		{
			var coroutine = new AsyncRoutine(runner);

			while (coroutine.Pump()) { }

			return (T) coroutine.ReturnValue;
		}

		/// <summary>
		/// Execute all the given coroutines in parallel.
		/// </summary>
		public static IEnumerator MakeParallelGroup(IEnumerable<IEnumerator> runners)
		{
			var runnerList = runners.Select(x => new AsyncRoutine(x)).ToList();

			while (runnerList.Any())
			{
				foreach (var runner in runnerList)
				{
					runner.Pump();
				}

				foreach (var runner in runnerList.Where(x => x.IsDone).ToList())
				{
					runnerList.Remove(runner);
				}

				yield return null;
			}
		}
	}
}