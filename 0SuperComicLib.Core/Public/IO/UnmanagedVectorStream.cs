using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SuperComicLib.IO
{
    public unsafe class UnmanagedVectorStream : Stream
    {
        protected byte* _pointer;
        protected int _length;
        protected int _position;

        public UnmanagedVectorStream() { }

        public UnmanagedVectorStream(void* pointer, int bytesLength) => Reset(pointer, bytesLength);

        #region property impl
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => _length;
        public override long Position { get => _position; set => Int32Seek((int)value, 0); }
        #endregion

        #region method impl
        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin) => Int32Seek((int)offset, origin);

        public override void SetLength(long value) { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ref int position = ref _position;

            count = (int)CMath.Min((uint)(_length - position), (uint)count);
            Marshal.Copy((IntPtr)(_pointer + position), buffer, offset, count);
            position += count;

            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ref int position = ref _position;

            count = (int)CMath.Min((uint)(_length - position), (uint)count);
            Marshal.Copy(buffer, offset, (IntPtr)(_pointer + position), count);
            position += count;
        }
        #endregion

        #region methods & properties
        public int Int32Position { get => _position; set => Int32Seek(value, 0); }

        public int Int32Seek(int offset, SeekOrigin origin)
        {
            int temp;
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

            // _position = Math.Max(Math.Min(temp, _length), 0);
            _position = (int)CMath.Min((uint)temp, (uint)_length);
            return _position;
        }

        public void SeekBegin0() => _position = 0;

        public void Reset(void* pointer, int bytesLength)
        {
            _pointer = (byte*)pointer;
            _length = bytesLength;
            _position = 0;
        }

        public void ApplyOffset()
        {
            int pos = _position;
            _position += pos;
            _pointer += pos;
            _length -= pos;
        }
        #endregion
    }
}
