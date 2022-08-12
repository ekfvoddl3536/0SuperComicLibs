using SuperComicLib.Collections;
using System;
using System.Text;

namespace SuperComicLib.IO
{
    /// <summary>
    /// [kr] 다음과 같은 양식의 파일을 타겟으로 합니다.
    /// int int int int int int int int int int int int int int int int\r\n
    /// int int int int int int int int int int int int int int int int\r\n
    /// </summary>
    public unsafe class Int32x16TableResolver : IFormatParseResolver<Int32x16Table, int>
    {
        private string previous;
        private StringBuilder sb;
        private char* ptr;

        public int EncodingBufferSize => 16;

        public void SetNativeBuffer(char* source)
        {
            ptr = source;

            if (sb == null)
                sb = new StringBuilder(128);
        }

        #region parse
        public ResolverState Push(int readCount, int* local_state, Int32x16Table* result)
        {
            if (previous != null && PrevPush(local_state, result))
            {
                if (readCount != 0)
                    previous += new string(ptr, 0, readCount);

                return ResolverState.Success;
            }

            return OnPush(ptr, readCount, local_state, result);
        }

        private bool PrevPush(int* local_state, Int32x16Table* result)
        {
            string str = previous;
            previous = null;

            fixed (char* ptr_s = str)
            {
                var state = OnPush(ptr_s, str.Length, local_state, result);
                return state == ResolverState.Success;
            }
        }

        private ResolverState OnPush(char* c_ptr, int readCount, int* local_state, Int32x16Table* result)
        {
            var stream_ = new NativeCharStream(c_ptr);

            var sb_ = sb;

            int idx = 0;
            for (; ; )
            {
                idx = stream_.SkipWhiteSpaces(idx, readCount);
                int end_idx = stream_.IndexOfWhiteSpace(idx, readCount);

                string temp_str;
                if (sb_.Length > 0)
                {
                    if (readCount != 0 && idx == 0) // 이전과 연결된 숫자
                    {
                        sb_.Append(stream_._source, end_idx - idx);
                        idx = end_idx + 1;
                    }

                    temp_str = sb_.ToString();
                    sb_.Clear();
                }
                else
                {
                    if (idx == readCount)
                        break;
                    else if (end_idx == readCount)
                    {
                        sb_.Append(stream_ + idx, readCount - idx);
                        break;
                    }

                    temp_str = stream_.ToString(idx, readCount, end_idx - idx);
                    idx = end_idx + 1;
                }

                if (int.TryParse(temp_str, out *((int*)result + *local_state)) == false)
                    return ResolverState.Fail;

                (*local_state)++;
                if (*local_state == 16)
                {
                    if (idx != readCount)
                        previous = new string(stream_ + idx, 0, readCount - idx);

                    return ResolverState.Success;
                }
            }

            return ResolverState.Continue;
        }
        #endregion

        #region eos
        public StreamStateControl OnEndOfStream(int* local_state)
        {
            if (sb.Length > 0 || previous != null)
                return StreamStateControl.PushAndReturn;

            if (*local_state != 0)
                throw new InvalidOperationException("not support format");

            return StreamStateControl.None;
        }

        public void Dispose()
        {
            previous = null;
            sb = null;
            ptr = null;
        }
        #endregion
    }
}
