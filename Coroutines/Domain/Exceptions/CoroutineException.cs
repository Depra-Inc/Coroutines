// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace Depra.Coroutines.Domain.Exceptions
{
    public class CoroutineException : Exception
    {
        private readonly List<Type> _objTrace;

        public IEnumerable<Type> ObjectTrace => _objTrace;

        public CoroutineException(List<Type> objTrace, Exception innerException) : base(CreateMessage(objTrace),
            innerException) => _objTrace = objTrace;

        private static string CreateMessage(List<Type> objTrace)
        {
            var result = new StringBuilder();

            foreach (var objType in objTrace)
            {
                if (result.Length != 0)
                {
                    result.Append(" -> ");
                }

                result.Append(objType.Name);
            }

            result.AppendLine();

            return "Coroutine Object Trace: " + result;
        }
    }
}