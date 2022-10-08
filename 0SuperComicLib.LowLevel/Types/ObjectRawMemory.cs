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
using System.Runtime.CompilerServices;

namespace SuperComicLib.LowLevel
{
    public readonly unsafe ref struct ObjectRawMemory
    {
        internal readonly byte* _typehnd;
        internal readonly byte[] _memory;

        internal ObjectRawMemory(IntPtr typehnd, byte[] memory)
        {
            _typehnd = (byte*)typehnd;
            _memory = memory;
        }

        #region property
        public bool IsNull => _typehnd == null;

        public IntPtr SyncBlock => *(IntPtr*)(_typehnd - sizeof(void*));

        public IntPtr TypeHandle => *(IntPtr*)_typehnd;

        public int Length => _memory.Length;

        public PubMethodTable MethodTable => **(PubMethodTable**)_typehnd;
        #endregion

        #region indexer
        public ref byte this[int index] => ref _memory[index];
        #endregion

        #region array & fixed
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToArray() => (byte[])_memory.Clone();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte GetPinnableReference() => ref _memory[0];
        #endregion

        #region read
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Read(int source_byteOffset, byte[] buffer, int buffer_start, int count) =>
            Array.Copy(_memory, source_byteOffset, buffer, buffer_start, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Read(int source_byteOffset, byte[] buffer, int buffer_start) =>
            Read(source_byteOffset, buffer, buffer_start, buffer.Length - buffer_start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Read(int source_byteOffset, byte[] buffer) =>
            Read(source_byteOffset, buffer, 0, buffer.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Read(int source_byteOffset, int count)
        {
            byte[] buffer = new byte[count];
            Read(source_byteOffset, buffer, 0, count);
            return buffer;
        }
        #endregion

        #region write
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int source_byteOffset, byte[] buffer, int buffer_start, int count) =>
            Array.Copy(buffer, buffer_start, _memory, source_byteOffset, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int source_byteOffset, byte[] buffer, int buffer_start) =>
            Write(source_byteOffset, buffer, buffer_start, buffer.Length - buffer_start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int source_byteOffset, byte[] buffer) =>
            Write(source_byteOffset, buffer, 0, buffer.Length);
        #endregion

        #region generic + unsafe
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>(int source_byteOffset) where T : struct
        {
            Validate(source_byteOffset, _memory);

            var rp = NativeStruct<T>.Instance;
            fixed (byte* ptr = &_memory[0])
                return rp.Read(ptr + source_byteOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Ref<T>(int source_byteOffset) where T : unmanaged
        {
            Validate(source_byteOffset, _memory);

            fixed (byte* ptr = &_memory[0])
                return ref *(T*)(ptr + source_byteOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Cast<T>(int source_byteOffset) where T : class
        {
            Validate(source_byteOffset, _memory);

            var cc = NativeClass<T>.Instance;
            fixed (byte* ptr = &_memory[0])
                return cc.Cast(ptr + source_byteOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteReference(int source_byteOffset, object value)
        {
            Validate(source_byteOffset, _memory);

            var tr = __makeref(value);
            fixed (byte* ptr = &_memory[0])
                *(IntPtr*)(ptr + source_byteOffset) = **(IntPtr**)&tr;
        }
        #endregion

        #region helper
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Validate(int source_byteOffset, byte[] m)
        {
            if ((uint)source_byteOffset >= (uint)m.Length)
                throw new ArgumentOutOfRangeException(nameof(source_byteOffset));
        }
        #endregion
    }
}
