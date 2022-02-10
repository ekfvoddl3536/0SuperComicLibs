using System;

namespace SuperComicLib
{
    public static class CachedType<T>
    {
        public static readonly Type Value = typeof(T);
    }

    [Obsolete("use sizeof(T) instead of this class.", true)]
    public static unsafe class CachedSize<T> where T : unmanaged
    {
        [Obsolete("use sizeof(T) instaed of this field.", true)]
        public static readonly int Value = sizeof(T);
    }
}
