using System;

namespace SuperComicLib.LowLevel
{
    public static unsafe class NET35IntPtrExtension
    {
        public static IntPtr Add(this IntPtr ptr) => (IntPtr)((byte*)ptr + NativeClass.PtrSize_i);

        public static IntPtr Add(this IntPtr ptr, int value) => (IntPtr)((byte*)ptr + NativeClass.PtrSize_i * value);

        public static IntPtr Sub(this IntPtr ptr, int value) => Add(ptr, -value);
    }
}
