using System;

namespace SuperComicLib
{
    public static unsafe class SCL_GLOBAL_ArrayExtension_MemoryT
    {
        public static Memory<T> Slice<T>(this T[] source, int startIndex, int count)
        {
            if ((uint)(startIndex + count) > (uint)source.Length)
                throw new ArgumentNullException(nameof(startIndex));

            return new Memory<T>(source, startIndex, count);
        }
    }
}
