// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
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

        /// <summary>
        /// <paramref name="mode"/> 값에 따른 유효 논리 프로세서 개수를 가져옵니다.
        /// </summary>
        /// <param name="mode">사용자 지정 <see cref="Preference"/> 열거형 값 입니다.</param>
        public static int EffectiveProcessorCount(Preference mode)
        {
            ref ProcessorCountEx _pinfo = ref procInfo;
            if ((_pinfo.littleCores | _pinfo.bigCores) == 0)
                GetProcessorCount(ref _pinfo);

            return _pinfo.EffectiveCount(mode);
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

            const int SYSTEMCPUINFO_SZ = 32;
            for (byte* si = pbuf + 18, di = pbuf + (int)size; si <= di; si += SYSTEMCPUINFO_SZ)
                cores[*si & 1]++;

            // 0번째 cpu의 SystemCPUSetInfo->efficiencyClass가 0(효율 코어)인 경우
            // 리틀 코어가 빅 코어보다 앞에 위치함 (ARM big.LITTLE 구조)
            if (*(pbuf + 18) == 0)
                headLittleCores = 1;

            Marshal.FreeHGlobal((IntPtr)pbuf);

            result = new ProcessorCountEx(cores[0], cores[1]);
            return;

        f_exit:
            result = new ProcessorCountEx(0, Environment.ProcessorCount);
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