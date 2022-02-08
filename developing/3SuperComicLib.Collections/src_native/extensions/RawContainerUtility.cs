using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Collections
{
    internal static unsafe class RawContainerUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Internal_IncreaseCapacity<Ty, Ta>(ref Ty* ptr, int prev_size, int want_size)
            where Ty : unmanaged
            where Ta : unmanaged, IRawAllocater
        {
            var item = CachedRawAllocater<Ta>.item;
            Ty* np = (Ty*)item.stdAlloc(want_size, false);

            int i;
            for (i = prev_size; --i >= 0;)
                np[i] = ptr[i];

            Ty def00 = default;
            for (i = want_size; --i >= prev_size;)
                np[i] = def00;

            item.stdFree((IntPtr)ptr);
            ptr = np;
        }

        internal static int Internal_Earse<T>(RawIterator<T> first, RawIterator<T> last, RawIterator<T> arrayEnd) where T : unmanaged
        {
            RawIterator<T> copy_src = last + 1, copy_dst = first;
            for (; copy_src != arrayEnd; copy_dst++, copy_src++)
                *copy_dst.Value = *copy_src.Value;

            T def00 = default;
            int earse_cnt = 0;
            for (; copy_dst != arrayEnd; copy_dst++, earse_cnt++)
                *copy_dst.Value = def00;

            return earse_cnt;
        }

        internal static void Internal_Earse_Single<T>(RawIterator<T> position, RawIterator<T> arrayEnd) where T : unmanaged
        {
            RawIterator<T> copy_src = position + 1;
            for (; copy_src != arrayEnd; position++, copy_src++)
                *position.Value = *copy_src.Value;

            *position.Value = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CheckVaildateAddress(void* start, void* end, void* item_begin, void* item_end)
        {
            if (start < item_begin || end >= item_end)
                throw new ArgumentOutOfRangeException();
        }
    }
}
