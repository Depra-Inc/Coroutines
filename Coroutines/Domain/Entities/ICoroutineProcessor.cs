// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;

namespace Depra.Coroutines.Domain.Entities
{
    public interface ICoroutineProcessor
    {
        bool IsRunning { get; }
        
        void Tick();

        ICoroutine Process(IEnumerator process);
    }
}