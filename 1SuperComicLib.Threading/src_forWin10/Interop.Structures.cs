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
using System.Runtime.InteropServices;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.Threading
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    internal readonly unsafe ref struct SystemCPUSetInfo // 32bytes
    {
        [FieldOffset(sizeof(int) * 3 + sizeof(short) + sizeof(byte) * 4)] // 18
        public readonly byte efficiencyClass;

        [FieldOffset(sizeof(int) * 4 + sizeof(short) + sizeof(byte) * 6)] // 24
        public readonly ulong __reserved__; // struct size hint

        // ref: https://docs.microsoft.com/ko-kr/windows/win32/api/winnt/ns-winnt-system_cpu_set_information
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal readonly unsafe ref struct GROUP_AFFINITY // 16bytes
    {
        [FieldOffset(0)]
        public readonly UIntPtr mask;
        [FieldOffset(sizeof(long))]
        public readonly int group;          // 하위 ushort 구간만 사용

        [FieldOffset(sizeof(long) + sizeof(int))]
        public readonly int __reserved__;   // struct size hint

        // ref: https://docs.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-group_affinity

        public GROUP_AFFINITY(UIntPtr mask, int group)
        {
            FastContract.Requires((uint)group <= (uint)(Environment.ProcessorCount >> 6));

            this.mask = mask;
            this.group = group;
            __reserved__ = 0;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public readonly unsafe ref struct ThreadEntry32 // 28bytes? 강제로 32bytes 정렬
    {
        [FieldOffset(0)]
        public readonly uint dwSize;
        [FieldOffset(sizeof(int) * 2)]
        public readonly uint th32ThreadID;

        [FieldOffset(sizeof(int) * 6)]
        public readonly ulong __reserved__; // struct size hint. 하위 32비트는 dwFlags이나 사용되지 않음
    }
}
