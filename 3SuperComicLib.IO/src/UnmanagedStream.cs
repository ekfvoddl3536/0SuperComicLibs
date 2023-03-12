// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using SuperComicLib.CodeContracts;

namespace SuperComicLib.IO
{
    [SuppressUnmanagedCodeSecurity]
    public unsafe class UnmanagedStream : Stream
    {
        protected byte* _source;
        protected long _length;
        protected long _position;
        protected bool b_writable;

        public UnmanagedStream([DisallowNull] void* pointer, long bytesLength, bool writable = true)
        {
            _source = (byte*)pointer;
            _length = bytesLength;
            _position = 0;
            b_writable = writable;
        }

        #region property impl + property
        public override bool CanTimeout => false;
        public override bool CanWrite => b_writable;
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override sealed long Length => _length;
        public override sealed long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Current);
        }

        public bool EndOfStream => (ulong)_position >= (ulong)_length;
        #endregion

        #region states test method
        /// <summary>
        /// Check that the specified buffer size can be read or written.
        /// </summary>
        public bool IsAccessible(long buffer_bytes) => (ulong)buffer_bytes < (ulong)(_length - _position);
        #endregion

        #region method impl
        public override sealed int ReadByte()
        {
            if ((ulong)_position >= (ulong)_length)
                return -1;

            return _source[_position++];
        }

        public override void WriteByte(byte value)
        {
            if (!b_writable)
                throw new InvalidOperationException("ReadOnly stream");

            if ((ulong)_position >= (ulong)_length)
                throw new EndOfStreamException(nameof(UnmanagedStream));

            _source[_position++] = value;
        }

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long temp;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    temp = offset;
                    break;

                case SeekOrigin.Current:
                    temp = _position + offset;
                    break;

                case SeekOrigin.End:
                    temp = _length - offset;
                    break;

                default:
                    return offset;
            }

            _position = (long)CMath.Min((ulong)temp, (ulong)_length);
            return _position;
        }

        public override sealed void SetLength(long value) { }

        public override sealed int Read(byte[] buffer, int offset, int count)
        {
            long position = _position;

            count = (int)CMath.Min((ulong)(_length - position), (uint)count);
            Marshal.Copy((IntPtr)(_source + position), buffer, offset, count);
            _position += (uint)count;

            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!b_writable)
                throw new InvalidOperationException("ReadOnly stream");

            long position = _position;

            if ((ulong)(_length - position) < (uint)count)
                throw new EndOfStreamException(nameof(UnmanagedStream));

            Marshal.Copy(buffer, offset, (IntPtr)(_source + position), count);
            _position += (uint)count;
        }
        #endregion

        #region +native span
        public virtual long Read(byte* buffer_begin, byte* buffer_end)
        {
            ref long position = ref _position;

            ulong sizeInBytes = CMath.Min((ulong)(buffer_end - buffer_begin), (ulong)(_length - position));
            Buffer.MemoryCopy(_source + position, buffer_begin, sizeInBytes, sizeInBytes);
            position += (long)sizeInBytes;

            return (long)sizeInBytes;
        }

        public virtual void Write(byte* buffer_begin, byte* buffer_end)
        {
            if (!b_writable)
                throw new InvalidOperationException("ReadOnly stream");

            long position = _position;

            ulong sizeInBytes = (ulong)(buffer_end - buffer_begin);
            if ((ulong)(_length - position) < sizeInBytes)
                throw new EndOfStreamException(nameof(UnmanagedStream));

            Buffer.MemoryCopy(buffer_begin, _source + position, sizeInBytes, sizeInBytes);
            _position += (long)sizeInBytes;
        }
        #endregion

        #region copy-to direct
        public virtual long CopyToDirect(UnmanagedStream dest)
        {
            ulong sizeInBytes = (ulong)(_length - _position);

            if (dest.CanWrite == false ||
                dest.IsAccessible((long)sizeInBytes) == false)
                return 0;

            Buffer.MemoryCopy(_source + _position, dest._source + dest._position, sizeInBytes, sizeInBytes);

            _position += (long)sizeInBytes;
            dest._position += (long)sizeInBytes;

            return (long)sizeInBytes;
        }
        #endregion

        #region methods
        public virtual void SeekBeginFast() => _position = 0;

        public virtual void Reset(void* pointer, long bytesLength)
        {
            _source = (byte*)pointer;
            _length = bytesLength;
            _position = 0;
        }
        #endregion

        #region disposing
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _source = null;
        }
        #endregion
    }
}
