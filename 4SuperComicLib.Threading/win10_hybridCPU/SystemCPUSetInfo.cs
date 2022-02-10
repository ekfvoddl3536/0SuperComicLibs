using System.Runtime.InteropServices;

namespace SuperComicLib.Threading
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    internal readonly unsafe struct SystemCPUSetInfo // 32bytes
    {
        [FieldOffset(sizeof(int) * 3 + sizeof(short) + sizeof(byte) * 4)] // 18
        public readonly byte efficiencyClass;
        [FieldOffset(sizeof(int) * 4 + sizeof(short) + sizeof(byte) * 6)] // 24
        public readonly ulong __dummy__;

        // ref: https://docs.microsoft.com/ko-kr/windows/win32/api/winnt/ns-winnt-system_cpu_set_information
    }
}
