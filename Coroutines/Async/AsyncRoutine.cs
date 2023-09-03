// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Depra.Coroutines.Entities;
using Depra.Coroutines.Exceptions;

namespace Depra.Coroutines.Async
{
	/// <summary>
	/// Wrapper class for <see cref="IEnumerator"/> objects.
	/// <remarks>
	/// This class is nice because it allows <see cref="IEnumerator"/>'s to return other <see cref="IEnumerator"/>'s just like Unity.
	/// </remarks>
	/// </summary>
	public sealed partial class AsyncRoutine : ICoroutine
	{
		private readonly List<IEnumerator> _finished = new();
		private readonly Stack<IEnumerator> _processStack = new();

		private object _returnValue;

		public AsyncRoutine(IEnumerator enumerator) =>
			_processStack.Push(enumerator);

		public object ReturnValue
		{
			get
			{
				Guard.Against(_processStack.Any() == false);
				return _returnValue;
			}
		}

		public bool IsDone => _processStack.Any() == false;

		/// <summary>
		/// Returns true if it needs to be called again.
		/// </summary>
		public bool Pump()
		{
			Guard.Against(_processStack.Any());
			Guard.Against(_returnValue == null);

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
					throw new UnhandledCoroutineException(exception);
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

		public void Stop()
		{
			_processStack.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static List<Type> GenerateObjectTrace(IEnumerable<IEnumerator> enumerators)
		{
			const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			var objTrace = new List<Type>();

			foreach (var enumerator in enumerators)
			{
				var type = enumerator.GetType();
				var field = type.GetField("<>4__this", BINDING_FLAGS);
				if (field == null)
				{
					// Mono seems to use a different name.
					field = type.GetField("<>f__this", BINDING_FLAGS);
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
	}
}