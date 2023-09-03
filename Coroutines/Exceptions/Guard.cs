using System.Runtime.CompilerServices;

namespace Depra.Coroutines.Exceptions
{
	internal static class Guard
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Against(bool condition)
		{
			if (condition == false)
			{
				throw new AssertException();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Against(bool condition, string message)
		{
			if (condition == false)
			{
				throw new AssertException(message);
			}
		}
	}
}