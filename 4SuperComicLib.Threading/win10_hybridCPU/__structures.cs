using System;
using System.Runtime.InteropServices;

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
            System.Diagnostics.Contracts.Contract.Requires((uint)group <= (uint)(Environment.ProcessorCount >> 6));

            this.mask = mask;
            this.group = group;
            __reserved__ = 0;
        }
    }
}
