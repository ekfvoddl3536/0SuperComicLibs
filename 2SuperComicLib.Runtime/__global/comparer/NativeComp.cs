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

using SuperComicLib.LowLevel;
using System;

namespace SuperComicLib.Runtime
{
    internal sealed class NativeComp<T> : FastComparer<T>
        where T : struct
    {
        public override bool EqualsAB(T left, T right) => NativeClass.CompareTo(ref left, ref right) == 0;

        public override bool Greater(T left, T right) => NativeClass.CompareTo(ref left, ref right) > 0;

        public override bool GreatOrEquals(T left, T right) => NativeClass.CompareTo(ref left, ref right) >= 0;

        public override bool Lesser(T left, T right) => NativeClass.CompareTo(ref left, ref right) < 0;

        public override bool LessOrEquals(T left, T right) => NativeClass.CompareTo(ref left, ref right) <= 0;
    }

    internal sealed class ObjectNativeComparer<T> : FastComparer<T>
    {
        public override bool EqualsAB(T left, T right) => ReferenceEquals(left, right);

        public override bool Greater(T left, T right) => NativeClass.ReferenceCompare(left, right) > 0;

        public override bool GreatOrEquals(T left, T right) => NativeClass.ReferenceCompare(left, right) >= 0;

        public override bool Lesser(T left, T right) => NativeClass.ReferenceCompare(left, right) < 0;

        public override bool LessOrEquals(T left, T right) => NativeClass.ReferenceCompare(left, right) <= 0;
    }

    internal sealed class NullableNativeComparer<T> : FastComparer<T?>
        where T : struct
    {
        public override bool EqualsAB(T? left, T? right) =>
            left.HasValue
            ? right.HasValue && NativeClass.CompareTo(left.Value, right.Value) == 0
            : right.HasValue == false;

        public override bool Greater(T? left, T? right) =>
            left.HasValue
            ? right.HasValue && NativeClass.CompareTo(left.Value, right.Value) > 0
            : right.HasValue == false;

        public override bool GreatOrEquals(T? left, T? right) =>
            left.HasValue
            ? right.HasValue && NativeClass.CompareTo(left.Value, right.Value) >= 0
            : right.HasValue == false;

        public override bool Lesser(T? left, T? right) =>
            left.HasValue
            ? right.HasValue && NativeClass.CompareTo(left.Value, right.Value) < 0
            : right.HasValue == false;

        public override bool LessOrEquals(T? left, T? right) =>
            left.HasValue
            ? right.HasValue && NativeClass.CompareTo(left.Value, right.Value) <= 0
            : right.HasValue == false;
    }
}
