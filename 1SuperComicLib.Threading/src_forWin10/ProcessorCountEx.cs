// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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

using System.Runtime.CompilerServices;

namespace SuperComicLib.Threading
{
    public readonly struct ProcessorCountEx
    {
        private readonly int _littleCores;
        private readonly int _bigCores;

        internal ProcessorCountEx(int littleCores, int bigCores)
        {
            _littleCores = littleCores;
            _bigCores = bigCores;
        }

        public bool IsLittleCoresBeforeBigCores
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (uint)_littleCores >> 31 != 0;
        }

        public int LittleCores
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _littleCores & int.MaxValue;
        }

        public int BigCores
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _bigCores;
        }

        public bool IsHybrid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _bigCores > 0 && _littleCores > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AbsoluteLittleCoreIndex() => (_littleCores >> 31) & _littleCores & int.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AbsoluteBigCoreIndex() => ~(_littleCores >> 31) & _bigCores;

        /// <summary>
        /// 빅 코어(P-코어) 프로세서 인덱스를 전체 프로세서 기준의 원래 인덱스로 변환합니다.
        /// </summary>
        /// <param name="__relative__bigCores_index">변환할 빅 코어(P-코어) 프로세서 인덱스 입니다</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BigCoreIndex(int __relative__bigCores_index) =>
            __relative__bigCores_index + AbsoluteLittleCoreIndex();

        /// <summary>
        /// 리틀 코어(E-코어) 프로세서 인덱스를 전체 프로세서 기준의 원래 인덱스로 변환합니다.
        /// </summary>
        /// <param name="bigCores_index">변환할 리틀 코어(E-코어) 프로세서 인덱스 입니다</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LittleCoreIndex(int __relative__littleCores_index) =>
            __relative__littleCores_index + AbsoluteBigCoreIndex();

        /// <summary>
        /// <paramref name="mode"/> 값에 따른 유효 논리 프로세서 개수를 가져옵니다.
        /// </summary>
        /// <param name="mode">사용자 지정 <see cref="Preference"/> 열거형 값 입니다.</param>
        public int EffectiveCount(Preference mode) =>
            _littleCores * (((int)mode >> 1) & 1) +
            _bigCores * ((int)mode & 1);
    }
}