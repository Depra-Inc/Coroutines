// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

namespace Depra.Coroutines.Domain.Entities
{
    public interface ICoroutine
    {
	    bool IsDone { get; }

	    void Stop();
    }
}