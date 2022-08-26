using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Collections
{
    public static unsafe class IRawContainer_Extension
    {
        public static void CopyTo<T>(this IRawContainer<T> source, IRawContainer<T> dest) where T : unmanaged => 
            new NativeSpan<T>(source.begin(), source.end()).CopyTo(new NativeSpan<T>(dest.begin(), dest.end()));

        public static void CopyTo<T>(this IRawContainer<T> source, int src_start_index, IRawContainer<T> dest, int dst_start_index, int count) 
            where T : unmanaged
        {
            var src = new NativeSpan<T>(source.begin() + src_start_index, source.end()).Slice(0, count);
            var dst = new NativeSpan<T>(dest.begin() + dst_start_index, dest.end());

            src.CopyTo(dst);
        }

        public static T[] ToArray<T>(this IRawContainer<T> source) where T : unmanaged => 
            new NativeSpan<T>(source.begin(), source.end()).ToArray();

        public static IEnumerator<T> GetEnumerator<T>(this IRawContainer<T> source) where T : unmanaged => new RawIteratorEnumerator<T>(source);

        public static _iterator<T> Find<T>(this IRawContainer<T> source, T item) where T : unmanaged =>
            Find(source, item, EqualityComparer<T>.Default);

        public static _iterator<T> Find<T>(this IRawContainer<T> source, T item, IEqualityComparer<T> comparer) where T : unmanaged
        {
            _iterator<T> cur = source.begin();
            _iterator<T> end = source.end();

            for (; cur != end; cur++)
                if (comparer.Equals(cur.value, item))
                    return cur;

            return default;
        }

        public static void* ToIndex<T>(this IRawContainer<T> source, _iterator<T> iterator) where T : unmanaged => iterator - source.begin();

        public static bool SequenceEqual<T>(this IRawContainer<T> first, IRawContainer<T> second) where T : unmanaged =>
            SequenceEqual(first, second, EqualityComparer<T>.Default);

        public static bool SequenceEqual<T>(this IRawContainer<T> first, IRawContainer<T> second, IEqualityComparer<T> comparer) where T : unmanaged
        {
            if (first.size() == second.size())
            {
                var p2 = second.begin();
                for (var iter = first.begin(); iter != first.end(); iter++, p2++)
                    if (!comparer.Equals(iter.value, p2.value))
                        return false;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<T> AsSpan<T>(this IRawContainer<T> source) where T : unmanaged => 
            new NativeSpan<T>(source.begin(), source.end());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeConstSpan<T> AsConstSpan<T>(this IReadOnlyRawContainer<T> source) where T : unmanaged => 
            new NativeConstSpan<T>(source.cbegin(), source.cend());
    }
}
