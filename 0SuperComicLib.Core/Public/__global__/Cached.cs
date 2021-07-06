using System;

namespace SuperComicLib
{
    public static class CachedType<T>
    {
        public static readonly Type Value = typeof(T);
    }

    public static unsafe class CachedSize<T> where T : unmanaged
    {
        public static readonly int Value = sizeof(T);
    }
}
