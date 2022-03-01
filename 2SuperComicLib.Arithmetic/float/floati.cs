using System;
#pragma warning disable IDE1006 // 명명 스타일
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SuperComicLib.Arithmetic
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public readonly unsafe struct floati : IComparable, IEquatable<floati>, IComparable<floati>, IConvertible, IFormattable
    {
        public const int MaxValue = int.MaxValue;
        public const int MinValue = -(int.MinValue + 1);

        private readonly int _value;

        #region implement interfaces
        int IComparable.CompareTo(object obj) =>
            obj is floati other
            ? CompareTo(other)
            : throw new ArgumentException($"invalid type");

        public int CompareTo(floati other) => _value - *(int*)&other;

        public bool Equals(floati other) => _value == *(int*)&other;
        #endregion

        #region implement IConvertible
        TypeCode IConvertible.GetTypeCode() => TypeCode.Int32;

        bool IConvertible.ToBoolean(IFormatProvider provider) => _value > 0;
        byte IConvertible.ToByte(IFormatProvider provider) => (byte)_value;
        char IConvertible.ToChar(IFormatProvider provider) => (char)_value;
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => new DateTime(_value * TimeSpan.TicksPerMillisecond);
        decimal IConvertible.ToDecimal(IFormatProvider provider) => _value;
        double IConvertible.ToDouble(IFormatProvider provider) => this;
        short IConvertible.ToInt16(IFormatProvider provider) => (short)_value;
        int IConvertible.ToInt32(IFormatProvider provider) => _value;
        long IConvertible.ToInt64(IFormatProvider provider) => _value;
        sbyte IConvertible.ToSByte(IFormatProvider provider) => (sbyte)_value;
        float IConvertible.ToSingle(IFormatProvider provider) => (float)this;
        string IConvertible.ToString(IFormatProvider provider) => ToString();
        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => this;
        ushort IConvertible.ToUInt16(IFormatProvider provider) => (ushort)_value;
        uint IConvertible.ToUInt32(IFormatProvider provider) => (uint)_value;
        ulong IConvertible.ToUInt64(IFormatProvider provider) => (uint)_value;
        #endregion

        #region override methods (instance)
        public override bool Equals(object obj) => obj is floati other && this == other;
        public override int GetHashCode() => _value;
        public override string ToString() => ((double)this).ToString();

        public string ToString(string format, IFormatProvider provider) => ((double)this).ToString(format, provider);
        public string ToString(string format) => ((double)this).ToString(format);
        #endregion

        #region cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(floati val) => *(int*)&val;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double(floati val) => CMath.Max(int.MinValue + 1, *(int*)&val) / (double)int.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator floati(int val) => *(floati*)&val;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator floati(double val)
        {
            int temp = (int)(Math.Min(Math.Max(val, -1.0d), 1.0d) * int.MaxValue);
            return *(floati*)&temp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float(floati val) => CMath.Max(int.MinValue + 1, *(int*)&val) / (float)int.MaxValue;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator floati(float val)
        {
            int temp = (int)(Math.Min(Math.Max(val, -1.0f), 1.0f) * (double)int.MaxValue);
            return *(floati*)&temp;
        }
        #endregion

        #region comapre & equals (operator override)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(floati left, floati right) => *(int*)&left == *(int*)&right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(floati left, floati right) => *(int*)&left != *(int*)&right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(floati left, floati right) => *(int*)&left < *(int*)&right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(floati left, floati right) => *(int*)&left > *(int*)&right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(floati left, floati right) => *(int*)&left <= *(int*)&right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(floati left, floati right) => *(int*)&left >= *(int*)&right;
        #endregion

        #region [+ - * / %] operator (override)
        #region right int
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator +(floati left, int right) => *(int*)&left + right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator -(floati left, int right) => *(int*)&left - right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator *(floati left, int right) => (int)((*(int*)&left * (long)(right + (int)((uint)~right >> 31))) >> 31);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator /(floati left, int right) => left * (int.MaxValue / right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator %(floati left, int right) => left - left / right;
        #endregion

        #region self
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator +(floati left, floati right) => *(int*)&left + *(int*)&right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator -(floati left, floati right) => *(int*)&left - *(int*)&right;
        // 부정확한 연산 (근사값). 1 * 1 = 0.9999...
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator *(floati left, floati right) => left * *(int*)&right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator /(floati left, floati right) => left * (int.MaxValue / *(int*)&right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator %(floati left, floati right) => left - left / right;
        #endregion
        #endregion

        #region bit operator (override)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator &(floati left, floati right) => *(int*)&left & *(int*)&right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator |(floati left, floati right) => *(int*)&left | *(int*)&right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator ^(floati left, floati right) => *(int*)&left ^ *(int*)&right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator <<(floati left, int right)  => *(int*)&left << right;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator >>(floati left, int right)  => *(int*)&left >> right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator -(floati left) => -*(int*)&left;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator +(floati left) => +*(int*)&left;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static floati operator ~(floati left) => ~*(int*)&left;
        #endregion
    }
}