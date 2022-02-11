using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.Threading
{

    internal unsafe static class HybridCPU_NativeMethods
    {
        private const string kl32 = "kernel32.dll";

        // LONG및 ULONG이 long 및 unsigned long 이라서 x86 에서는 int와 unsigned int 크기임 (즉, 가변 크기)
        [DllImport(kl32)]
        public static extern bool GetSystemCpuSetInformation(
            [Out, Optional] SystemCPUSetInfo* Information,
            [In] IntPtr BufferLength,
            [Out] out IntPtr ReturnedLength,
            [In, Optional] IntPtr Process,
            IntPtr Flags);

        [DllImport(kl32)]
        public static extern UIntPtr SetThreadAffinityMask(
            [In] IntPtr hThread, 
            [In] UIntPtr dwThreadAffinityMask);

        [DllImport(kl32)]
        public static extern IntPtr GetCurrentThread();

        [DllImport(kl32)]
        public static extern bool SetThreadGroupAffinity(
            [In] IntPtr hThread,
            [In] GROUP_AFFINITY* GroupAffinity,
            [Out, Optional] GROUP_AFFINITY* PreviousGroupAffinity);
    }
}
