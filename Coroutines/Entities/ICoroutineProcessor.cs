// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections;

namespace Depra.Coroutines.Entities
{
	public interface ICoroutineProcessor
	{
		bool IsRunning { get; }

		void Tick();

		ICoroutine Process(IEnumerator process);

		void Stop(ICoroutine coroutine);
	}
}