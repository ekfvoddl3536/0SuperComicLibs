using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.Reflection
{
    /// <summary>
    /// Allocate new code block
    /// </summary>
    public static unsafe class CodeRegion
    {
        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(nint lpAddress, nint dwSize, int flNewProtect, out int lpflOldProtect);

        /// <summary>
        /// Allocate new code block
        /// </summary>
        /// <param name="cb">size of bytes</param>
        public static nint Alloc(int cb)
        {
            const int PAGE_EXECUTE_READWRITE = 0x40;

            nint v = Marshal.AllocHGlobal(cb);

            if (Environment.OSVersion.Platform < PlatformID.Unix)
                VirtualProtect(v, cb, PAGE_EXECUTE_READWRITE, out _);

            return v;
        }
    }
}
