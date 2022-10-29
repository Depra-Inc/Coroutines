// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Depra.Coroutines.Domain;
using Depra.Coroutines.Domain.Exceptions;

namespace Depra.Coroutines.Application
{
    // Wrapper class for IEnumerator objects
    // This class is nice because it allows IEnumerator's to return other IEnumerator's just like Unity
    // We call it CoRoutine instead of Coroutine to differentiate it from UnityEngine.CoRoutine
    public class Coroutine : ICoroutine
    {
        private readonly Stack<IEnumerator> _processStack;
        private readonly List<IEnumerator> _finished = new List<IEnumerator>();

        private object _returnValue;

        public Coroutine(IEnumerator enumerator)
        {
            _processStack = new Stack<IEnumerator>();
            _processStack.Push(enumerator);
        }

        public object ReturnValue
        {
            get
            {
                Assert(_processStack.Any() == false);
                return _returnValue;
            }
        }

        public bool IsDone => _processStack.Any() == false;
        
        public bool Pump()
        {
            Assert(_processStack.Any());
            Assert(_returnValue == null);

            var topWorker = _processStack.Peek();

            bool isFinished;

            try
            {
                isFinished = topWorker.MoveNext() == false;
            }
            catch (CoroutineException e)
            {
                var objectTrace = GenerateObjectTrace(_finished.Concat(_processStack));
                if (objectTrace.Any() == false)
                {
                    throw;
                }

                throw new CoroutineException(objectTrace.Concat(e.ObjectTrace).ToList(), e.InnerException);
            }
            catch (Exception exception)
            {
                var objectTrace = GenerateObjectTrace(_finished.Concat(_processStack));
                if (objectTrace.Any() == false)
                {
                    throw;
                }

                throw new CoroutineException(objectTrace, exception);
            }

            if (isFinished)
            {
                _finished.Add(_processStack.Pop());
            }

            if (topWorker.Current is IEnumerator current)
            {
                _processStack.Push(current);
            }

            if (_processStack.Any() == false)
            {
                _returnValue = topWorker.Current;
            }

            return _processStack.Any();
        }

        private static List<Type> GenerateObjectTrace(IEnumerable<IEnumerator> enumerators)
        {
            var objTrace = new List<Type>();

            foreach (var enumerator in enumerators)
            {
                var field = enumerator.GetType().GetField("<>4__this",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                if (field == null)
                {
                    // Mono seems to use a different name
                    field = enumerator.GetType().GetField("<>f__this",
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    if (field == null)
                    {
                        continue;
                    }
                }

                var obj = field.GetValue(enumerator);
                if (obj == null)
                {
                    continue;
                }

                var objType = obj.GetType();
                if (objTrace.Any() == false || objType != objTrace.Last())
                {
                    objTrace.Add(objType);
                }
            }

            objTrace.Reverse();
            return objTrace;
        }

        private static void Assert(bool condition)
        {
            if (condition == false)
            {
                throw new AssertException("Assert hit in Coroutine!");
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (condition == false)
            {
                throw new AssertException("Assert hit in Coroutine! " + message);
            }
        }
    }
}