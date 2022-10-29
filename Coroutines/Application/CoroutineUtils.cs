// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Depra.Coroutines.Domain.Exceptions;
using TimeoutException = Depra.Coroutines.Domain.Exceptions.TimeoutException;

namespace Depra.Coroutines.Application
{
    public static class CoroutineUtils
    {
        // This can be used to make an untyped coroutine typed
        // (it's nice sometimes to work with untyped coroutines so you can yield other coroutines)
        public static IEnumerator<T> Wrap<T>(IEnumerator runner)
        {
            var coroutine = new Coroutine(runner);

            while (coroutine.Pump())
            {
                yield return default(T);
            }

            if (coroutine.ReturnValue != null)
            {
                if (!(coroutine.ReturnValue is T))
                {
                    throw new AssertException(
                        $"Unexpected type returned from coroutine!  Expected '{typeof(T).Name}' and found '{coroutine.ReturnValue.GetType().Name}'");
                }
            }

            yield return (T) coroutine.ReturnValue;
        }

        // public static IEnumerator WaitSeconds(float seconds)
        // {
        //     float startTime = Time.realtimeSinceStartup;
        //
        //     while (Time.realtimeSinceStartup - startTime < seconds)
        //     {
        //         yield return null;
        //     }
        // }

        /// <summary>
        /// Block synchronously until the given coroutine completes
        /// </summary>
        public static void SyncWait(IEnumerator runner)
        {
            var coroutine = new Coroutine(runner);

            while (coroutine.Pump()) { }
        }

        /// <summary>
        /// Block synchronously until the given coroutine completes and give up after a timeout.
        /// </summary>
        public static void SyncWaitWithTimeout(IEnumerator runner, float timeout)
        {
            var startTime = DateTime.UtcNow;
            var coroutine = new Coroutine(runner);

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
            var coroutine = new Coroutine(runner);

            while (coroutine.Pump()) { }

            return (T) coroutine.ReturnValue;
        }

        public static T SyncWaitGet<T>(IEnumerator runner)
        {
            var coroutine = new Coroutine(runner);

            while (coroutine.Pump()) { }

            return (T) coroutine.ReturnValue;
        }

        /// <summary>
        /// Execute all the given coroutines in parallel.
        /// </summary>
        public static IEnumerator MakeParallelGroup(IEnumerable<IEnumerator> runners)
        {
            var runnerList = runners.Select(x => new Coroutine(x)).ToList();

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