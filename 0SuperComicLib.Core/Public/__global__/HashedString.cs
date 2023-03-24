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
    /// 잦은 해시 생성과 해시값 충돌을 피하기위해 64비트로 확장된 해시 문자열을 사용합니다.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct HashedString : IEquatable<HashedString>, IEquatable<string>
    {
        /// <summary>
        /// 저장된 해시의 값
        /// </summary>
        public readonly long Value;

        /// <summary>
        /// 지정된 문자열에 대한 64비트 확장 해시 코드를 생성합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashedString(string data) => 
            Value =
                data == null
                ? default
                : ((uint)data.GetFixedHashcode() | ((long)data.Length << 32));

        /// <summary>
        /// 지정된 값으로 초기화합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashedString(long value) => Value = value;

        /// <summary>
        /// 지정된 값으로 초기화합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HashedString(int hash1, int hash2) => Value = (uint)hash1 | ((long)hash2 << 32);

        /// <summary>
        /// 지정된 문자열을 <see cref="HashedString"/>로 변환합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator HashedString(string op) => new HashedString(op);
        /// <summary>
        /// 지정된 <see cref="HashedString"/>에 저장된 64비트 정수를 읽어서 64비트 정수형으로 변환합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(HashedString str) => str.Value;

        /// <summary>
        /// 하위 32비트 해시 코드를 가져옵니다.
        /// </summary>
        public int Hash1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (int)Value;
        }

        /// <summary>
        /// 상위 32비트 해시 코드를 가져옵니다.<para/>
        /// 이 값은 일반적으로 <see cref="string.Length"/> 입니다.
        /// </summary>
        public int Hash2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (int)(Value >> 32);
        }

        /// <summary>
        /// 대상 <see cref="HashedString"/>의 값과 일치하는지 비교합니다.
        /// </summary>
        public bool Equals(HashedString other) => Value == other.Value;
        /// <summary>
        /// 대상 <see cref="string"/>에 대한 <see cref="HashedString"/>의 값과 일치하는지 비교합니다.
        /// </summary>
        public bool Equals(string other) => this == other;

        /// <summary>
        /// <paramref name="obj"/>의 <see cref="object.ToString"/> 결과값에 대한 <see cref="HashedString"/>의 값과 일치하는지 비교합니다.
        /// </summary>
        public override bool Equals(object obj) => obj != null && this == new HashedString(obj.ToString());
        /// <summary>
        /// 64비트 정수에 대한 해시코드를 반환합니다.
        /// </summary>
        public override int GetHashCode() => Value.GetHashCode();
        /// <summary>
        /// 해시 코드를 16진수 값으로 변환하여 문자열로 만듭니다
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Hash2:x8}`{Hash1:x8}";

        /// <summary>
        /// 두 <see cref="HashedString"/>의 값이 같은지 비교합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(HashedString a1, HashedString a2) => a1.Value == a2.Value;
        /// <summary>
        /// 두 <see cref="HashedString"/>의 값이 다른지 비교합니다.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(HashedString a1, HashedString a2) => a1.Value != a2.Value;
    }
}