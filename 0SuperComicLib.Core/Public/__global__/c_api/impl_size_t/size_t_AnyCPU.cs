#if AnyCPU
using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    unsafe partial struct size_t
    {
        public readonly byte* value;

        #region constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t(void* val) => value = (byte*)val;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t(int val) => value = (byte*)val;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t(uint val) => value = (byte*)val;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t(long val) => value = (byte*)val;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public size_t(ulong val) => value = (byte*)val;
        #endregion

        #region interface impl
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(size_t other) => (int)(value - other.value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(size_t other) => value == other.value;
        #endregion

        #region override
        public override bool Equals(object obj) => obj is size_t other && value == other.value;
        public override int GetHashCode() => ((IntPtr)value).GetHashCode();
        #endregion

        #region cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator size_t(void* val) => new size_t(val);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator size_t(int val) => new size_t(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator size_t(uint val) => new size_t(val);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator size_t(long val) => new size_t(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator size_t(ulong val) => new size_t(val);
        #endregion

        #region offset math (quick)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t operator ++(size_t val) => new size_t(val.value + 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t operator --(size_t val) => new size_t(val.value - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t operator +(size_t val, uint offset) => new size_t(val.value + offset);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t operator -(size_t val, uint offset) => new size_t(val.value - offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public static size_t operator +(size_t val, ulong offset) => new size_t(val.value + offset);
        [MethodImpl(MethodImplOptions.AggressiveInlining), CodeContracts.X64Only]
        public static size_t operator -(size_t val, ulong offset) => new size_t(val.value - offset);
        #endregion

        #region size_t math operation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t operator +(size_t left, size_t right) =>
            IntPtr.Size == sizeof(int)
            ? new size_t(left.value + (uint)right.value)
            : new size_t(left.value + (ulong)right.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static size_t operator -(size_t left, size_t right) => new size_t((void*)(left.value - right.value));
        #endregion

        #region eq & compare
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(size_t left, size_t right) => left.value == right.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(size_t left, size_t right) => left.value != right.value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(size_t left, size_t right) => left.value < right.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(size_t left, size_t right) => left.value > right.value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(size_t left, size_t right) => left.value >= right.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(size_t left, size_t right) => left.value <= right.value;
        #endregion
    }
}

#endif