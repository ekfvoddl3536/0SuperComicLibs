﻿using System.Runtime.CompilerServices;

namespace SuperComicLib.Threading
{
    public readonly struct ProcessorCountEx
    {
        public readonly int littleCores;
        public readonly int bigCores;

        internal ProcessorCountEx(int littleCores, int bigCores)
        {
            this.littleCores = littleCores;
            this.bigCores = bigCores;
        }

        public bool IsHybrid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => bigCores > 0 && littleCores > 0;
        }

        /// <summary>
        /// 빅 코어(P-코어) 프로세서 인덱스를 전체 프로세서 기준의 원래 인덱스로 변환합니다.
        /// </summary>
        /// <param name="__relative__bigCores_index">변환할 빅 코어(P-코어) 프로세서 인덱스 입니다</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BigCoreIndex(int __relative__bigCores_index) =>
            __relative__bigCores_index + littleCores * HybridCPU.headLittleCores;

        /// <summary>
        /// 리틀 코어(E-코어) 프로세서 인덱스를 전체 프로세서 기준의 원래 인덱스로 변환합니다.
        /// </summary>
        /// <param name="bigCores_index">변환할 리틀 코어(E-코어) 프로세서 인덱스 입니다</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LittleCoreIndex(int __relative__littleCores_index) =>
            __relative__littleCores_index + bigCores * (HybridCPU.headLittleCores ^ 1);
    }
}