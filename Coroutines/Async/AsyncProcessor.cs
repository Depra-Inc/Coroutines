// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Depra.Coroutines.Domain.Entities;

namespace Depra.Coroutines.Async
{
    /// <summary>
    /// A container for running multiple routines in parallel. Coroutines can be nested.
    /// </summary>
    public sealed class AsyncProcessor : ICoroutineProcessor
    {
        private readonly List<CoroutineInfo> _newWorkers;
        private readonly LinkedList<CoroutineInfo> _workers;

        public AsyncProcessor()
        {
            _newWorkers = new List<CoroutineInfo>();
            _workers = new LinkedList<CoroutineInfo>();
        }

        public bool IsRunning => _workers.Any() || _newWorkers.Any();

        public void Tick()
        {
            AddNewWorkers();

            if (_workers.Any() == false)
            {
                return;
            }

            AdvanceFrameAll();
            AddNewWorkers();
        }

        public ICoroutine Process(IEnumerator process) =>
            ProcessInternal(process);

        public void Stop(ICoroutine coroutine)
        {
            coroutine.Stop();
            var worker = _workers.FirstOrDefault(x => x.Routine == coroutine);
            _workers.Remove(worker);
            _newWorkers.Remove(worker);
        }

        private void AdvanceFrameAll()
        {
            var currentNode = _workers.First;

            while (currentNode != null)
            {
                var next = currentNode.Next;
                var worker = currentNode.Value;

                try
                {
                    worker.Routine.Pump();
                    worker.IsFinished = worker.Routine.IsDone;
                }
                catch (Exception)
                {
                    worker.IsFinished = true;
                    throw;
                }

                if (worker.IsFinished)
                {
                    _workers.Remove(currentNode);
                }

                currentNode = next;
            }
        }

        private ICoroutine ProcessInternal(IEnumerator process)
        {
            var coroutine = new AsyncRoutine(process);
            var data = new CoroutineInfo {Routine = coroutine,};
            _newWorkers.Add(data);

            return coroutine;
        }

        private static IEnumerator WaitUntilFinished(CoroutineInfo workerData)
        {
            while (workerData.IsFinished == false)
            {
                yield return null;
            }
        }

        private void AddNewWorkers()
        {
            foreach (var worker in _newWorkers)
            {
                _workers.AddLast(worker);
            }

            _newWorkers.Clear();
        }

        private class CoroutineInfo
        {
            public bool IsFinished;
            public AsyncRoutine Routine;
        }
    }
}