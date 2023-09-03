// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.Coroutines.Entities
{
	public interface ICoroutine
	{
		bool IsDone { get; }

		void Stop();
	}
}