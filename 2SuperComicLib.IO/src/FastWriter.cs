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

namespace SuperComicLib.IO
{
    public unsafe sealed class FastWriter : IDisposable
    {
        private Stream _stream;
        private byte[] _buffer;

        public FastWriter(Stream baseStream)
        {
            _stream = baseStream;
            _buffer = Array.Empty<byte>();
        }

        public FastWriter(string filepath, FileMode mode, FileAccess access)
        {
            _stream = File.Open(filepath, mode, access);
            _buffer = Array.Empty<byte>();
        }

        public FastWriter(string filepath) : this(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite)
        {
        }

        #region 메서드
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteUnsafe<T>(Stream ss, ref byte[] buf, T* ptr) where T : unmanaged
        {
            if (buf.Length < sizeof(T))
                // 16 pad
                buf = new byte[sizeof(T) + ((16 - (sizeof(T) & 0xF)) & 0xF)];

            fixed (byte* dst = &buf[0])
            {
                *(T*)dst = *ptr;

                ss.Write(buf, 0, sizeof(T));
            }
        }

        public void Write<T>(T value) where T : unmanaged
        {
            if (sizeof(T) == 0)
                return;

            WriteUnsafe(_stream, ref _buffer, &value);
        }

        public void Write<T>(T* p_value) where T : unmanaged
        {
            if (sizeof(T) == 0)
                return;

            WriteUnsafe(_stream, ref _buffer, p_value);
        }

        public void Write(byte[] data) => _stream.Write(data, 0, data.Length);

        public void Write(byte data) => _stream.WriteByte(data);

        public void Write(sbyte data) => _stream.WriteByte(*(byte*)&data);

        public void Write(bool data) => _stream.WriteByte(data ? (byte)1 : byte.MinValue);

        public void Write(char data) => WriteUnsafe(_stream, ref _buffer, &data);

        public void Write(short data) => WriteUnsafe(_stream, ref _buffer, &data);

        public void Write(ushort data) => WriteUnsafe(_stream, ref _buffer, &data);

        public void Write(int data) => WriteUnsafe(_stream, ref _buffer, &data);

        public void Write(uint data) => WriteUnsafe(_stream, ref _buffer, &data);

        public void Write(long data) => WriteUnsafe(_stream, ref _buffer, &data);

        public void Write(ulong data) => WriteUnsafe(_stream, ref _buffer, &data);

        public void Write(float data) => WriteUnsafe(_stream, ref _buffer, &data);

        public void Write(double data) => WriteUnsafe(_stream, ref _buffer, &data);

        public void Write(string strdata)
        {
            if (string.IsNullOrWhiteSpace(strdata))
                Write(0);
            else
                fixed (char* ptr = strdata)
                {
                    int length = strdata.Length;

                    Write(length);
                    new NativeSpan<char>(ptr, length).CopyToStream(_stream);
                }
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

        ~FastWriter()
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
