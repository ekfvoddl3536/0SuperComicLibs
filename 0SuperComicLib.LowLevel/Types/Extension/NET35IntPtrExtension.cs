using System;

namespace SuperComicLib.LowLevel
{
    public static unsafe class NET35IntPtrExtension
    {
        public static IntPtr Add(this IntPtr ptr) => (IntPtr)((byte*)ptr + IntPtr.Size);

        public static IntPtr Add(this IntPtr ptr, int value) => (IntPtr)((byte*)ptr + IntPtr.Size * value);

        public static IntPtr Sub(this IntPtr ptr, int value) => Add(ptr, -value);
    }
}
