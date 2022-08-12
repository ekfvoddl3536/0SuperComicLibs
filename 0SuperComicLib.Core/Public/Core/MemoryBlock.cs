using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.Core
{
    public static unsafe class MemoryBlock
    {
        public static void Clear(IntPtr ptr, int size_bytes)
        {
            byte* p1 = (byte*)ptr;

            for (int i = size_bytes >> 6; i-- > 0; p1 += sizeof(Block64))
                *(Block64*)p1 = default;

            if (IntPtr.Size == sizeof(int))
            {
                for (int i = (size_bytes & 0x3F) >> 2; i-- > 0; p1 += sizeof(uint))
                    *(uint*)p1 = 0;
            }
            else
            {
                for (int i = (size_bytes & 0x3F) >> 3; i-- > 0; p1 += sizeof(ulong))
                    *(ulong*)p1 = 0;

                if ((size_bytes & 4) != 0)
                {
                    *(uint*)p1 = 0;
                    p1 += sizeof(uint);
                }
            }

            if ((size_bytes & 2) != 0)
            {
                *(ushort*)p1 = 0;
                p1 += sizeof(ushort);
            }

            if ((size_bytes & 1) != 0)
                *p1 = 0;
        }

        [StructLayout(LayoutKind.Sequential, Size = 64)]
        private readonly ref struct Block64 { }
    }
}
