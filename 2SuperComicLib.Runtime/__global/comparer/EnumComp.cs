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

namespace SuperComicLib.Runtime
{
    internal sealed unsafe class EnumComparer<T> : FastComparer<T> where T : unmanaged
    {
        public override bool EqualsAB(T left, T right) => *(int*)&left == *(int*)&right;

        public override bool Greater(T left, T right) => *(int*)&left > *(int*)&right;

        public override bool GreatOrEquals(T left, T right) => *(int*)&left >= *(int*)&right;

        public override bool Lesser(T left, T right) => *(int*)&left < *(int*)&right;

        public override bool LessOrEquals(T left, T right) => *(int*)&left <= *(int*)&right;
    }

    internal sealed unsafe class UnsignedEnumComparer<T> : FastComparer<T> where T : unmanaged
    {
        public override bool EqualsAB(T left, T right) => *(uint*)&left == *(uint*)&right;

        public override bool Greater(T left, T right) => *(uint*)&left > *(uint*)&right;

        public override bool GreatOrEquals(T left, T right) => *(uint*)&left >= *(uint*)&right;

        public override bool Lesser(T left, T right) => *(uint*)&left < *(uint*)&right;

        public override bool LessOrEquals(T left, T right) => *(uint*)&left <= *(uint*)&right;
    }

    internal sealed unsafe class LongEnumComparer<T> : FastComparer<T> where T : unmanaged
    {
        public override bool EqualsAB(T left, T right) => *(long*)&left == *(long*)&right;

        public override bool Greater(T left, T right) => *(long*)&left > *(long*)&right;

        public override bool GreatOrEquals(T left, T right) => *(long*)&left >= *(long*)&right;

        public override bool Lesser(T left, T right) => *(long*)&left < *(long*)&right;

        public override bool LessOrEquals(T left, T right) => *(long*)&left <= *(long*)&right;
    }

    internal sealed unsafe class UnsignedLongEnumComparer<T> : FastComparer<T> where T : unmanaged
    {
        public override bool EqualsAB(T left, T right) => *(ulong*)&left == *(ulong*)&right;

        public override bool Greater(T left, T right) => *(ulong*)&left > *(ulong*)&right;

        public override bool GreatOrEquals(T left, T right) => *(ulong*)&left >= *(ulong*)&right;

        public override bool Lesser(T left, T right) => *(ulong*)&left < *(ulong*)&right;

        public override bool LessOrEquals(T left, T right) => *(ulong*)&left <= *(ulong*)&right;
    }
}
