// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

namespace Depra.Coroutines.Domain
{
    public interface ICoroutine
    {
        object ReturnValue { get; }
        
        bool IsDone { get; }
        
        /// <summary>
        /// Returns true if it needs to be called again.
        /// </summary>
        bool Pump();
    }
}