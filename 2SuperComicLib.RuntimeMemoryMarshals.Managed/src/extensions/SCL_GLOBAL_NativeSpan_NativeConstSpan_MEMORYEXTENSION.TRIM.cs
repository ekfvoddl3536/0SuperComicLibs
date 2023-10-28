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

namespace SuperComicLib
{
    // reference: https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/MemoryExtensions.Trim.cs
    public static unsafe partial class SCL_GLOBAL_NativeSpan_NativeConstSpan_MEMORYEXTENSION
    {
        #region string trim (trim char)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeConstSpan<char> TrimStart(this in NativeConstSpan<char> span, char trimChar) => span.Slice(ClampStart_char(span._source, span.cend()._ptr, trimChar));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeConstSpan<char> TrimEnd(this in NativeConstSpan<char> span, char trimChar) => span.Slice(0, ClampEnd_char(span.crbegin()._ptr, span._source - 1, trimChar));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeConstSpan<char> Trim(this in NativeConstSpan<char> span, char trimChar)
        {
            var res = span._source;
            var len = TrimCore_char(ref res, span.Length, trimChar);
            return new NativeConstSpan<char>(res, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<char> TrimStart(this in NativeSpan<char> span, char trimChar) => span.Slice(ClampStart_char(span.Source, span.end()._ptr, trimChar));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<char> TrimEnd(this in NativeSpan<char> span, char trimChar) => span.Slice(0, ClampEnd_char(span.rbegin()._ptr, span.Source - 1, trimChar));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<char> Trim(this in NativeSpan<char> span, char trimChar)
        {
            var res = span.Source;
            var len = TrimCore_char(ref res, span.Length, trimChar);
            return new NativeSpan<char>(res, len);
        }
        #endregion

        #region string trim (any whitespace)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeConstSpan<char> TrimStart(this in NativeConstSpan<char> span) => span.Slice(ClampStart(span._source, span.cend()._ptr));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeConstSpan<char> TrimEnd(this in NativeConstSpan<char> span) => span.Slice(0, ClampEnd(span.crbegin()._ptr, span._source - 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeConstSpan<char> Trim(this in NativeConstSpan<char> span)
        {
            var res = span._source;
            var len = TrimCore(ref res, span.Length);
            return new NativeConstSpan<char>(res, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<char> TrimStart(this in NativeSpan<char> span) => span.Slice(ClampStart(span.Source, span.end()._ptr));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<char> TrimEnd(this in NativeSpan<char> span) => span.Slice(0, ClampEnd(span.rbegin()._ptr, span.Source - 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeSpan<char> Trim(this in NativeSpan<char> span)
        {
            var res = span.Source;
            var len = TrimCore(ref res, span.Length);
            return new NativeSpan<char>(res, len);
        }
        #endregion

        #region string trim help (private)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ClampStart_char(char* begin, char* end, char trimChar)
        {
            int trimInt = trimChar | (trimChar << 16);

            var iter = (int*)begin;

            for (; iter != end; ++iter)
                if (*iter != trimInt)
                {
                    if (*(char*)iter == trimChar)
                        iter = (int*)((char*)iter + 1);

                    break;
                }

            return (char*)iter - begin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ClampEnd_char(char* rbegin, char* rend, char trimChar)
        {
            int trimInt = trimChar | (trimChar << 16);

            var iter = (int*)rbegin;

            for (; iter != rend; --iter)
                if (*iter != trimInt)
                {
                    if (*(char*)iter == trimChar)
                        iter = (int*)((char*)iter + 1);

                    break;
                }

            return (char*)iter - rend;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long TrimCore_char(ref char* inout_src, long in_length, char trimChar)
        {
            char* psi = inout_src;
            char* pdi = psi + in_length;

            var start = ClampStart_char(psi, pdi, trimChar);

            inout_src = psi + start;
            return ClampEnd_char(pdi - 1, inout_src - 1, trimChar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ClampStart(char* begin, char* end)
        {
            var iter = begin;

            for (; iter != end; ++iter)
                if (!char.IsWhiteSpace(*iter))
                    break;

            return iter - begin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ClampEnd(char* rbegin, char* rend)
        {
            var iter = rbegin;

            for (; iter != rend; --iter)
                if (!char.IsWhiteSpace(*iter))
                    break;

            return iter - rend;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long TrimCore(ref char* inout_src, long in_length)
        {
            char* psi = inout_src;
            char* pdi = psi + in_length;

            var start = ClampStart(psi, pdi);

            inout_src = psi + start;
            return ClampEnd(pdi - 1, inout_src - 1);
        }
        #endregion
    }
}
