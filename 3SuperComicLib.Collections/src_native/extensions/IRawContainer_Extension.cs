using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Collections
{
    public static unsafe class IRawContainer_Extension
    {
        public static void CopyTo<T>(this IRawContainer<T> source, IRawContainer<T> dest) where T : unmanaged =>
            CopyTo(source, 0, dest, 0, source.size());

        public static void CopyTo<T>(this IRawContainer<T> source, int src_start_index, IRawContainer<T> dest, int dst_start_index, int count) 
            where T : unmanaged
        {
            if (source.size() - src_start_index < count ||
                dest.size() - dst_start_index < count)
                throw new InvalidOperationException("can't copy to dest. reason: 'overflow'");
            else if (count <= 0)
                return;
            else if (source == dest)
            {
                Internal_CopySelf(source, src_start_index, dst_start_index, count);
                return;
            }

            _iterator<T> src_iter = source.begin() + src_start_index;
            _iterator<T> dst_iter = dest.begin() + dst_start_index;

            for (; --count >= 0; dst_iter++, src_iter++)
                dst_iter.value = src_iter.value;
        }

        internal static void Internal_CopySelf<T>(this IRawContainer<T> self, int sidx, int didx, int count)
            where T : unmanaged
        {
            if (sidx == didx)
                return;

            _iterator<T> ss_iter = self.begin();
            _iterator<T> ds_iter = ss_iter;

            if (sidx < didx)
            {
                int tmp_i = count - 1;

                ss_iter += sidx + tmp_i;
                ds_iter += didx + tmp_i;

                for (; --count >= 0; ds_iter--, ss_iter--)
                    ds_iter.value = ss_iter.value;
            }
            else
            {
                ss_iter += sidx;
                ds_iter += didx;

                for (; --count >= 0; ds_iter++, ss_iter++)
                    ds_iter.value = ss_iter.value;
            }
        }

        public static T[] ToArray<T>(this IRawContainer<T> source) where T : unmanaged
        {
            int sz = source.size();
            T[] result = new T[sz];

            for (var r_iter = source.rbegin(); --sz >= 0; r_iter++)
                result[sz] = r_iter.value;

            return result;
        }

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

        public static int ToIndex<T>(this IRawContainer<T> source, _iterator<T> iterator) where T : unmanaged =>
            (int)(iterator - source.begin()) / sizeof(T);

        public static bool SequenceEqual<T>(this IRawContainer<T> first, IRawContainer<T> second) where T : unmanaged =>
            SequenceEqual(first, second, EqualityComparer<T>.Default);

        public static bool SequenceEqual<T>(this IRawContainer<T> first, IRawContainer<T> second, IEqualityComparer<T> comparer) where T : unmanaged
        {
            int size;
            if ((size = first.size()) == second.size())
            {
                while (--size >= 0)
                    if (!comparer.Equals(first[size], second[size]))
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
