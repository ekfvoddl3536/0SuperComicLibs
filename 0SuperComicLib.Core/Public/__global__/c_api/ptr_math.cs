#pragma warning disable IDE1006 // 명명 스타일
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public static unsafe class ptr_math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t and(size_t a, size_t b) =>
#if AnyCPU
            sizeof(void*) == sizeof(int)
            ? (int)a.value & (int)b.value
            : (long)a.value & (long)b.value;
#else
            a.value & b.value;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t or(size_t a, size_t b) =>
#if AnyCPU
            sizeof(void*) == sizeof(int)
            ? (int)a.value | (int)b.value
            : (long)a.value | (long)b.value;
#else
            a.value | b.value;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t xor(size_t a, size_t b) =>
#if AnyCPU
            sizeof(void*) == sizeof(int)
            ? (int)a.value ^ (int)b.value
            : (long)a.value ^ (long)b.value;
#else
            a.value ^ b.value;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isNeg(size_t v) =>
#if AnyCPU
            sizeof(void*) == sizeof(int)
            ? (int)v.value < 0
            : (long)v.value < 0;
#elif X86
            (int)v.value < 0;
#else
            (long)v.value < 0;
#endif
    }
}
