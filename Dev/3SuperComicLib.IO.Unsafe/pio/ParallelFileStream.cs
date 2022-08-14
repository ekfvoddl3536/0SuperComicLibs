using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SuperComicLib.IO.AdvancedParallel
{
    public unsafe class ParallelFileStream : IParallelFileReader
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

        public void ReadToEnd_internal(int bufferSizePerThread, int thread_count, ParallelReadHandler body)
        {
            var indices = IndexingReadToEnd_lineBased((uint)thread_count);

            var workers_ = new Action[thread_count];
            for (int i = 0; i < workers_.Length; i++)
                workers_[i] = new ParallelFileReadWorker(this, body, indices[i], bufferSizePerThread, i).Run;

            Marshal.FreeHGlobal((IntPtr)indices);

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = thread_count }, workers_);
        }

        private Range64* IndexingReadToEnd_byteBased(uint thread_count)
        {
            var fs = _baseStream;

            var block_sz = (fs.Length - fs.Position) / thread_count;

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

        private Range64* IndexingReadToEnd_lineBased(uint thread_count)
        {
            var fs = _baseStream;

            var stpos = fs.Position;
            var edpos = fs.Length;

            var block_sz = (edpos - stpos) / thread_count;

            var ranges = (Range64*)Marshal.AllocHGlobal(sizeof(Range64) * (int)thread_count);
            *ranges = new Range64(stpos, block_sz);
            ranges[thread_count - 1].end = edpos;

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

            fs.Seek(stpos, 0);

            return ranges;
        }
    }
}
