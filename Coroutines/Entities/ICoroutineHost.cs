// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections;

namespace Depra.Coroutines.Entities
{
	public interface ICoroutineHost
	{
		ICoroutine StartCoroutine(IEnumerator process);

		void StopCoroutine(ICoroutine coroutine);
	}
}