// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SuperComicLib.Core;

namespace SuperComicLib.Collections
{
    public static class Arrays
    {
        public static int HashCodeFast<T>(this T[] arr) where T : struct
        {
            if (arr == null)
                return 0;

            int x = arr.Length;
            int result = 7;
            while (--x >= 0)
                result = IntHash.CombineMOD(result, arr[x].GetHashCode());

            return result;
        }

        public static int HashCodeFast<T>(this IEnumerable<T> arr) where T : struct
        {
            if (arr == null)
                return 0;

            int result = 7;
            foreach (T v in arr)
                result = IntHash.CombineMOD(result, v.GetHashCode());

            return result;
        }

        public static int HashCode<T>(this T[] arr)
        {
            if (arr == null)
                return 0;

            int x = arr.Length;
            int result = 7;
            while (--x >= 0)
                result = IntHash.CombineMOD(result, arr[x]?.GetHashCode() ?? 0);

            return result;
        }

        public static int HashCode<T>(this IEnumerable<T> arr)
        {
            if (arr == null)
                return 0;

            int result = 7;
            foreach (T v in arr)
                result = IntHash.CombineMOD(result, v?.GetHashCode() ?? 0);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetNextSize(int oldSize)
        {
            const uint MaxArrayLength = 0x7FEF_FFFF;
            return (int)CMath.Min((uint)oldSize << 1, MaxArrayLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void QueueMoveNext(ref int index, int length)
        {
            int tmp = index + 1;
            if (tmp == length)
                tmp = 0;

            index = tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void QueueMovePrev(ref int index, int length)
        {
            int tmp = index - 1;
            if (tmp < 0)
                tmp += length;

            index = tmp;
        }
    }
}
