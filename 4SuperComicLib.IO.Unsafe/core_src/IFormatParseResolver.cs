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

namespace SuperComicLib.IO
{
    public unsafe interface IFormatParseResolver<TResult, TLocalBuf> : IDisposable
        where TResult : unmanaged
        where TLocalBuf : unmanaged
    {
        /// <summary>
        /// [kr] byte 배열 생성에 사용되는 값, 한 번에 인코딩할 바이트 개수
        /// </summary>
        int EncodingBufferSize { get; }

        void SetNativeBuffer(char* source);

        /// <summary>
        /// [kr] 읽은 문자를 처리합니다
        /// </summary>
        /// <param name="readCount">[kr] (입력) 읽은 문자 수, (반환) 처리된 문자 수</param>
        ResolverState Push(int readCount, TLocalBuf* local_state, TResult* result);

        /// <returns>[kr] true 반환 시, stream 초기화(리셋)</returns>
        StreamStateControl OnEndOfStream(TLocalBuf* local_state);
    }
}
