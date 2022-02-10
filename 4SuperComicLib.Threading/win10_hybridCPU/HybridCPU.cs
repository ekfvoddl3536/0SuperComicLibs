﻿using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.Threading
{
    using static HybridCPU_NativeMethods;
    public unsafe static class HybridCPU
    {
        internal static ProcessorCountEx procInfo;
        internal static int headLittleCores;

        public static bool IsHybridProcessor
        {
            get
            {
                ref ProcessorCountEx _pinfo = ref procInfo;
                if ((_pinfo.littleCores | _pinfo.bigCores) == 0)
                    GetProcessorCount(ref _pinfo);

                return _pinfo.bigCores > 0 && _pinfo.littleCores > 0;
            }
        }

        public static ProcessorCountEx ProcessorCount
        {
            get
            {
                ref ProcessorCountEx _pinfo = ref procInfo;
                if ((_pinfo.littleCores | _pinfo.bigCores) == 0)
                    GetProcessorCount(ref _pinfo);

                return _pinfo;
            }
        }

        private static void GetProcessorCount(ref ProcessorCountEx result)
        {
            if (IsValidateOS() == false)
                goto f_exit;

            int* cores = stackalloc int[2];

            GetSystemCpuSetInformation(null, IntPtr.Zero, out IntPtr size, IntPtr.Zero, IntPtr.Zero);

            byte* pbuf = (byte*)Marshal.AllocHGlobal(size);
            if (pbuf == null)
                goto f_exit;

            GetSystemCpuSetInformation((SystemCPUSetInfo*)pbuf, size, out size, IntPtr.Zero, IntPtr.Zero);

            // 시작할 때 SystemCPUSetInfo->efficiencyClass 액세스가 가능하도록 미리조정
            // big.LITTLE 같은 1:1 구조만 지원

            for (byte* si = pbuf + 18, di = pbuf + (int)size; si <= di; si += sizeof(SystemCPUSetInfo))
                cores[*si & 1]++;

            // 0번째 cpu의 SystemCPUSetInfo->efficiencyClass가 0(효율 코어)인 경우
            // 리틀 코어가 빅 코어보다 앞에 위치함 (ARM big.LITTLE 구조)
            if (*(pbuf + 18) == 0)
                headLittleCores = 1;

            Marshal.FreeHGlobal((IntPtr)pbuf);

            result = new ProcessorCountEx(cores[0], cores[1]);
            return;

        f_exit:
            result = new ProcessorCountEx(Environment.ProcessorCount, 0);
        }

        private static bool IsValidateOS()
        {
            var os = Environment.OSVersion;
            return
                // runtime is x64
                // IntPtr.Size >= sizeof(long) &&
                // Windows 10 이상
                os.Platform == PlatformID.Win32NT && 
                os.Version.Major >= 10;
        }
    }
}