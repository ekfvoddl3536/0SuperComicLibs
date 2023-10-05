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
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SuperComicLib.IO.AdvancedParallel
{
    public unsafe class ParallelFileStream : IParallelFileReader, IDisposable
    {
        private Stream _baseStream;
        private int _lock;

        public ParallelFileStream(Stream baseStream)
        {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));

            if (baseStream.CanSeek == false)
                throw new InvalidOperationException("can't seek");

            _baseStream = baseStream;
            _lock = 0;
        }

        #region property
        public Stream BaseStream => _baseStream;
        #endregion

        #region interface impl
        public int UpdateBuffer(byte[] buffer, ref Range64 offset)
        {
            var fs = _baseStream;

            for (SpinWait w = default; Interlocked.CompareExchange(ref _lock, 1, 0) != 0;)
                w.SpinOnce();

            int readCount = 0;
            try
            {
                fs.Seek(offset.start, 0);

                readCount = fs.Read(buffer, 0, buffer.Length);

                _lock = 0;
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                Debugger.Break();

                _lock = 0;

                offset.start = offset.end = -1; // invalidate (force break)
            }

            return readCount;
        }
        #endregion

        #region public methods (+helper)
        #region lineBaased
        public void ReadToEnd_LineBased(int bufferSizePerThread, int thread_count, ParallelReadHandler body)
        {
            if (bufferSizePerThread < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSizePerThread));

            if (thread_count < 2)
                throw new ArgumentOutOfRangeException(nameof(thread_count));

            if (body == null)
                throw new ArgumentNullException(nameof(body));

            var fs = _baseStream;
            var range = new Range64(fs.Position, fs.Length);

            bufferSizePerThread = (int)CMath.Max((uint)bufferSizePerThread, 128u);
            ReadToEnd_LineBased_internal(range, bufferSizePerThread, thread_count, body);
        }

        public void ReadToEnd_LineBased(in Range64 range, int bufferSizePerThread, int thread_count, ParallelReadHandler body)
        {
            if (range.Length < 0)
                throw new ArgumentOutOfRangeException(nameof(range));

            if (bufferSizePerThread < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSizePerThread));

            if (thread_count < 2)
                throw new ArgumentOutOfRangeException(nameof(thread_count));

            if (body == null)
                throw new ArgumentNullException(nameof(body));

            bufferSizePerThread = (int)CMath.Max((uint)bufferSizePerThread, 128u);
            ReadToEnd_LineBased_internal(range, bufferSizePerThread, thread_count, body);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadToEnd_LineBased_internal(in Range64 range, int bufferSizePerThread, int thread_count, ParallelReadHandler body)
        {
            var ranges = IndexingReadToEnd_lineBased(_baseStream, range, (uint)thread_count);
            var size = new Range(bufferSizePerThread, thread_count);

            CreateRunTaskParallel(ranges, body, size);
        }
        #endregion

        #region byteBased
        public void ReadToEnd(int bufferSizePerThread, int thread_count, ParallelReadHandler body)
        {
            if (bufferSizePerThread < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSizePerThread));

            if (thread_count < 2)
                throw new ArgumentOutOfRangeException(nameof(thread_count));

            if (body == null)
                throw new ArgumentNullException(nameof(body));

            var fs = _baseStream;
            var range = new Range64(fs.Position, fs.Length);

            bufferSizePerThread = (int)CMath.Max((uint)bufferSizePerThread, 128u);
            ReadToEnd_internal(range, bufferSizePerThread, thread_count, body);
        }

        public void ReadToEnd(in Range64 range, int bufferSizePerThread, int thread_count, ParallelReadHandler body)
        {
            if (range.Length < 0)
                throw new ArgumentOutOfRangeException(nameof(range));

            if (bufferSizePerThread < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSizePerThread));

            if (thread_count < 2)
                throw new ArgumentOutOfRangeException(nameof(thread_count));

            if (body == null)
                throw new ArgumentNullException(nameof(body));

            bufferSizePerThread = (int)CMath.Max((uint)bufferSizePerThread, 128u);
            ReadToEnd_internal(range, bufferSizePerThread, thread_count, body);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadToEnd_internal(in Range64 range, int bufferSizePerThread, int thread_count, ParallelReadHandler body)
        {
            var ranges = IndexingReadToEnd_byteBased(range, (uint)thread_count);
            var size = new Range(bufferSizePerThread, thread_count);

            CreateRunTaskParallel(ranges, body, size);
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateRunTaskParallel(Range64* ranges, ParallelReadHandler body, in Range size)
        {
            var workers_ = new Action[size.end];
            for (int i = 0; i < workers_.Length; i++)
                workers_[i] = new ParallelFileReadWorker(this, body, ranges[i], size.start, i).Run;

            Marshal.FreeHGlobal((IntPtr)ranges);

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = size.end }, workers_);
        }
        #endregion

        #region disposing
        ~ParallelFileStream()
        {
            _baseStream?.Dispose();
        }

        public void Dispose()
        {
            if (_baseStream != null)
            {
                _baseStream.Dispose();
                _baseStream = null;
            }
            GC.SuppressFinalize(this);
        }
        #endregion

        #region private static methods
        private static Range64* IndexingReadToEnd_byteBased(in Range64 range, uint thread_count)
        {
            var block_sz = range.Length / thread_count;

            var ranges = (Range64*)Marshal.AllocHGlobal(sizeof(Range64) * (int)thread_count);
            *ranges = new Range64(0, block_sz);

            for (uint i = 1; i < thread_count;)
            {
                ref Range64 curr = ref ranges[i];
                curr.start = ranges[i - 1].end;
                curr.end = block_sz * ++i;
            }

            return ranges;
        }

        private static Range64* IndexingReadToEnd_lineBased(Stream fs, in Range64 range, uint thread_count)
        {
            var block_sz = range.Length / thread_count;

            var ranges = (Range64*)Marshal.AllocHGlobal(sizeof(Range64) * (int)thread_count);
            *ranges = new Range64(range.start, block_sz);
            ranges[thread_count - 1].end = range.end;

            var buffer = new byte[128];

            fixed (byte* pbuf = &buffer[0])
                for (uint i = 1; i < thread_count; i++)
                {
                    var pos = fs.Seek(block_sz * i, 0);

                    for (; ; )
                    {
                        int rdcnt = fs.Read(buffer, 0, buffer.Length);
                        int nl_idx = ByteStreamHelper.IndexOfNewLine(fs, pbuf, rdcnt);

                        if (nl_idx < 0)
                            pos += buffer.LongLength;
                        else
                        {
                            pos += (uint)nl_idx;
                            break;
                        }
                    }

                    ranges[i - 1].end = pos;
                    ranges[i].start = pos;
                }

            fs.Seek(range.start, 0);

            return ranges;
        }
        #endregion
    }
}
