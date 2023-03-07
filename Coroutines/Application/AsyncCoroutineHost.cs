using System.Collections;
using Depra.Coroutines.Domain.Entities;

namespace Depra.Coroutines.Application
{
	public sealed class AsyncCoroutineHost : ICoroutineHost
	{
		private readonly ICoroutineProcessor _processor;

		public AsyncCoroutineHost(ICoroutineProcessor processor) =>
			_processor = processor;

		public ICoroutine StartCoroutine(IEnumerator process) =>
			_processor.Process(process);

		public void StopCoroutine(ICoroutine coroutine) => 
			coroutine.Stop();
	}
}