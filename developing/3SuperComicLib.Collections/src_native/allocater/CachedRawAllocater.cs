using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Collections
{
    public static unsafe class CachedRawAllocater<TAllocater> where TAllocater : unmanaged, IRawAllocater
    {
        public static readonly TAllocater item;
        public static readonly bool persistentAlloc = item is IRawPersistentAllocater;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr Alloc(int cb, bool initDefault) => item.stdAlloc(cb, initDefault);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(IntPtr ptr) => item.stdFree(ptr);
    }
}
