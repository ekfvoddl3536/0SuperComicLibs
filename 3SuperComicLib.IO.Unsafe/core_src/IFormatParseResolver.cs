using SuperComicLib.Collections;
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
