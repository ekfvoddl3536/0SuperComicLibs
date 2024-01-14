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

#pragma warning disable IDE1006
#pragma warning disable CS1591

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Runtime
{
    /// <summary>
    /// Provides functions that use special x86 or x64(amd64) instructions.
    /// </summary>
    public static class X86Function
    {
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct REG
        {
            public readonly long
                rax, 
                rbx, 
                rcx, 
                rdx;
        }

        /// <summary>
        /// <code>
        /// EAX = $eax
        /// CPUID
        /// </code>
        /// </summary>
        public static REG _CPUID(int eax) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <code>
        /// EAX = $eax
        /// ECX = $ecx
        /// CPUID
        /// </code>
        /// </summary>
        public static REG _CPUID(int eax, int ecx) => throw new PlatformNotSupportedException();

        /// <summary>
        /// <c>RAX = RSP - <see langword="sizeof"/>(<see cref="IntPtr"/>)</c>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr _STACKPOINTER() => throw new PlatformNotSupportedException();
    }
}
