#pragma warning disable IDE1006 // 명명 스타일
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Arithmetic
{
    /// <summary>
    /// Compatible with BF16 used by <see langword="AVX512-VNNI"/> and <see langword="AVX-VNNI"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 2)]
    public readonly unsafe struct bfloat16
    {
        public const short MaxValue = 0x7F_7F;
        public const short MinValue = MaxValue | short.MinValue; // 0xFF_7F;

        private readonly short _value;

        public bfloat16(int __bit_value__) => _value = (short)__bit_value__;

        #region override
        public override bool Equals(object obj) => obj is bfloat16 other && this == other;

        public override int GetHashCode() => _value;

        public override string ToString() => ((float)this).ToString();
        #endregion

        #region cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bfloat16(float v) => *(bfloat16*)((byte*)&v + 2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator bfloat16(double v)
        {
            float tmp = (float)v;
            return *(bfloat16*)((byte*)&v + 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(bfloat16 v)
        {
            int temp = v._value << 16;
            return *(float*)&temp;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double(bfloat16 v) => (float)v;
        #endregion

        #region compare & equals
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(bfloat16 left, bfloat16 right) => (float)left == (float)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(bfloat16 left, bfloat16 right) => (float)left != (float)right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(bfloat16 left, bfloat16 right) => (float)left < (float)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(bfloat16 left, bfloat16 right) => (float)left > (float)right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(bfloat16 left, bfloat16 right) => (float)left <= (float)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(bfloat16 left, bfloat16 right) => (float)left >= (float)right;
        #endregion

        #region [+ - * / %] operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 operator +(bfloat16 left, bfloat16 right) => (float)left + (float)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 operator -(bfloat16 left, bfloat16 right) => (float)left - (float)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 operator *(bfloat16 left, bfloat16 right) => (float)left * (float)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 operator /(bfloat16 left, bfloat16 right) => (float)left / (float)right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 operator %(bfloat16 left, bfloat16 right) => (float)left % (float)right;
        #endregion

        #region unary operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 operator -(bfloat16 v) => new bfloat16(v._value ^ 0x80_00);
        #endregion

        #region bit operator
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 bitOR(bfloat16 a, bfloat16 b) => new bfloat16(a._value | b._value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 bitXOR(bfloat16 a, bfloat16 b) => new bfloat16(a._value ^ b._value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 bitAND(bfloat16 a, bfloat16 b) => new bfloat16(a._value & b._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 bitLSH(bfloat16 a, int shift) => new bfloat16(a._value << shift);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 bitRSH(bfloat16 a, int shift) => new bfloat16(a._value >> shift);
        #endregion

        #region math extension
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bfloat16 abs(bfloat16 value) => new bfloat16(value._value & short.MaxValue);
        #endregion
    }
}