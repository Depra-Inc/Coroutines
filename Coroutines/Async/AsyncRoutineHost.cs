using System.Collections;
using Depra.Coroutines.Domain.Entities;

namespace Depra.Coroutines.Async
{
	public sealed class AsyncRoutineHost : ICoroutineHost
	{
		private readonly ICoroutineProcessor _processor;

		public AsyncRoutineHost(ICoroutineProcessor processor) =>
			_processor = processor;

		public ICoroutine StartCoroutine(IEnumerator process) =>
			_processor.Process(process);

		public void StopCoroutine(ICoroutine coroutine) => 
			_processor.Stop(coroutine);
	}
}