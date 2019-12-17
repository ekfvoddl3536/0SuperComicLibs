using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.LowLevel
{
    internal static class NativeMehods0
    {
        private const string k = "kernel32.dll";

        [DllImport(k, SetLastError = true)]
        internal unsafe static extern void* VirtualAlloc(void* lpAddress, int dwSize, uint flAllocationType, uint flProtect);


        [DllImport(k, SetLastError = true)]
        internal unsafe static extern bool VirtualFree(void* lpAddress, int dwSize, uint dwFreeType);
    }
}
