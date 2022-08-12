using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.XPatch
{
    internal static class NativeMethods
    {
        private const string kn32 = "kernel32.dll";

        [DllImport(kn32)]
        internal static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, int flNewProtect, out int lpflOldProtect);

        // [DllImport(kn32, CharSet = CharSet.Ansi)]
        // internal static extern IntPtr GetProcAddress(IntPtr hModule, IntPtr lpProcName);

        // [DllImport("Clrjit.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetJit")]
        // internal static extern IntPtr GetJitX64();
    }
}
