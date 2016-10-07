using System;
namespace Brightstone
{
	public static class Util
    {
        public static int GetEnumCount<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                return -1;
            }
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}