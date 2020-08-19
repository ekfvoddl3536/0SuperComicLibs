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

        public StringStream(int capacity) : this(capacity, Encoding.UTF8) { }

        public StringStream(int capacity, Encoding encoding)
        {
            this.encoding = encoding;
            buffer = new byte[capacity];
        }

        public StringStream(string text) : this(text, Encoding.UTF8) { }

        public StringStream(string text, Encoding encoding)
        {
            this.encoding = encoding;
            buffer = encoding.GetBytes(text);
        }

        public StreamReader Out() => 
            buffer == null 
            ? throw new ObjectDisposedException(nameof(StringStream))
            : new StreamReader(this, encoding);

        public StreamWriter In() =>
            buffer == null
            ? throw new ObjectDisposedException(nameof(StringStream))
            : new StreamWriter(this, encoding);
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

            ref int pos = ref this.pos;
            int n = pos + count;
            if (n > src.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "overflow");

            if (count <= 8)
            {
                int bytecnt = n;
                while (--bytecnt >= 0)
                    src[pos + bytecnt] = buffer[offset + bytecnt];
            }
            else
                Buffer.BlockCopy(buffer, offset, src, pos, count);

            pos = n;
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
