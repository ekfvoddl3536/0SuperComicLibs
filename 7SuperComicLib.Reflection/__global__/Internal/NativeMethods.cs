using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.Reflection
{
    internal static class NativeMethods
    {
        private const string kn32 = "kernel32.dll";

        [DllImport(kn32)]
        internal static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, int flNewProtect, out int lpflOldProtect);

        [DllImport(kn32, CharSet = CharSet.Ansi)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, IntPtr lpProcName);


        // [DllImport("Clrjit.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetJit")]
        // internal static extern IntPtr GetJitX64();

        internal static IntPtr GetProcAddress(IntPtr handle, string procName)
        {
            IntPtr proc = Marshal.StringToHGlobalAnsi(procName);
            try
            {
                return GetProcAddress(handle, proc);
            }
            finally
            {
                Marshal.FreeHGlobal(proc);
            }
        }
    }
}
