using System;

namespace SuperComicLib
{
    public static class CachedType<T>
    {
        public static readonly Type Value = typeof(T);
    }
}
