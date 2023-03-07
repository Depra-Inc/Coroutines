// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Depra.Coroutines.Domain.Entities;

namespace Depra.Coroutines.Application
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

        private void AdvanceFrameAll()
        {
            var currentNode = _workers.First;

            while (currentNode != null)
            {
                var next = currentNode.Next;
                var worker = currentNode.Value;

                try
                {
                    worker.Pump.Pump();
                    worker.IsFinished = worker.Pump.IsDone;
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

        private Coroutine ProcessInternal(IEnumerator process)
        {
            var data = new CoroutineInfo()
            {
                Pump = new CoroutinePump(process),
            };

            _newWorkers.Add(data);

            return new Coroutine(data.Pump, WaitUntilFinished(data));
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
            public CoroutinePump Pump;
        }
    }
}