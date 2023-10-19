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

using System.Runtime.CompilerServices;
using SuperComicLib.RuntimeMemoryMarshals;

namespace SuperComicLib.Runtime
{
    internal sealed unsafe class NativeComp<T> : LazyComparer<T>
    {
        public override int Compare(T x, T y)
        {
            var sz = Unsafe.SizeOf<T>();

            var p1 = (nint_t*)((byte*)Unsafe.AsPointer(ref x) + sz);
            var p2 = (nint_t*)((byte*)Unsafe.AsPointer(ref y) + sz);

            var shift = (sizeof(long) >> 2) + 1;
            for (var i = sz >> shift; i != 0; --i)
            {
                var cmp = *p1-- - *p2--;

                if (cmp != 0)
                    return 1 - (ILUnsafe.ConvI4(cmp < 0) << 1);
            }

            var p1b = (byte*)p1;
            var p2b = (byte*)p2;
            for (var i = sz & (sizeof(long) - 1); i != 0; --i)
            {
                var cmp = *p1b-- - *p2b--;

                if (cmp != 0)
                    return 1 - (ILUnsafe.ConvI4(cmp < 0) << 1);
            }

            return 0;
        }
    }
}
