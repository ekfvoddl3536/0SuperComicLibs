namespace SuperComicLib.Reflection
{
    /// <summary>
    /// Access for CPU Registers
    /// </summary>
    public static unsafe class Register
    {
        /// <summary>
        /// Instruction Pointer. (x86: EIP, x64: RIP)
        /// </summary>
        public static readonly delegate*<nint> IP;
        /// <summary>
        /// Capture the stored values of registers.<para/>
        /// x86: eax, ebx, ecx, edx, esi, edi, esp, ebp<para/>
        /// x64: rax, rbx, rcx, rdx, rsi, rdi, rsp, rbp
        /// </summary>
        public static readonly delegate*<RegisterValue> Capture; // nax ~ nbp
        /// <summary>
        /// Capture the stored values of x64 extend registers.<para/>
        /// x86: eax, ebx, ecx, edx, esi, edi, esp, ebp<para/>
        /// x64: r8, r9, r10, r11, r12, r13, r14, r15
        /// </summary>
        public static readonly delegate*<RegisterValue> Capture64Ex; // r8 ~ r15 (x64 only)

        static Register()
        {
            nint v = CodeRegion.Alloc(128);

            if (sizeof(nint) == sizeof(int))
            {
                // mov eax, [esp]   ; 8b 04 24
                // sub eax, 0x02    ; 83 e8 07
                // ret              ; c3
                *(uint*)v = 0x83_24_04_8B;
                *(uint*)(v + 4) = 0x00_C3_02_E8;
                // 8bytes

                // mov [ecx], eax
                // mov [ecx+4], ebx
                // ...
                // mov [ecx+28], ebp
                // ret
                *(uint*)(v + 16) = 0x59_89_01_89;
                *(uint*)(v + 20) = 0x08_49_89_04;
                *(uint*)(v + 24) = 0x89_0C_51_89;
                *(uint*)(v + 28) = 0x79_89_10_71;
                *(uint*)(v + 32) = 0x18_61_89_14;
                *(uint*)(v + 36) = 0xC3_1C_69_89;

                Capture64Ex = (delegate*<RegisterValue>)(v + 0x10);
            }
            else
            {
                *(ulong*)v = 0x02_E8_83_48_24_04_8B_48;
                *(ulong*)(v + 8) = 0x00_00_C3_51_00_00_00_C3;
                // 16bytes

                // rax ~ rbp
                *(ulong*)(v + 0x10) = 0x48_08_59_89_48_01_89_48;
                *(ulong*)(v + 0x18) = 0x48_18_51_89_48_10_49_89;
                *(ulong*)(v + 0x20) = 0x48_28_79_89_48_20_71_89;
                *(ulong*)(v + 0x28) = 0xC3_38_69_89_48_30_61_89;

                // r8 ~ r15
                *(ulong*)(v + 0x40) = 0x4C_08_49_89_4C_01_89_4C;
                *(ulong*)(v + 0x48) = 0x4C_18_59_89_4C_10_51_89;
                *(ulong*)(v + 0x50) = 0x4C_28_69_89_4C_20_61_89;
                *(ulong*)(v + 0x58) = 0xC3_38_79_89_4C_30_71_89;

                Capture64Ex = (delegate*<RegisterValue>)(v + 0x40);
            }

            IP = (delegate*<nint>)v;
            Capture = (delegate*<RegisterValue>)(v + 0x10);
        }
    }
}
