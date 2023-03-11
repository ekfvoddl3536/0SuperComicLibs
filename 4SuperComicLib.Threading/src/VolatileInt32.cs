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

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SuperComicLib.Threading
{
    public struct VolatileInt32 : IEquatable<VolatileInt32>, IEquatable<int>, IComparable<VolatileInt32>, IComparable<int>
    {
        private volatile int m_value;

        public VolatileInt32(int value) =>
            m_value = value;

        #region get/set value
        public int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_value;
            set
            {
                for (SpinWait sw = default; ;)
                {
                    int observedValue = m_value;
                    if (Interlocked.CompareExchange(ref m_value, value, observedValue) != observedValue)
                        sw.SpinOnce();
                    else
                        break;
                }
            }
        }
        #endregion

        #region util
        public void Max(int other)
        {
            for (SpinWait sw = default; ;)
            {
                int observedValue = m_value;
                if (other > observedValue &&
                    Interlocked.CompareExchange(ref m_value, other, observedValue) != observedValue)
                    sw.SpinOnce();
                else
                    break;
            }
        }

        public void Min(int other)
        {
            for (SpinWait sw = default; ;)
            {
                int observedValue = m_value;
                if (other < observedValue &&
                    Interlocked.CompareExchange(ref m_value, other, observedValue) != observedValue)
                    sw.SpinOnce();
                else
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int value) => Interlocked.Add(ref m_value, value);

        /// <returns>변경되기 전 값</returns>
        public int CompareExchange(int value)
        {
            int observedValue = m_value;
            return Interlocked.CompareExchange(ref m_value, value, observedValue);
        }
        #endregion

        #region interface impl
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(VolatileInt32 other) => Equals(other.m_value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(int other) => m_value == other;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(VolatileInt32 other) => CompareTo(other.m_value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(int other) => m_value.CompareTo(other);
        #endregion

        #region override
        public override bool Equals(object obj) =>
            obj is int _ival && Equals(_ival) ||
            obj is VolatileInt32 _other && Equals(_other);

        public override int GetHashCode() => m_value.GetHashCode();

        public override string ToString() => m_value.ToString();
        #endregion

        #region implicit cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator VolatileInt32(int from) => new VolatileInt32(from);
        #endregion

        #region equals (operator)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in VolatileInt32 left, in VolatileInt32 right) => left.m_value == right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in VolatileInt32 left, in VolatileInt32 right) => left.m_value != right.m_value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(int left, in VolatileInt32 right) => left == right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(int left, in VolatileInt32 right) => left != right.m_value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in VolatileInt32 left, int right) => left.m_value == right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in VolatileInt32 left, int right) => left.m_value != right;
        #endregion

        #region compare (operator)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(in VolatileInt32 left, in VolatileInt32 right) => left.m_value < right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in VolatileInt32 left, in VolatileInt32 right) => left.m_value > right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in VolatileInt32 left, in VolatileInt32 right) => left.m_value <= right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in VolatileInt32 left, in VolatileInt32 right) => left.m_value >= right.m_value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(int left, in VolatileInt32 right) => left < right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(int left, in VolatileInt32 right) => left > right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(int left, in VolatileInt32 right) => left <= right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(int left, in VolatileInt32 right) => left >= right.m_value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(in VolatileInt32 left, int right) => left.m_value < right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in VolatileInt32 left, int right) => left.m_value > right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in VolatileInt32 left, int right) => left.m_value <= right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in VolatileInt32 left, int right) => left.m_value >= right;
        #endregion

        #region math operator
        #region + - * / %
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator +(in VolatileInt32 left, in VolatileInt32 right) => left.m_value + right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator -(in VolatileInt32 left, in VolatileInt32 right) => left.m_value - right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator *(in VolatileInt32 left, in VolatileInt32 right) => left.m_value * right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator /(in VolatileInt32 left, in VolatileInt32 right) => left.m_value / right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator %(in VolatileInt32 left, in VolatileInt32 right) => left.m_value % right.m_value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator +(int left, in VolatileInt32 right) => left + right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator -(int left, in VolatileInt32 right) => left - right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator *(int left, in VolatileInt32 right) => left * right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator /(int left, in VolatileInt32 right) => left / right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator %(int left, in VolatileInt32 right) => left % right.m_value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator +(in VolatileInt32 left, int right) => left.m_value + right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator -(in VolatileInt32 left, int right) => left.m_value - right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator *(in VolatileInt32 left, int right) => left.m_value * right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator /(in VolatileInt32 left, int right) => left.m_value / right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator %(in VolatileInt32 left, int right) => left.m_value % right;
        #endregion

        #region & | ^
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator &(in VolatileInt32 left, in VolatileInt32 right) => left.m_value & right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator |(in VolatileInt32 left, in VolatileInt32 right) => left.m_value | right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator ^(in VolatileInt32 left, in VolatileInt32 right) => left.m_value ^ right.m_value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator &(int left, in VolatileInt32 right) => left & right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator |(int left, in VolatileInt32 right) => left | right.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator ^(int left, in VolatileInt32 right) => left ^ right.m_value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator &(in VolatileInt32 left, int right) => left.m_value & right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator |(in VolatileInt32 left, int right) => left.m_value | right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator ^(in VolatileInt32 left, int right) => left.m_value ^ right;
        #endregion

        #region other
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator >>(in VolatileInt32 val, int shift) => val.m_value >> shift;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator <<(in VolatileInt32 val, int shift) => val.m_value << shift;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator ~(in VolatileInt32 val) => ~val.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator -(in VolatileInt32 val) => -val.m_value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator +(in VolatileInt32 val) => +val.m_value;
        #endregion
        #endregion
    }
}
