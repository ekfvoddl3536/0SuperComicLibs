// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

//namespace SuperComicLib.Collections
//{
//    internal static unsafe class RawContainerUtility
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        internal static void Internal_Clear<T>(T* iter, T* end) where T : unmanaged
//        {
//            while (iter != end)
//                *iter++ = default;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        internal static void Internal_IncreaseCapacity<T>(ref T* ptr, int prev_size, int want_size) where T : unmanaged
//        {
//            T* np = (T*)Marshal.AllocHGlobal(want_size * sizeof(T));

//            int i;
//            for (i = prev_size; --i >= 0;)
//                np[i] = ptr[i];

//            T def00 = default;
//            for (i = want_size; --i >= prev_size;)
//                np[i] = def00;

//            Marshal.FreeHGlobal((IntPtr)ptr);
//            ptr = np;
//        }

//        internal static int Internal_Earse<T>(_iterator<T> first, _iterator<T> last, _iterator<T> arrayEnd) where T : unmanaged
//        {
//            var iter = last + 1;

//            var cnt = (uint)((T*)last - (T*)first);

//            ulong sz = (ulong)cnt * (uint)sizeof(T);
//            Buffer.MemoryCopy((T*)iter, (T*)first, sz, sz);

//            for (iter = arrayEnd - (int)cnt; iter != arrayEnd; iter++)
//                iter.value = default;

//            return (int)(arrayEnd - iter);
//        }

//        internal static void Internal_Earse_Single<T>(_iterator<T> position, _iterator<T> arrayEnd) where T : unmanaged
//        {
//            var iter = position + 1;

//            ulong sz = (ulong)((T*)arrayEnd - (T*)iter) * (uint)sizeof(T);
//            Buffer.MemoryCopy((T*)iter, (T*)position, sz, sz);

//            (arrayEnd - 1).value = default;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        internal static void CheckVaildateAddress(void* start, void* end, void* item_begin, void* item_end)
//        {
//            if (start < item_begin || end >= item_end)
//                throw new ArgumentOutOfRangeException();
//        }
//    }
//}
