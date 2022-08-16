using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Collections
{
    internal static unsafe class RawContainerUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Internal_Clear<T>(T* iter, T* end) where T : unmanaged
        {
            while (iter != end)
                *iter++ = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Internal_IncreaseCapacity<T>(ref T* ptr, int prev_size, int want_size) where T : unmanaged
        {
            T* np = (T*)Marshal.AllocHGlobal(want_size * sizeof(T));

            int i;
            for (i = prev_size; --i >= 0;)
                np[i] = ptr[i];

            T def00 = default;
            for (i = want_size; --i >= prev_size;)
                np[i] = def00;

            Marshal.FreeHGlobal((IntPtr)ptr);
            ptr = np;
        }

        internal static int Internal_Earse<T>(_iterator<T> first, _iterator<T> last, _iterator<T> arrayEnd) where T : unmanaged
        {
            var iter = last + 1;

            var cnt = (uint)((T*)last.UnsafePointerValue - (T*)first.UnsafePointerValue);

            ulong sz = (ulong)cnt * (uint)sizeof(T);
            Buffer.MemoryCopy(iter.UnsafePointerValue, first.UnsafePointerValue, sz, sz);

            for (iter = arrayEnd - (int)cnt; iter != arrayEnd; iter++)
                iter.value = default;

            return (int)(arrayEnd - iter);
        }

        internal static void Internal_Earse_Single<T>(_iterator<T> position, _iterator<T> arrayEnd) where T : unmanaged
        {
            var iter = position + 1;

            ulong sz = (ulong)((T*)arrayEnd.UnsafePointerValue - (T*)iter.UnsafePointerValue) * (uint)sizeof(T);
            Buffer.MemoryCopy(iter.UnsafePointerValue, position.UnsafePointerValue, sz, sz);

            (arrayEnd - 1).value = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CheckVaildateAddress(void* start, void* end, void* item_begin, void* item_end)
        {
            if (start < item_begin || end >= item_end)
                throw new ArgumentOutOfRangeException();
        }
    }
}
