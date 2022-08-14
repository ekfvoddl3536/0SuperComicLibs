using SuperComicLib.Collections;
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

        public bool EndOfStream => _offset.start == _offset.end && (uint)_bufpos >= _bufsize;

        public int BufferPosition => _bufpos;

        public int BufferLength => _bufsize;

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
    }
}
