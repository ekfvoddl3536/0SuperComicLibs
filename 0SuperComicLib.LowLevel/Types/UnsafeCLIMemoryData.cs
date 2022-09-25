using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib.LowLevel
{
    public readonly unsafe ref struct UnsafeCLIMemoryData
    {
        internal readonly byte* _typehnd;
        internal readonly byte[] _memory;

        internal UnsafeCLIMemoryData(IntPtr typehnd, byte[] memory)
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
            if ((uint)source_byteOffset >= (uint)_memory.Length)
                throw new ArgumentOutOfRangeException(nameof(source_byteOffset));

            var rp = NativeStruct<T>.Instance;
            fixed (byte* ptr = &_memory[0])
                return rp.Read(ptr + source_byteOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Ref<T>(int source_byteOffset) where T : unmanaged
        {
            if ((uint)source_byteOffset >= (uint)_memory.Length)
                throw new ArgumentOutOfRangeException(nameof(source_byteOffset));

            fixed (byte* ptr = &_memory[0])
                return ref *(T*)(ptr + source_byteOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Cast<T>(int source_byteOffset) where T : class
        {
            if ((uint)source_byteOffset >= (uint)_memory.Length)
                throw new ArgumentOutOfRangeException(nameof(source_byteOffset));

            var cc = NativeClass<T>.Instance;
            fixed (byte* ptr = &_memory[0])
                return cc.Cast(ptr + source_byteOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteReference(int source_byteOffset, object value)
        {
            if ((uint)source_byteOffset >= (uint)_memory.Length)
                throw new ArgumentOutOfRangeException(nameof(source_byteOffset));

            var tr = __makeref(value);
            fixed (byte* ptr = &_memory[0])
                *(IntPtr*)(ptr + source_byteOffset) = **(IntPtr**)&tr;
        }
        #endregion
    }
}
