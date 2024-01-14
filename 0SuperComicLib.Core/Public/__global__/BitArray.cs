// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib
{
    /// <summary>
    /// 비트열
    /// </summary>
    public readonly unsafe struct BitArray : IEquatable<BitArray>, ICloneable
    {
        /// <summary>
        /// 비트열 정보
        /// </summary>
        public readonly uint[] Values;

        /// <summary>
        /// 기존 비트열 정보를 사용해 복사본 생성
        /// </summary>
        public BitArray(BitArray other)
        {
            FastContract.Requires(other.Values != null);
            FastContract.Requires(other.Values.Length >= 0);

            Values = 
                other.Values.Length == 0
                ? other.Values
                : (uint[])other.Values.Clone();
        }

        /// <summary>
        /// 새로운 비트열 생성
        /// </summary>
        /// <param name="uintArray_size">초기 비트열 정보의 길이, 하나의 비트열 정보(uint)는 32개의 비트 길이를 갖고 있음</param>
        public BitArray(int uintArray_size)
        {
            FastContract.Requires(uintArray_size >= 0);

            Values = 
                uintArray_size == 0
                ? Array.Empty<uint>()
                : new uint[uintArray_size];
        }

        #region method 1
        /// <summary>
        /// 비트열 정보의 길이
        /// </summary>
        public int Length => Values.Length;

        /// <summary>
        /// 지정된 위치의 비트의 flag 여부를 가져오거나 설정
        /// </summary>
        public bool this[int bitPosition]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                uint temp = (Values[bitPosition >> 5] >> (bitPosition & 0x1F)) & 1;
                return *(bool*)&temp;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                ref uint k = ref Values[bitPosition >> 5];

                uint _set = 1u << (bitPosition & 0x1F);

                uint _val = *(byte*)&value - 1u;

                k &= ~_set & _val;
                k |= _set & ~_val;
            }
        }

        /// <summary>
        /// 모든 비트열 정보를 기본 값으로 초기화
        /// </summary>
        public void Clear() => Array.Clear(Values, 0, Values.Length);

        /// <summary>
        /// 모든 비트열의 값을 지정된 값으로 설정
        /// </summary>
        /// <param name="value"></param>
        public void SetAll(bool value)
        {
            uint val = (uint)-*(byte*)&value;

            var arr = Values;
            for (int i = arr.Length; --i >= 0;)
                arr[i] = val;
        }
        #endregion

        #region method 2
        /// <summary>
        /// <see cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource}, IEqualityComparer{TSource})"/>
        /// </summary>
        public bool Equals(BitArray other) => Values.SequenceEqual(other.Values, EqualityComparer<uint>.Default);

        object ICloneable.Clone() => new BitArray(this);
        #endregion

        #region override
        /// <summary>
        /// <see cref="object.GetHashCode"/>
        /// </summary>
        public override int GetHashCode() => Values.GetHashCode();

        /// <summary>
        /// <see cref="object.Equals(object)"/>
        /// </summary>
        public override bool Equals(object obj) =>
            obj is BitArray other && Equals(other);
        #endregion

        #region math operator
        /// <summary>
        /// 두 비트열을 AND 연산<para/>
        /// 더 작은 길이를 가진 비트열의 길이를 사용하여 결과 생성
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArray operator &(BitArray left, BitArray right)
        {
            var result = new BitArray((int)CMath.Max((uint)left.Values.Length, (uint)right.Values.Length));

            for (int i = 0; i < result.Values.Length; ++i)
                result.Values[i] = left.Values[i] & right.Values[i];

            return result;
        }

        /// <summary>
        /// 두 비트열을 OR 연산<para/>
        /// 더 작은 길이를 가진 비트열의 길이를 사용하여 결과 생성
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArray operator |(BitArray left, BitArray right)
        {
            var result = new BitArray((int)CMath.Max((uint)left.Values.Length, (uint)right.Values.Length));

            for (int i = 0; i < result.Values.Length; ++i)
                result.Values[i] = left.Values[i] | right.Values[i];

            return result;
        }

        /// <summary>
        /// 두 비트열을 XOR 연산<para/>
        /// 더 작은 길이를 가진 비트열의 길이를 사용하여 결과 생성
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArray operator ^(BitArray left, BitArray right)
        {
            var result = new BitArray((int)CMath.Max((uint)left.Values.Length, (uint)right.Values.Length));

            for (int i = 0; i < result.Values.Length; ++i)
                result.Values[i] = left.Values[i] ^ right.Values[i];

            return result;
        }

        /// <summary>
        /// 비트열을 NOT 연산
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArray operator ~(BitArray left)
        {
            var result = new BitArray(left.Values.Length);

            for (int i = 0; i < result.Values.Length; ++i)
                result.Values[i] = ~left.Values[i];

            return result;
        }

        /// <summary>
        /// 비트열을 RSHIFT 연산
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArray operator >>(BitArray left, int count) => RSHIFT_CORE(left, count);

        /// <summary>
        /// 비트열을 LSHIFT 연산
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitArray operator <<(BitArray left, int count) => LSHIFT_CORE(left, count);

        private static BitArray RSHIFT_CORE(BitArray left, int count)
        {
            int majorShift = (int)((uint)count >> 5);

            int majorCount = left.Values.Length - majorShift;
            if (majorCount <= 0)
                return new BitArray(0);

            var res = new BitArray(majorCount);

            Array.Copy(left.Values, majorShift, res.Values, 0, majorCount);

            int lowShift = count & 0x1F;    // 31
            int highShift = 0x20 - lowShift;    // 1

            var vres = res.Values;

            vres[0] >>= lowShift;

            for (int i = 1; i < vres.Length; ++i)
            {
                vres[i - 1] |= vres[i] << highShift;
                vres[i] >>= lowShift;
            }

            return res;
        }

        private static BitArray LSHIFT_CORE(BitArray left, int count)
        {
            int majorShift = (int)((uint)count >> 5);

            int majorCount = left.Values.Length - majorShift;
            if (majorCount <= 0)
                return new BitArray(0);

            var res = new BitArray(left.Values.Length);

            Array.Copy(left.Values, 0, res.Values, majorShift, majorCount);

            int lowShift = count & 0x1F;
            int highShift = 0x20 - lowShift;

            var vres = res.Values;

            uint tmp = 0;
            for (int i = majorShift; i < vres.Length; ++i)
            {
                var val = vres[i];

                vres[i] = (val << lowShift) | tmp;
                tmp = val >> highShift;
            }

            return res;
        }
        #endregion

        #region comparsion operator
        /// <summary>
        /// <see cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource}, IEqualityComparer{TSource})"/> == true
        /// </summary>
        public static bool operator ==(BitArray left, BitArray right) => left.Equals(right);
        /// <summary>
        /// <see cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource}, IEqualityComparer{TSource})"/> != true
        /// </summary>
        public static bool operator !=(BitArray left, BitArray right) => !left.Equals(right);
        #endregion
    }
}
