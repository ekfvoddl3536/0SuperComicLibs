using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.Reflection
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, int flNewProtect, out int lpflOldProtect);
    }
}
