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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.IO
{
    public unsafe sealed class FastReader : IDisposable
    {
        private Stream _stream;
        private byte[] _buffer;

        public FastReader(Stream baseStream)
        {
            _stream = baseStream;
            _buffer = Array.Empty<byte>();
        }

        public FastReader(string filepath)
        {
            _stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _buffer = Array.Empty<byte>();
        }

        #region 메서드
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T ReadUnsafe<T>(Stream ss, ref byte[] buf) where T : unmanaged
        {
            if (buf.Length < sizeof(T))
                // 16 pad
                buf = new byte[sizeof(T) + ((16 - (sizeof(T) & 0xF)) & 0xF)];

            ss.Read(buf, 0, sizeof(T));

            fixed (byte* ptr = &buf[0])
                return *(T*)ptr;
        }

        public T ReadTo<T>() where T : unmanaged =>
            sizeof(T) == 0
            ? default
            : ReadUnsafe<T>(_stream, ref _buffer);

        public bool ReadBoolean() => _stream.ReadByte() > 0;

        public byte ReadUInt8() => (byte)_stream.ReadByte();

        public sbyte ReadInt8() => (sbyte)_stream.ReadByte();

        public char ReadChar() => ReadUnsafe<char>(_stream, ref _buffer);

        public short ReadInt16() => ReadUnsafe<short>(_stream, ref _buffer);

        public ushort ReadUInt16() => ReadUnsafe<ushort>(_stream, ref _buffer);

        public int ReadInt32() => ReadUnsafe<int>(_stream, ref _buffer);

        public uint ReadUInt32() => ReadUnsafe<uint>(_stream, ref _buffer);

        public float ReadSingle() => ReadUnsafe<float>(_stream, ref _buffer);

        public long ReadInt64() => ReadUnsafe<long>(_stream, ref _buffer);

        public ulong ReadUInt64() => ReadUnsafe<ulong>(_stream, ref _buffer);

        public double ReadDouble() => ReadUnsafe<double>(_stream, ref _buffer);

        public int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public string ReadString()
        {
            int length = ReadInt32();

            if (length == 0)
                return string.Empty;

            string str = new string('\0', length);
            fixed (char* pchars = str)
            {
                int bytes = (int)CMath.Min((uint)(length << 1), 16384u);

                var buffer = new byte[bytes];
                
                byte* pdest = (byte*)pchars;

                do
                {
                    int count = _stream.Read(buffer, 0, (int)CMath.Min((uint)buffer.Length, (uint)length));

                    if (count <= 0)
                        throw new EndOfStreamException(nameof(FastReader));

                    Marshal.Copy(buffer, 0, (IntPtr)pdest, count);
                    
                    pdest += count;
                    length -= count;
                } while (length != 0);
            }

            return str;
        }
        #endregion

        #region IDisposable Support
        private void Dispose(bool disposing)
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }

            if (disposing)
                _buffer = null;
        }

        ~FastReader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
