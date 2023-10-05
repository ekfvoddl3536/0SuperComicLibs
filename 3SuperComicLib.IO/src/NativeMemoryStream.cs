// MIT License
//
// Copyright (c) 2020-2023. SuperComic (ekfvoddl3535@naver.com)
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
using System.Threading;
using System.Threading.Tasks;
using SuperComicLib.RuntimeMemoryMarshals;

namespace SuperComicLib.IO
{
    // ref. https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/IO/MemoryStream.cs
    public unsafe class NativeMemoryStream : Stream
    {
        protected const StreamOption
            IsOpen = StreamOption.RESERVED_01,
            Expandable = StreamOption.RESERVED_02;

        protected const int ARRAY_MAXLENGTH = 0x7FFF_FFC7;

        protected arrayref<byte> m_buffer;
        protected int m_origin;
        protected int m_position;
        protected int m_length;
        protected int m_capacity;
        protected StreamOption m_flags;

        protected Task<int> m_lastReadTask;

        #region constructors
        public NativeMemoryStream() : this(0)
        {
        }

        public NativeMemoryStream(int capacity)
        {
            const StreamOption DEFAULT_SET = StreamOption.Writable | StreamOption.PublicVisibile | IsOpen | Expandable;

            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            m_buffer =
                capacity != 0
                ? new arrayref<byte>(capacity)
                : default;

            m_capacity = capacity;
            m_flags = DEFAULT_SET;
        }

        public NativeMemoryStream(in arrayref<byte> buffer, StreamOption option)
        {
            if (buffer.IsNull)
                throw new ArgumentNullException(nameof(buffer));

            m_buffer = buffer;
            m_length = m_capacity = buffer.Length;

            m_flags = (option & StreamOption.ALL) | IsOpen;
        }

        public NativeMemoryStream(in arrayrefSegment<byte> buffer, StreamOption option)
        {
            if (buffer.IsNull)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.IsIndexOutOfRange)
                throw new ArgumentOutOfRangeException(nameof(buffer));

            m_buffer = buffer._source;
            m_origin = m_position = buffer._start;
            m_length = m_capacity = buffer._start + buffer.Length;

            m_flags = (option & StreamOption.ALL) | IsOpen;
        }
        #endregion

        #region properties
        public override bool CanRead => m_flags.HasFlag(IsOpen);
        public override bool CanSeek => m_flags.HasFlag(IsOpen);
        public override bool CanWrite => m_flags.HasFlag(StreamOption.Writable);

        public override long Length
        {
            get
            {
                EnsureNotClosed();
                return m_length - m_origin;
            }
        }

        public override long Position
        {
            get
            {
                EnsureNotClosed();
                return m_position - m_origin;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                EnsureNotClosed();

                if (value > int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), "TooLong");

                m_position = m_origin + (int)value;
            }
        }

        public virtual int Capacity
        {
            get
            {
                EnsureNotClosed();
                return m_capacity - m_origin;
            }
            set
            {
                if (value < Length)
                    throw new ArgumentOutOfRangeException(nameof(value), "SmallCapacity");

                EnsureNotClosed();

                if (!m_flags.HasFlag(Expandable))
                {
                    if (value != Capacity)
                        throw new NotSupportedException("NotExpandable");
                }
                else if (value == m_capacity)
                    SetCapacityCore(value);
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Empty method body.
        /// </summary>
        public override void Flush() { }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            try
            {
                Flush();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        public virtual byte[] GetBuffer()
        {
            EnsureExposable();
            return m_buffer.AsManaged();
        }

        public virtual NativeSpan<byte> GetBufferAsUnmanaged()
        {
            EnsureExposable();
            return m_buffer.AsSpan();
        }

        public virtual bool TryGetBuffer(out ArraySegment<byte> buffer)
        {
            if (!m_flags.HasFlag(StreamOption.PublicVisibile))
            {
                buffer = default;
                return false;
            }

            buffer = new ArraySegment<byte>(m_buffer.AsManaged(), m_origin, m_length - m_origin);
            return true;
        }

        public virtual bool TryGetBufferAsUnmanaged(out NativeSpan<byte> buffer)
        {
            if (!m_flags.HasFlag(StreamOption.PublicVisibile))
            {
                buffer = default;
                return false;
            }

            buffer = new NativeSpan<byte>(m_buffer.GetDataPointer() + m_origin, (nint_t)(m_length - m_origin));
            return true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            EnsureNotClosed();

            int n = (int)CMath.Min((uint)(m_length - m_position), (uint)count);
            if (n <= 0)
                return 0;

            Debug.Assert(m_position + n >= 0, "m_position + n >= 0");

            if (n <= sizeof(long))
            {
                var psrc = m_buffer.GetDataPointer() + m_position;
                for (int i = n; --i >= 0;)
                    buffer[offset + i] = psrc[i];
            }
            else
                Buffer.BlockCopy(m_buffer.AsManaged(), m_position, buffer, offset, n);

            m_position += n;

            return n;
        }

        public virtual int Read(in NativeSpan<byte> buffer)
        {
            if (buffer.Source == null)
                throw new ArgumentNullException(nameof(buffer));

            var n = CMathi.Min(m_length - m_position, buffer.Length);
            if (n <= 0)
                return 0;

            new NativeSpan<byte>(m_buffer.GetDataPointer() + m_position, n).CopyTo(buffer);

            m_position += (int)n;
            return (int)n;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateBufferArguments(buffer, offset, count);
            EnsureNotClosed();

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);

            try
            {
                int n = Read(buffer, offset, count);
                return GetReadTask(n);
            }
            catch (OperationCanceledException ex1)
            {
                return Task.FromCanceled<int>(ex1.CancellationToken);
            }
            catch (Exception ex2)
            {
                return Task.FromException<int>(ex2);
            }
        }

        public override int ReadByte()
        {
            EnsureNotClosed();

            if (m_position >= m_length)
                return -1;

            return m_buffer.GetDataPointer()[m_position++];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            EnsureNotClosed();

            if (offset > ARRAY_MAXLENGTH)
                throw new ArgumentOutOfRangeException(nameof(offset));

            int temp_pos;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    temp_pos = m_origin + (int)offset;
                    if (offset < 0 || temp_pos < m_origin)
                        throw new IOException(nameof(SeekOrigin.Begin));
                    break;

                case SeekOrigin.Current:
                    temp_pos = m_position + (int)offset;
                    if (unchecked(m_position + offset) < m_origin || temp_pos < m_origin)
                        throw new IOException(nameof(SeekOrigin.Current));
                    break;

                case SeekOrigin.End:
                    temp_pos = m_length + (int)offset;
                    if (unchecked(m_length + offset) < m_origin || temp_pos < m_origin)
                        throw new IOException(nameof(SeekOrigin.End));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }

            m_position += temp_pos;
            Debug.Assert(m_position >= 0, "m_position >= 0");

            return m_position;
        }

        public override void SetLength(long value)
        {
            if ((ulong)value > ARRAY_MAXLENGTH)
                throw new ArgumentOutOfRangeException(nameof(value));

            EnsureWriteable();

            if (value > ARRAY_MAXLENGTH - m_origin)
                throw new ArgumentOutOfRangeException(nameof(value));

            int newLength = m_origin + (int)value;

            bool allocateNew = EnsureCapacity(newLength);
            if (!allocateNew && newLength > m_length)
                Array.Clear(m_buffer.AsManaged(), m_length, newLength - m_length);

            m_length = newLength;
            m_position = (int)CMath.Min((uint)m_position, (uint)newLength);
        }

        public virtual byte[] ToArray()
        {
            int count = m_length - m_origin;
            if (count == 0)
                return Array.Empty<byte>();

            byte[] res = new byte[count];
            Buffer.BlockCopy(m_buffer.AsManaged(), m_origin, res, 0, count);

            return res;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            EnsureNotClosed();
            EnsureWriteable();

            int i = m_position + count;
            if (i < 0)
                throw new IOException("StreamTooLong");

            if (i > m_length)
                AdjustLength(i);

            if (count <= 8 && m_buffer.AsManaged() != buffer)
            {
                var pdst = m_buffer.GetDataPointer() + m_position;
                for (int x = count; --x >= 0;)
                    pdst[x] = buffer[offset + x];
            }
            else
                Buffer.BlockCopy(buffer, offset, m_buffer.AsManaged(), m_position, count);

            m_position = i;
        }

        public virtual void Write(in NativeConstSpan<byte> buffer)
        {
            if (buffer.DangerousGetPointer() == null)
                throw new ArgumentNullException(nameof(buffer));

            EnsureNotClosed();
            EnsureWriteable();

            var i = m_position + (int)buffer.Length;
            if (i < 0)
                throw new IOException("StreamTooLong");

            if (i > m_length)
                AdjustLength(i);

            buffer.CopyTo(new NativeSpan<byte>(m_buffer.GetDataPointer() + m_position, buffer.Length));
            m_position = i;
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateBufferArguments(buffer, offset, count);

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            try
            {
                Write(buffer, offset, count);
                return Task.CompletedTask;
            }
            catch (OperationCanceledException ex1)
            {
                return Task.FromCanceled(ex1.CancellationToken);
            }
            catch (Exception ex2)
            {
                return Task.FromException(ex2);
            }
        }

        public override void WriteByte(byte value)
        {
            EnsureNotClosed();
            EnsureWriteable();

            if (m_position >= m_length)
                AdjustLength(m_position + 1);

            m_buffer[m_position++] = value;
        }

        public virtual void WriteTo(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            EnsureNotClosed();

            stream.Write(m_buffer.AsManaged(), m_origin, m_length - m_origin);
        }
        #endregion

        #region helper methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AdjustLength(int i)
        {
            if ((i <= m_capacity || !EnsureCapacity(i)) && m_position > m_length)
                Array.Clear(m_buffer.AsManaged(), m_length, i - m_length);

            m_length = i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Task<int> GetReadTask(int result)
        {
            if (m_lastReadTask != null)
            {
                Debug.Assert(m_lastReadTask.Status == TaskStatus.RanToCompletion, "Expected that a stored last task completed successfully");
                if (m_lastReadTask.Result == result)
                    return m_lastReadTask;
            }

            return m_lastReadTask = Task.FromResult(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetCapacityCore(int value)
        {
            if (m_buffer.IsNull)
                m_buffer = value == 0 ? default : new arrayref<byte>(value);
            else if (value == 0)
            {
                m_buffer.Dispose();
                m_buffer = default;
            }
            else
            {
                var nBuffer = new arrayref<byte>(value);

                m_buffer.AsSpan().CopyTo(nBuffer.AsSpan());
                m_buffer.Dispose();

                m_buffer = nBuffer;
            }

            m_capacity = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureExposable()
        {
            if (!m_flags.HasFlag(StreamOption.PublicVisibile))
                throw new UnauthorizedAccessException(nameof(NativeMemoryStream));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureNotClosed()
        {
            if (!m_flags.HasFlag(IsOpen))
                throw new ObjectDisposedException(nameof(NativeMemoryStream));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureWriteable()
        {
            if (!m_flags.HasFlag(StreamOption.Writable))
                throw new NotSupportedException("UnwritableStream");
        }

        private bool EnsureCapacity(int value)
        {
            if (value < 0)
                throw new IOException("IO_StreamTooLong");

            if (value > m_capacity)
            {
                int nCapacity = (int)CMath.Max(CMath.Max((uint)value, 0x100u), (uint)(m_capacity << 1));

                if ((uint)nCapacity > ARRAY_MAXLENGTH)
                    nCapacity = Math.Max(value, ARRAY_MAXLENGTH);

                Capacity = nCapacity;
                return true;
            }

            return false;
        }
        #endregion

        #region dispose
        protected override void Dispose(bool disposing)
        {
            if (!m_flags.HasFlag(StreamOption.LeaveOpen | IsOpen))
                m_buffer.Dispose();

            if (disposing)
                m_flags = 0;

            base.Dispose(disposing);
        }
        #endregion

        #region static method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void ValidateBufferArguments(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (buffer.Length - offset < (uint)count)
                throw new ArgumentOutOfRangeException(nameof(count));
        }
        #endregion
    }
}
