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

using System.Runtime.CompilerServices;

namespace SuperComicLib.IO.AdvancedParallel
{
    public unsafe ref struct OffsetParallelStream
    {
        private readonly IParallelFileReader _owner;
        private readonly byte[] _buffer;
        private Range64 _offset;
        private int _bufpos;
        private int _bufsize;

        internal OffsetParallelStream(IParallelFileReader owner, in Range64 offset, int bufsize)
        {
            _owner = owner;
            _offset = offset;
            _buffer = new byte[bufsize];
            _bufpos = 0;
            _bufsize = 0;
        }

        #region property
        public bool EndOfStream => _offset.start == _offset.end && (uint)_bufpos >= _bufsize;

        public int BufferPosition => _bufpos;

        public int BufferLength => _bufsize;
        #endregion

        #region methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Range64 GetOffset() => _offset;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> GetBuffer() => _buffer.Slice(_bufpos, _bufsize - _bufpos);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Read() => EnsurePosition() ? _buffer[_bufpos++] : -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Peek() => EnsurePosition() ? _buffer[_bufpos] : -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SkipByte() => _bufpos++;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool EnsurePosition()
        {
            if ((uint)_bufpos >= (uint)_bufsize)
            {
                ref Range64 offset = ref _offset;
                if (offset.Length == 0)
                    return false;

                _bufpos = 0;

                uint size = (uint)_owner.UpdateBuffer(_buffer, ref offset);
                size = (uint)CMath.Min(size, (ulong)offset.Length);

                offset.start += size;

                _bufsize = (int)size;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte GetPinnableReference() => ref _buffer[0];
        #endregion
    }
}
