using System.Collections;

namespace Depra.Coroutines.Domain.Entities
{
	public interface ICoroutineHost
	{
		ICoroutine StartCoroutine(IEnumerator process);

		void StopCoroutine(ICoroutine coroutine);
	}
}