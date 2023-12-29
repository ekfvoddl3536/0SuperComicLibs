// MIT License
//
// Copyright (c) 2020-2023. SuperComic (ekfvoddl3535@naver.com)
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

namespace SuperComicLib.RuntimeMemoryMarshals
{
    /// <summary>
    /// Performs bitwise comparison of <b>any type</b>.
    /// </summary>
    /// <remarks>
    /// If <typeparamref name="T"/> is a reference type (e.g.: <see langword="class"/>), an address value comparison is performed.<br/>
    /// </remarks>
    public sealed unsafe class BitwiseComparer<T> : IComparer<T>
    {
        public readonly static IComparer<T> Default = Create();

        int IComparer<T>.Compare(T x, T y) => Compare(x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Compare(T x, T y)
        {
            if ((uint)ILUnsafe.SizeOf<T>() <= sizeof(long))
            {
                long dx = ILUnsafe.AsRef<T, long>(in x);
                long dy = ILUnsafe.AsRef<T, long>(in y);

                return (int)(dx - dy).Normal();
            }

            // zero-extend
            var sz = (long)(uint)ILUnsafe.SizeOf<T>();

            var p1 = (long*)((byte*)ILUnsafe.AsPointer(ref x) + sz);
            var p2 = (long*)((byte*)ILUnsafe.AsPointer(ref y) + sz);

            // (8 * 2) - 1
            long t0, t1, t2, t3;
            if ((uint)ILUnsafe.SizeOf<T>() >= 16)
                for (long ofs = 0, len = sz & -16L; ofs < len; ofs += 16)
                {
                    t0 = p1[ofs + 0];
                    t1 = p1[ofs + 1];

                    t2 = p2[ofs + 0];
                    t3 = p2[ofs + 1];

                    t0 -= t2;
                    t1 -= t3;

                    if (t0 != 0) goto T0_RET;
                    if (t1 != 0) goto T1_RET;
                }

            if (((uint)ILUnsafe.SizeOf<T>() & 15) != 0)
            {
                t0 = p1[sz - sizeof(long)];
                t1 = p2[sz - sizeof(long)];

                t2 = t0 - t1;
                if (t2 != 0) return (int)t2.Normal();
            }

            // hard-optimization
            goto JMP;

        T1_RET:
            // hard-optimization (prevent jmp instruction)
            t0 = t1;

        T0_RET:
            return (int)t0.Normal();

        JMP:
            return 0;
        }

        private static IComparer<T> Create() => new BitwiseComparer<T>();
    }
}
