using System;

namespace Depra.Coroutines.Domain.Exceptions
{
    public sealed class UnhandledCoroutineException : CoroutineException
    {
        private const string MESSAGE_FORMAT = "Unhandled Coroutine Exception";

        public UnhandledCoroutineException(Exception innerException) : 
            base(MESSAGE_FORMAT, innerException) { }
    }
}