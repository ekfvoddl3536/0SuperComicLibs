#pragma warning disable IDE1006 // 명명 스타일
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public static unsafe class ptr_offset<T> where T : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* add(T* p, size_t n) =>
#if AnyCPU
            System.IntPtr.Size == sizeof(int)
            ? p + (uint)n.value
            : p + (ulong)n.value;
#else
        p + n.value;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* sub(T* p, size_t n) =>
#if AnyCPU
            System.IntPtr.Size == sizeof(int)
            ? p + (uint)n.value
            : p + (ulong)n.value;
#else
        p + n.value;
#endif
    }
}
