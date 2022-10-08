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
    internal sealed class ByteComparer : FastComparer<byte>
    {
        public override bool EqualsAB(byte left, byte right) => left == right;

        public override bool NotEqualsAB(byte left, byte right) => left != right;

        public override bool Greater(byte left, byte right) => left > right;

        public override bool GreatOrEquals(byte left, byte right) => left >= right;

        public override bool Lesser(byte left, byte right) => left < right;

        public override bool LessOrEquals(byte left, byte right) => left <= right;
    }

    internal sealed class SByteComparer : FastComparer<sbyte>
    {
        public override bool EqualsAB(sbyte left, sbyte right) => left == right;

        public override bool NotEqualsAB(sbyte left, sbyte right) => left != right;

        public override bool Greater(sbyte left, sbyte right) => left > right;

        public override bool GreatOrEquals(sbyte left, sbyte right) => left >= right;

        public override bool Lesser(sbyte left, sbyte right) => left < right;

        public override bool LessOrEquals(sbyte left, sbyte right) => left <= right;
    }

    internal sealed class Int16Comparer : FastComparer<short>
    {
        public override bool EqualsAB(short left, short right) => left == right;

        public override bool NotEqualsAB(short left, short right) => left != right;

        public override bool Greater(short left, short right) => left > right;

        public override bool GreatOrEquals(short left, short right) => left >= right;

        public override bool Lesser(short left, short right) => left < right;

        public override bool LessOrEquals(short left, short right) => left <= right;
    }

    internal sealed class UInt16Comparer : FastComparer<ushort>
    {
        public override bool EqualsAB(ushort left, ushort right) => left == right;

        public override bool NotEqualsAB(ushort left, ushort right) => left != right;

        public override bool Greater(ushort left, ushort right) => left > right;

        public override bool GreatOrEquals(ushort left, ushort right) => left >= right;

        public override bool Lesser(ushort left, ushort right) => left < right;

        public override bool LessOrEquals(ushort left, ushort right) => left <= right;
    }

    internal sealed class Int32Comparer : FastComparer<int>
    {
        public override bool EqualsAB(int left, int right) => left == right;

        public override bool NotEqualsAB(int left, int right) => left != right;

        public override bool Greater(int left, int right) => left > right;

        public override bool GreatOrEquals(int left, int right) => left >= right;

        public override bool Lesser(int left, int right) => left < right;

        public override bool LessOrEquals(int left, int right) => left <= right;
    }

    internal sealed class UInt32Comparer : FastComparer<uint>
    {
        public override bool EqualsAB(uint left, uint right) => left == right;

        public override bool NotEqualsAB(uint left, uint right) => left != right;

        public override bool Greater(uint left, uint right) => left > right;

        public override bool GreatOrEquals(uint left, uint right) => left >= right;

        public override bool Lesser(uint left, uint right) => left < right;

        public override bool LessOrEquals(uint left, uint right) => left <= right;
    }

    internal sealed class Int64Comparer : FastComparer<long>
    {
        public override bool EqualsAB(long left, long right) => left == right;

        public override bool NotEqualsAB(long left, long right) => left != right;

        public override bool Greater(long left, long right) => left > right;

        public override bool GreatOrEquals(long left, long right) => left >= right;

        public override bool Lesser(long left, long right) => left < right;

        public override bool LessOrEquals(long left, long right) => left <= right;
    }

    internal sealed class UInt64Comparer : FastComparer<ulong>
    {
        public override bool EqualsAB(ulong left, ulong right) => left == right;

        public override bool NotEqualsAB(ulong left, ulong right) => left != right;

        public override bool Greater(ulong left, ulong right) => left > right;

        public override bool GreatOrEquals(ulong left, ulong right) => left >= right;

        public override bool Lesser(ulong left, ulong right) => left < right;

        public override bool LessOrEquals(ulong left, ulong right) => left <= right;
    }

    internal sealed class SingleComparer : FastComparer<float>
    {
        public override bool EqualsAB(float left, float right) => left == right;

        public override bool NotEqualsAB(float left, float right) => left != right;

        public override bool Greater(float left, float right) => left > right;

        public override bool GreatOrEquals(float left, float right) => left >= right;

        public override bool Lesser(float left, float right) => left < right;

        public override bool LessOrEquals(float left, float right) => left <= right;
    }

    internal sealed class DoubleComparer : FastComparer<double>
    {
        public override bool EqualsAB(double left, double right) => left == right;

        public override bool NotEqualsAB(double left, double right) => left != right;

        public override bool Greater(double left, double right) => left > right;

        public override bool GreatOrEquals(double left, double right) => left >= right;

        public override bool Lesser(double left, double right) => left < right;

        public override bool LessOrEquals(double left, double right) => left <= right;
    }
}
