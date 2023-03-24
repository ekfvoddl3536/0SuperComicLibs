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
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    /// <summary>
    /// 잦은 해시 생성과 해시값 충돌을 피하기위해 128비트로 확장된 고해상도 해시 문자열을 사용합니다.<para/>
    /// <see cref="HashedString"/>과 호환되지 않습니다.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct LongHashedString : IEquatable<LongHashedString>
    {
        /// <summary>
        /// 하위 64비트 해시
        /// </summary>
        public readonly ulong Hash1EX;
        /// <summary>
        /// 상위 64비트 해시
        /// </summary>
        public readonly ulong Hash2EX;

        /// <summary>
        /// 지정된 문자열에 대한 128비트 확장 고해상도 해시 코드를 생성합니다.
        /// </summary>
        public LongHashedString(string data)
        {
            if (data == null)
                this = default;
            else
                Hash1EX = Hashcode128G1(data, out Hash2EX);
        }

        /// <summary>
        /// 기본 해시의 하위 32비트
        /// </summary>
        public int BHash => (int)Hash1EX;

        /// <summary>
        /// 기본 해시의 상위 32비트
        /// </summary>
        public int GHash => (int)(Hash1EX >> 32);

        /// <summary>
        /// 보조 해시 1번.
        /// </summary>
        public int Compare => (int)Hash2EX;

        /// <summary>
        /// 보조 해시 2번.
        /// </summary>
        public int Length => (int)(Hash2EX >> 32);

        /// <summary>
        /// 대상 <see cref="LongHashedString"/>의 값과 일치하는지 비교합니다.
        /// </summary>
        public bool Equals(LongHashedString other) => this == other;
        /// <summary>
        /// 대상 <see cref="string"/>에 대한 <see cref="LongHashedString"/>의 값과 일치하는지 비교합니다.
        /// </summary>
        public bool Equals(string other) => this == new LongHashedString(other);

        /// <summary>
        /// <paramref name="obj"/>의 <see cref="object.ToString"/> 결과값에 대한 <see cref="LongHashedString"/>의 값과 일치하는지 비교합니다.
        /// </summary>
        public override bool Equals(object obj) => obj != null && this == new LongHashedString(obj.ToString());
        /// <summary>
        /// 128비트 정수에 대한 해시코드를 반환합니다.
        /// </summary>
        public override int GetHashCode() => (Hash1EX ^ Hash2EX).GetHashCode();
        /// <summary>
        /// 해시 코드를 16진수 값으로 변환하여 문자열로 만듭니다
        /// </summary>
        /// <returns></returns>
        public unsafe override string ToString()
        {
            int hash1Lo = (int)Hash1EX,
                hash1Hi = (int)(Hash1EX >> 32),
                hash2Lo = (int)Hash2EX,
                hash2Hi = (int)(Hash2EX >> 32);

            return $"{hash2Hi:x8}`{hash2Lo:x8}`{hash1Hi:x8}`{hash1Lo:x8}";
        }

        /// <summary>
        /// 지정된 문자열을 <see cref="LongHashedString"/>로 변환합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LongHashedString(string op) => new LongHashedString(op);

        /// <summary>
        /// 두 <see cref="HashedString"/>의 값이 같은지 비교합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(LongHashedString a1, LongHashedString a2) => ((a1.Hash1EX ^ a2.Hash1EX) | (a1.Hash2EX ^ a2.Hash2EX)) == 0;
        /// <summary>
        /// 두 <see cref="HashedString"/>의 값이 다른지 비교합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(LongHashedString a1, LongHashedString a2) => !(a1 == a2);

        #region helper
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong Hashcode128G1(string data, out ulong r_hash2)
        {
            int comp = 0;

            int hash1 = SCL_GLOBAL_StringExtension.hashcode_start_c;
            int hash2 = hash1;

            fixed (char* src = data)
            {
                var pint = (int*)src;

                int len = data.Length;
                for (; len > 4; len -= 4, pint += 2)
                {
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];

                    comp = (hash2 - hash1) ^ comp;
                }

                if (len > 0)
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ *pint++;
            }

            r_hash2 = (uint)comp | ((ulong)data.Length << 32);
            return (uint)hash1 | ((ulong)hash2 << 32);
        }
        #endregion
    }
}