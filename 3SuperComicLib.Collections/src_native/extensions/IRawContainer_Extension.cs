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

            RawIterator<T> src_iter = source.begin() + src_start_index;
            RawIterator<T> dst_iter = dest.begin() + dst_start_index;

            for (; --count >= 0; dst_iter++, src_iter++)
                *dst_iter.Value = *src_iter.Value;
        }

        internal static void Internal_CopySelf<T>(this IRawContainer<T> self, int sidx, int didx, int count)
            where T : unmanaged
        {
            if (sidx == didx)
                return;

            RawIterator<T> ss_iter = self.begin();
            RawIterator<T> ds_iter = ss_iter;

            if (sidx < didx)
            {
                int tmp_i = count - 1;

                ss_iter += sidx + tmp_i;
                ds_iter += didx + tmp_i;

                for (; --count >= 0; ds_iter--, ss_iter--)
                    *ds_iter.Value = *ss_iter.Value;
            }
            else
            {
                ss_iter += sidx;
                ds_iter += didx;

                for (; --count >= 0; ds_iter++, ss_iter++)
                    *ds_iter.Value = *ss_iter.Value;
            }
        }

        public static T[] ToArray<T>(this IRawContainer<T> source) where T : unmanaged
        {
            int sz = source.size();
            T[] result = new T[sz];

            for (RawReverseIterator<T> r_iter = source.rbegin(); --sz >= 0; r_iter++)
                result[sz] = *r_iter.Value;

            return result;
        }

        public static IEnumerator<T> GetEnumerator<T>(this IRawContainer<T> source) where T : unmanaged => new RawIteratorEnumerator<T>(source);

        public static RawIterator<T> Find<T>(this IRawContainer<T> source, T item) where T : unmanaged =>
            Find(source, item, EqualityComparer<T>.Default);

        public static RawIterator<T> Find<T>(this IRawContainer<T> source, T item, IEqualityComparer<T> comparer) where T : unmanaged
        {
            RawIterator<T> cur = source.begin();
            RawIterator<T> end = source.end();

            for (; cur != end; cur++)
                if (comparer.Equals(*cur.Value, item))
                    return cur;

            return default;
        }

        public static int ToIndex<T>(this IRawContainer<T> source, RawIterator<T> iterator) where T : unmanaged =>
            (int)(iterator.Value - source.begin().Value) / sizeof(T);

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
            new NativeSpan<T>(source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeConstSpan<T> AsConstSpan<T>(this IReadOnlyRawContainer<T> source) where T : unmanaged => 
            new NativeConstSpan<T>(source);
    }
}
