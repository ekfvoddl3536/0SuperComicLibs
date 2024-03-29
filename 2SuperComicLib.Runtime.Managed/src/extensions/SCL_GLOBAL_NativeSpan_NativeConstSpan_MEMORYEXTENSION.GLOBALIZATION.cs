﻿// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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
    static unsafe partial class SCL_GLOBAL_NativeSpan_NativeConstSpan_MEMORYEXTENSION
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhitespace(this NativeSpan<char> span) => IsWhitespace(span.Source, span.end()._ptr);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhitespace(this NativeConstSpan<char> span) => IsWhitespace(span._source, span.cend()._ptr);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SequenceEqual(this NativeSpan<char> span, string value) =>
            SequenceEqual(span.AsReadOnly(), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SequenceEqual(this NativeConstSpan<char> span, string value)
        {
            if (span.Length != (uint)value.Length)
                return false;

            char* pleft = span._source;
            for (int i = 0; i < value.Length; ++i)
                if (pleft[i] != value[i])
                    return false;

            return true;
        }

        #region private
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsWhitespace(char* iter, char* end)
        {
            for (; iter != end; ++iter)
                if (!char.IsWhiteSpace(*iter))
                    return false;

            return true;
        }
        #endregion
    }
}
