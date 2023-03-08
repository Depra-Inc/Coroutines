// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;

namespace Depra.Coroutines.Domain.Entities
{
	public interface ICoroutineHost
	{
		ICoroutine StartCoroutine(IEnumerator process);

		void StopCoroutine(ICoroutine coroutine);
	}
}