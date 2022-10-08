// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
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
using System.Runtime.InteropServices;

namespace SuperComicLib.Threading
{

    internal unsafe static class HybridCPU_NativeMethods
    {
        private const string kl32 = "kernel32.dll";

        // LONG및 ULONG이 long 및 unsigned long 이라서 x86 에서는 int와 unsigned int 크기임 (즉, 가변 크기)
        // windows 10 이상 사용가능
        [DllImport(kl32)]
        public static extern bool GetSystemCpuSetInformation(
            [Out, Optional] SystemCPUSetInfo* Information,
            [In] IntPtr BufferLength,
            [Out] out IntPtr ReturnedLength,
            [In, Optional] IntPtr Process,
            IntPtr Flags);

        [DllImport(kl32)]
        public static extern UIntPtr SetThreadAffinityMask(
            [In] IntPtr hThread,
            [In] UIntPtr dwThreadAffinityMask);

        [DllImport(kl32)]
        public static extern IntPtr GetCurrentThread();

        [DllImport(kl32)]
        public static extern bool SetThreadGroupAffinity(
            [In] IntPtr hThread,
            [In] GROUP_AFFINITY* GroupAffinity,
            [Out, Optional] GROUP_AFFINITY* PreviousGroupAffinity);
    }
}
