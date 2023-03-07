using System.Collections;
using Depra.Coroutines.Domain.Entities;

namespace Depra.Coroutines.Application
{
	public sealed class Coroutine : ICoroutine, IEnumerator
	{
		private readonly CoroutinePump _pump;
		private readonly IEnumerator _process;

		public Coroutine(CoroutinePump pump, IEnumerator process)
		{
			_pump = pump;
			_process = process;
		}

		public bool IsDone => _pump.IsDone;

		public object Current => _process.Current;

		public void Stop() => _pump.Stop();

		public bool MoveNext() => _process.MoveNext();

		public void Reset() => _process.Reset();
	}
}