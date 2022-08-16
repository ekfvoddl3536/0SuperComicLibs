#pragma warning disable IDE1006 // 명명 스타일
using System;
using System.Collections.Generic;

namespace SuperComicLib
{
    public static unsafe class SCL_GLOBAL_NativeSTLFunctionExtension_Iterators
    {
        public static _iterator<T> find<T>(this _iterator<T> v, _iterator<T> end, in T item) where T : unmanaged, IEquatable<T>
        {
            for (; v != end; v++)
                if (v.value.Equals(item))
                    return v;

            return default;
        }

        public static _iterator<T> find<T>(this _iterator<T> v, _iterator<T> end, in T item, IEqualityComparer<T> comparer) where T : unmanaged
        {
            for (; v != end; v++)
                if (comparer.Equals(v.value, item))
                    return v;

            return default;
        }

        public static _iterator<T> find_if<T>(this _iterator<T> v, _iterator<T> end, Predicate<T> match) where T : unmanaged
        {
            for (; v != end; v++)
                if (match.Invoke(v.value))
                    return v;

            return default;
        }

        public static reverse_iterator<T> find<T>(this reverse_iterator<T> v, reverse_iterator<T> end, in T item) where T : unmanaged, IEquatable<T>
        {
            for (; v != end; v++)
                if (v.value.Equals(item))
                    return v;

            return default;
        }

        public static reverse_iterator<T> find<T>(this reverse_iterator<T> v, reverse_iterator<T> end, in T item, IEqualityComparer<T> comparer) where T : unmanaged
        {
            for (; v != end; v++)
                if (comparer.Equals(v.value, item))
                    return v;

            return default;
        }

        public static reverse_iterator<T> find_if<T>(this reverse_iterator<T> v, reverse_iterator<T> end, Predicate<T> match) where T : unmanaged
        {
            for (; v != end; v++)
                if (match.Invoke(v.value))
                    return v;

            return default;
        }
    }
}
