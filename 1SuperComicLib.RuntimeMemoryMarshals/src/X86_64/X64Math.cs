// The MIT License (MIT)
//
// Copyright (c) 2023-2024. SuperComic (ekfvoddl3535@naver.com)
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

#pragma warning disable CS1591
using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public static class X64Math
    {
        public static readonly bool IsCompiled = Init();

        // Generate x64 Assembly Code
        private static bool Init() => throw new PlatformNotSupportedException();

        /// <summary>
        /// Multiply High<para/>
        /// Returns the high 64 bits of the product of two 64-bit signed integers.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static long Mulhi(long a, long b) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Multiply High<para/>
        /// Returns the high 64 bits of the product of two 64-bit unsigned integers.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static ulong Mulhi(ulong a, ulong b) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Produces the full product of two 64-bit numbers.<para/>
        /// ref.: <see href="https://learn.microsoft.com/en-us/dotnet/api/system.math.bigmul?view=net-8.0#system-math-bigmul(system-int64-system-int64-system-int64@)">Math.BigMul(Int64, Int64, Int64)</see>
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static long BigMul(long a, long b, out long low) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Produces the full product of two 64-bit numbers.<para/>
        /// ref.: <see href="https://learn.microsoft.com/en-us/dotnet/api/system.math.bigmul?view=net-8.0#system-math-bigmul(system-uint64-system-uint64-system-uint64@)">Math.BigMul(Int64, Int64, Int64)</see>
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static ulong BigMul(ulong a, ulong b, out ulong low) => throw new PlatformNotSupportedException();
    }
}
