using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Reflection
{
    /// <summary>
    /// [EN] Runtime Assembly helper class<para/>
    /// [KR] 런타임 어셈블리를 수행할 수 있는 클래스
    /// </summary>
    public static unsafe class RuntimeAsm
    {
        #region static field
        /// <summary>
        /// <see cref="Void">void</see> JmpHere(<see cref="IntPtr">nint</see> target_ip);<para/>
        /// </summary>
        public static readonly delegate*<nint, void> JmpHere;
        #endregion

        #region constructor
        static RuntimeAsm()
        {
            nint p = CodeRegion.Alloc(32);

            if (sizeof(nint) == sizeof(int))
            {
                // :jmp-here
                // mov eax, [esp]
                // mov byte ptr [ecx], 0xB8         ; code-inject(replace)
                // mov word ptr [ecx+5], 0xE0FF     ; code-inject(replace)
                // mov dword ptr [ecx+1], eax       ; code-inject(replace)
                // ret
                *(uint*)p = 0xC6_24_04_8B;
                *(uint*)(p + 4) = 0xC7_66_B8_01;
                *(uint*)(p + 8) = 0xE0_FF_05_41;
                *(uint*)(p + 12) = 0xC3_01_41_89;
            }
            else
            {
                *(ulong*)p = 0x48_01_C7_66_24_04_8B_48;
                *(ulong*)(p + 8) = 0x48_E0_FF_0A_41_C7_66_B8;
                *(ulong*)(p + 0x10) = 0xC3_02_41_89;
            }

            JmpHere = (delegate*<nint, void>)p;
        }
        #endregion

        #region methods
        /// <summary>
        /// Modify the return value of <paramref name="function_pointer"/> to a constant
        /// </summary>
        /// <param name="function_pointer">
        /// Modifiable safe function pointer with marked <see cref="MethodImplOptions.NoInlining"/><para/>
        /// Reference: <see cref="MethodImplAttribute"/>
        /// </param>
        /// <param name="return_value">value</param>
        public static void FixedReturnValue(delegate*<bool> function_pointer, bool return_value) =>
            *(uint*)function_pointer = 0xC3_58_00_6Au + (uint)(*(byte*)&return_value << 8); // x86, AMD64 호환

        /// <summary>
        /// Modifies the result value of the caller function to a constant<para/>
        /// Only methods marked with '<see cref="MethodImplOptions.NoInlining"/>' are supported
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void FixedReturnValue(bool return_value) =>
            FixedReturnValue((delegate*<bool>)new StackTrace().GetFrame(1)!.GetMethod()!.MethodHandle.GetFunctionPointer(), return_value);

        /// <summary>
        /// Get if the JIT execution environment is Release
        /// </summary>
        /// <returns>if JIT Release, returns TRUE</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool IsJITRelease()
        {
            bool result;

            var list = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(DebuggableAttribute), true);
            if (list == null || list.Length == 0)
                result = true;
            else
                result = ((int)((DebuggableAttribute)list[0])!.DebuggingFlags & 0x101) == 0;

            FixedReturnValue(&IsJITRelease, result);
            return result;
        }

        /// <summary>
        /// Copy instructions block
        /// </summary>
        /// <param name="src_ip"><see cref="Register.IP"/> address to start copying</param>
        /// <param name="dstArray">memory in which data will be stored</param>
        public static void CodeBlkCpy(nint src_ip, Span<byte> dstArray)
        {
            nint* srcPtr = (nint*)src_ip, dstPtr;
            byte* src_bp, dst_bp;

            fixed (byte* __dstp = dstArray)
            {
                dstPtr = (nint*)__dstp;

                for (int i = dstArray.Length >> ((sizeof(nint) + 6) >> 2); --i >= 0;)
                    *dstPtr++ = *srcPtr++;

                src_bp = (byte*)srcPtr;
                dst_bp = (byte*)dstPtr;
                for (int i = dstArray.Length & (sizeof(nint) - 1); --i >= 0;)
                    *dst_bp++ = *src_bp++;
            }
        }

        /// <summary>
        /// EN: inject new instructions into <paramref name="target_ip"/>, and back-up old code
        /// <para/>
        /// KR: 새 명령을 주입하고, 기존 명령은 백업합니다
        /// </summary>
        /// <param name="target_ip"><see cref="Register.IP"/> address where new instructions will be injected</param>
        /// <param name="new_codeBlk">new instructions block injected</param>
        /// <param name="old_codeBlk">buffer where old instructions block will be stored</param>
        /// <returns>size of the instructions that is actually injected</returns>
        public static int CodeInject_s(nint target_ip, ReadOnlySpan<byte> new_codeBlk, Span<byte> old_codeBlk)
        {
            nint* dstPtr = (nint*)target_ip, srcPtr, resPtr;
            byte* src_bp, dst_bp, res_bp;

            int safety_len = Math.Min(new_codeBlk.Length, old_codeBlk.Length);
            if (safety_len == 0)
                return 0;

            fixed (byte* __srcp = &new_codeBlk[0])
            fixed (byte* __resp = &old_codeBlk[0])
            {
                srcPtr = (nint*)__srcp;
                resPtr = (nint*)__resp;

                for (int i = safety_len >> ((sizeof(nint) + 6) >> 2); --i >= 0;)
                {
                    *resPtr++ = *dstPtr;
                    *dstPtr++ = *srcPtr++;
                }

                src_bp = (byte*)srcPtr;
                dst_bp = (byte*)dstPtr;
                res_bp = (byte*)resPtr;
                for (int i = safety_len & (sizeof(nint) - 1); --i >= 0;)
                {
                    *res_bp++ = *dst_bp;
                    *dst_bp++ = *src_bp++;
                }
            }

            return safety_len;
        }

        /// <summary>
        /// EN: inject new instructions into <paramref name="target_ip"/>, and back-up old code
        /// <para/>
        /// KR: 새 명령을 주입하고, 기존 명령은 백업합니다
        /// </summary>
        /// <param name="target_ip"><see cref="Register.IP"/> address where new instructions will be injected</param>
        /// <param name="value">new instructions block injected</param>
        /// <returns>old instructions block</returns>
        public static byte[] CodeInject_s(nint target_ip, ReadOnlySpan<byte> value)
        {
            byte[] res = new byte[value.Length];

            CodeInject_s(target_ip, value, res.AsSpan());

            return res;
        }

        /// <summary>
        /// Inject new instructions into <paramref name="target_ip"/>
        /// </summary>
        /// <param name="target_ip"><see cref="Register.IP"/> address where new instructions will be injected</param>
        /// <param name="value">new instructions blocks injected</param>
        public static void CodeInject(nint target_ip, ReadOnlySpan<byte> value)
        {
            nint* dstPtr = (nint*)target_ip, srcPtr;

            fixed (byte* __valuep = value)
            {
                srcPtr = (nint*)__valuep;

                for (int i = value.Length >> ((sizeof(nint) + 6) >> 2); --i >= 0;)
                    *dstPtr++ = *srcPtr++;

                byte* src_bp = (byte*)srcPtr;
                byte* dst_bp = (byte*)dstPtr;
                for (int i = value.Length & (sizeof(nint) - 1); --i >= 0;)
                    *dst_bp++ = *src_bp++;
            }
        }

        /// <summary>
        /// inject jmp instruction
        /// </summary>
        /// <param name="target_ip">target <see cref="Register.IP"/> address</param>
        /// <param name="jmp_address">absolute address</param>
        public static void JmpTo(nint target_ip, nint jmp_address)
        {
            if (sizeof(nint) == sizeof(int))
            {
                // mov eax, <ADDRESS>
                // jmp eax
                *(byte*)target_ip = 0xB8;
                *(nint*)(target_ip + 1) = jmp_address;
                *(ushort*)(target_ip + 5) = 0xE0_FF;
            }
            else
            {
                *(ushort*)target_ip = 0xB8_48;
                *(nint*)(target_ip + 2) = jmp_address;
                *(ushort*)(target_ip + 7) = 0xE0_FF;
            }
        }

        /// <summary>
        /// Inject new instructions into <paramref name="target_ip"/>
        /// </summary>
        /// <param name="target_ip"><see cref="Register.IP"/> address where new instructions will be injected</param>
        /// <param name="instruction">new 1-byte instruction</param>
        /// <returns>old 1-byte instruction</returns>
        public static byte CodeInject(nint target_ip, byte instruction)
        {
            byte res = *(byte*)target_ip;
            *(byte*)target_ip = instruction;
            return res;
        }

        /// <summary>
        /// Inject new instructions into <paramref name="target_ip"/>
        /// </summary>
        /// <param name="target_ip"><see cref="Register.IP"/> address where new instructions will be injected</param>
        /// <param name="instruction">new 2-byte instructions</param>
        /// <returns>old 2-byte instructions</returns>
        public static ushort CodeInject(nint target_ip, ushort instruction)
        {
            ushort res = *(ushort*)target_ip;
            *(ushort*)target_ip = instruction;
            return res;
        }

        /// <summary>
        /// Inject new instructions into <paramref name="target_ip"/>
        /// </summary>
        /// <param name="target_ip"><see cref="Register.IP"/> address where new instructions will be injected</param>
        /// <param name="instruction">new 4-byte instructions</param>
        /// <returns>old 4-byte instructions</returns>
        public static uint CodeInject(nint target_ip, uint instruction)
        {
            uint res = *(uint*)target_ip;
            *(uint*)target_ip = instruction;
            return res;
        }

        /// <summary>
        /// Inject new instructions into <paramref name="target_ip"/>
        /// </summary>
        /// <param name="target_ip"><see cref="Register.IP"/> address where new instructions will be injected</param>
        /// <param name="instruction">new 8-byte instructions</param>
        /// <returns>old 8-byte instructions</returns>
        public static ulong CodeInject(nint target_ip, ulong instruction)
        {
            ulong res = *(ulong*)target_ip;
            *(ulong*)target_ip = instruction;
            return res;
        }
        #endregion
    }
}
