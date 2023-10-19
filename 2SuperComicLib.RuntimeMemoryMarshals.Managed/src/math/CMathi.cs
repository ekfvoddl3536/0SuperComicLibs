// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
// Copyright (c) .NET Foundation and Contributors
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

namespace SuperComicLib
{
    /// <summary>
    /// Class to support <see langword="nint"/> and <see langword="nuint"/> of <see cref="CMath"/>
    /// </summary>
    public static unsafe class CMathi
    {
        #region native int
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t Min(this nint_t left, nint_t right) =>
            ((-(nint_t)ILUnsafe.ConvI4(right < left)) & (right - left)) + left;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t Max(this nint_t left, nint_t right) =>
            ((-(nint_t)ILUnsafe.ConvI4(right < left)) & (left - right)) + right;
        #endregion

        #region native uint
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t Min(this nuint_t left, nuint_t right)
        {
            var temp = left - right;
            return right + (temp & (nuint_t)((nint_t)temp >> ((sizeof(long) << 3) - 1)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t Max(this nuint_t left, nuint_t right)
        {
            var temp = left - right;
            return left - (temp & (nuint_t)((nint_t)temp >> ((sizeof(long) << 3) - 1)));
        }
        #endregion

        #region classic math
        /// <summary>
        /// Calculate in the classical way (using if).<para/>
        /// It will be possible to show higher performance in an environment where conditional branching can be optimized with CMOV.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t MinClassic(nint_t left, nint_t right) => left > right ? right : left;

        /// <summary>
        /// Calculate in the classical way (using if).<para/>
        /// It will be possible to show higher performance in an environment where conditional branching can be optimized with CMOV.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t MaxClassic(nint_t left, nint_t right) => left > right ? left : right;

        /// <summary>
        /// Calculate in the classical way (using if).<para/>
        /// It will be possible to show higher performance in an environment where conditional branching can be optimized with CMOV.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t MinClassic(nuint_t left, nuint_t right) => left > right ? right : left;

        /// <summary>
        /// Calculate in the classical way (using if).<para/>
        /// It will be possible to show higher performance in an environment where conditional branching can be optimized with CMOV.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint_t MaxClassic(nuint_t left, nuint_t right) => left > right ? left : right;
        #endregion

        #region compare
        /// <summary>
        /// Cross ExclusiveOr<para/>
        /// (<paramref name="left_p0"/> ^ <paramref name="right_p0"/>) |
        /// (<paramref name="left_v1"/> ^ <paramref name="right_v1"/>)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint_t CXOR(void* left_p0, void* right_p0, nint_t left_v1, nint_t right_v1) =>
            ((nint_t)left_p0 ^ (nint_t)right_p0) | (left_v1 ^ right_v1);
        #endregion
    }
}
