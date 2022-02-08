using System;

namespace SuperComicLib.Core
{
    public static unsafe class MemoryBlock
    {
        public static void Clear(IntPtr ptr, int size_bytes)
        {
            int ptr_sz = IntPtr.Size;

            byte* bp = (byte*)ptr;

            IntPtr zero = default;

            for (; size_bytes >= ptr_sz; size_bytes -= ptr_sz, bp += ptr_sz)
                *(IntPtr*)bp = zero;

            if (size_bytes > 0)
                *(IntPtr*)(bp - (ptr_sz - size_bytes)) = zero;
        }
    }
}
