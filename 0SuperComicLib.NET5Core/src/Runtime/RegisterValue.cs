using System.Runtime.InteropServices;

namespace SuperComicLib.Runtime
{
    /// <summary>
    /// A ref structure that stores 8 register values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe ref struct RegisterValue
    {
        /// <summary>
        /// Native Accumulator Register (x86: eax, x64: rax)<para/>
        /// if x64 extend capture, R8 register
        /// </summary>
        public readonly nint nax;
        /// <summary>
        /// Native Base Register (x86: ebx, x64: rbx)<para/>
        /// if x64 extend capture, R9 register
        /// </summary>
        public readonly nint nbx;
        /// <summary>
        /// Native Count Register (x86: ecx, x64: rcx)<para/>
        /// if x64 extend capture, R10 register
        /// </summary>
        public readonly nint ncx;
        /// <summary>
        /// Native Data Register (x86: edx, x64: rdx)<para/>
        /// if x64 extend capture, R11 register
        /// </summary>
        public readonly nint ndx;

        /// <summary>
        /// Native Source Index (x86: esi, x64: rsi)<para/>
        /// if x64 extend capture, R12 register
        /// </summary>
        public readonly nint nsi;
        /// <summary>
        /// Native Destination Index (x86: edi, x64: rdi)<para/>
        /// if x64 extend capture, R13 register
        /// </summary>
        public readonly nint ndi;

        /// <summary>
        /// Native Stack Pointer (x86: esp, x64: rsp)<para/>
        /// if x64 extend capture, R14 register
        /// </summary>
        public readonly nint nsp;
        /// <summary>
        /// Native Base Pointer (x86: ebp, x64: rbp)<para/>
        /// if x64 extend capture, R15 register
        /// </summary>
        public readonly nint nbp;

#pragma warning disable CS1591
        public override string ToString() =>
            sizeof(nint) == sizeof(int)
            ? FormatString('E', "X8")
            : FormatString('R', "X16");
#pragma warning restore CS1591  // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.

        #region ToSring help private static method
        private string FormatString(char prefix, string format) =>
            $"[{Print(nax, $"{prefix}AX(R8)", format)}, {Print(nbx, $"{prefix}BX(R9)", format)}, " +
            $"{Print(ncx, $"{prefix}CX(R10)", format)}, {Print(ndx, $"{prefix}DX(R11)", format)}, " +
            $"{Print(nsi, $"{prefix}SI(R12)", format)}, {Print(ndi, $"{prefix}DI(R13)", format)}, " +
            $"{Print(nsp, $"{prefix}SP(R14)", format)}, {Print(nbp, $"{prefix}BP(R15)", format)}]";

        private static string Print(nint value, string name, string format) =>
            $"{name}: {value.ToString(format)}";
        #endregion
    }
}
