﻿// MIT License
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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SuperComicLib.CodeContracts
{
    /// <summary>
    /// .NET 5+ and .net framework compatibility
    /// </summary>
    public static class FastContract
    {
        [Conditional("DEBUG")]
        public static void Requires(bool conditional, string message)
        {
            if (!conditional)
            {
                Debug.Fail(message);
                throw new FastContractFailException(message);
            }
        }

        [Conditional("DEBUG")]
        public static void Requires(bool conditional)
        {
            if (!conditional)
            {
                Debug.Fail(nameof(FastContract) + "." + nameof(Requires) + "(bool) Fail. (Empty Message)");
                throw new FastContractFailException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Requires<TExcept>(bool conditional, string message) where TExcept : Exception, new()
        {
            if (!conditional)
                throw new FastContractFailException(message, new TExcept());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Requires<TExcept>(bool conditional) where TExcept : Exception, new()
        {
            if (!conditional)
                throw new FastContractFailException(string.Empty, new TExcept());
        }
    }
}