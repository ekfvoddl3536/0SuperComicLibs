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
using System.IO;
using System.Text;

namespace SuperComicLib.Text
{
    public sealed unsafe class StringStream : Stream
    {
        private Encoding encoding;
        private byte[] buffer;
        private int pos;

        #region 생성자
        public StringStream() : this(4096, Encoding.UTF8) { }

        public StringStream(int capacity) : this(capacity, Encoding.Default) { }

        public StringStream(int capacity, Encoding encoding)
        {
            this.encoding = encoding;
            buffer = new byte[capacity];
        }

        public StringStream(string text) : this(text, Encoding.Default) { }

        public StringStream(string text, Encoding encoding)
        {
            this.encoding = encoding;
            buffer = encoding.GetBytes(text);
        }

        public StreamReader Out() => 
            buffer == null 
            ? throw new ObjectDisposedException(nameof(StringStream))
            : new StreamReader(this, encoding, true, 1024, true);

        public StreamWriter In() =>
            buffer == null
            ? throw new ObjectDisposedException(nameof(StringStream))
            : new StreamWriter(this, encoding, 1024, true);
        #endregion

        #region 속성
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => buffer?.Length ?? -1L;
        public override long Position
        {
            get => pos;
            set
            {
                if (buffer == null)
                    StreamIsClosed();

                pos = (int)value;
            }
        }
        #endregion

        #region 메소드
        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (buffer == null)
                StreamIsClosed();

            ref int pos = ref this.pos;
            if (origin == SeekOrigin.Begin)
                pos = (int)offset;
            else if (origin == SeekOrigin.Current)
                pos += (int)offset;
            else
                pos = buffer.Length - (int)offset;

            return pos;
        }

        public override void SetLength(long value)
        {
#if DEBUG
            throw new NotSupportedException();
#endif
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - offset < count)
                throw new ArgumentException();

            byte[] src = this.buffer;
            if (src == null)
                StreamIsClosed();

            ref int pos = ref this.pos;
            int n = (src.Length - pos).Min(count);
            if (n <= 0)
                return 0;

            if (n <= sizeof(long))
            {
                int bytecnt = n;
                while (--bytecnt >= 0)
                    buffer[offset + bytecnt] = src[pos + bytecnt];
            }
            else
                Buffer.BlockCopy(src, pos, buffer, offset, n);

            pos += n;

            return n;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - offset < count)
                throw new ArgumentException();

            byte[] src = this.buffer;
            if (src == null)
                StreamIsClosed();

            int pos = this.pos;
            int n = pos + count;
            if (n > src.Length)
                IncreaseCapacity();

            if (count <= 8)
            {
                int bytecnt = n;
                while (--bytecnt >= 0)
                    src[pos + bytecnt] = buffer[offset + bytecnt];
            }
            else
                Buffer.BlockCopy(buffer, offset, src, pos, count);

            this.pos = n;
        }

        private void IncreaseCapacity()
        {
            int newsize = buffer.Length << 1;
            if (newsize >= 0x7EFF_FFFF)
                throw new InsufficientMemoryException();

            byte[] newbuffer = new byte[newsize];
            Buffer.BlockCopy(buffer, 0, newbuffer, 0, buffer.Length);

            buffer = newbuffer;
        }

        protected override void Dispose(bool disposing)
        {
            if (buffer != null)
            {
                encoding = null;
                buffer = null;
                pos = 0;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region block
        private static void StreamIsClosed() => throw new ObjectDisposedException(nameof(StringStream));
        #endregion
    }
}
