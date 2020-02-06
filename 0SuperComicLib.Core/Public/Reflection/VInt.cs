using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = sizeof(int))]
    public readonly unsafe struct VInt : IEquatable<VInt>, IComparable<VInt>, IConvertible, IFormattable
    {
        #region 필드 & 생성자
        private readonly void* m_value;

        public VInt(int value) => m_value = (void*)value;

        public VInt(long value) => m_value = (void*)value;

        public VInt(uint value) => m_value = (void*)value;

        public VInt(ulong value) => m_value = (void*)value;

        public VInt(IntPtr value) => m_value = value.ToPointer();

        public VInt(UIntPtr value) => m_value = value.ToPointer();
        #endregion

        #region 함수
        public int ToInt32() => *(int*)m_value;

        public long ToInt64() => *(long*)m_value;

        public uint ToUInt32() => *(uint*)m_value;

        public ulong ToUInt64() => *(ulong*)m_value;
        #endregion

        #region override
        public override string ToString() =>
            IntPtr.Size == sizeof(long)
            ? (*(long*)m_value).ToString()
            : (*(uint*)m_value).ToString();
        public override int GetHashCode() =>
            IntPtr.Size == sizeof(long)
            ? (*(long*)m_value).GetHashCode()
            : (*(uint*)m_value).GetHashCode();
        public override bool Equals(object obj) =>
            IntPtr.Size == sizeof(long)
            ? (*(long*)m_value).Equals(obj)
            : (*(uint*)m_value).Equals(obj);
        #endregion

        #region Convertible
        private IConvertible IConv => IntPtr.Size == sizeof(long) ? (*(long*)m_value) : (*(uint*)m_value);

        TypeCode IConvertible.GetTypeCode() => IConv.GetTypeCode();
        bool IConvertible.ToBoolean(IFormatProvider provider) => IConv.ToBoolean(provider);
        char IConvertible.ToChar(IFormatProvider provider) => IConv.ToChar(provider);
        sbyte IConvertible.ToSByte(IFormatProvider provider) => IConv.ToSByte(provider);
        byte IConvertible.ToByte(IFormatProvider provider) => IConv.ToByte(provider);
        short IConvertible.ToInt16(IFormatProvider provider) => IConv.ToInt16(provider);
        ushort IConvertible.ToUInt16(IFormatProvider provider) => IConv.ToUInt16(provider);
        int IConvertible.ToInt32(IFormatProvider provider) => IConv.ToInt32(provider);
        uint IConvertible.ToUInt32(IFormatProvider provider) => IConv.ToUInt32(provider);
        long IConvertible.ToInt64(IFormatProvider provider) => IConv.ToInt64(provider);
        ulong IConvertible.ToUInt64(IFormatProvider provider) => IConv.ToUInt64(provider);
        float IConvertible.ToSingle(IFormatProvider provider) => IConv.ToSingle(provider);
        double IConvertible.ToDouble(IFormatProvider provider) => IConv.ToDouble(provider);
        decimal IConvertible.ToDecimal(IFormatProvider provider) => IConv.ToDecimal(provider);
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => IConv.ToDateTime(provider);
        string IConvertible.ToString(IFormatProvider provider) => IConv.ToString(provider);
        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => IConv.ToType(conversionType, provider);
        #endregion

        #region Formattable & Comparable<T> & Equatable<T>
        public string ToString(string format, IFormatProvider formatProvider) =>
            (IntPtr.Size == sizeof(long) ? (*(long*)m_value) : (*(uint*)m_value)).ToString(format, formatProvider);

        public int CompareTo(VInt other) =>
            IntPtr.Size == sizeof(long)
            ? (*(long*)m_value).CompareTo(*(long*)other.m_value)
            : (*(int*)m_value).CompareTo(*(int*)other.m_value);

        public bool Equals(VInt other) =>
            IntPtr.Size == sizeof(long)
            ? *(long*)m_value == *(long*)other.m_value
            : *(int*)m_value == *(int*)other.m_value;
        #endregion

        #region 명시적 변환
        public static explicit operator int(VInt value) => *(int*)value.m_value;
        public static explicit operator long(VInt value) => *(long*)value.m_value;
        public static explicit operator uint(VInt value) => *(uint*)value.m_value;
        public static explicit operator ulong(VInt value) => *(ulong*)value.m_value;
        public static explicit operator IntPtr(VInt value) => new IntPtr(value.m_value);
        public static explicit operator UIntPtr(VInt value) => new UIntPtr(value.m_value);

        public static explicit operator void*(VInt value) => value.m_value;
        public static explicit operator bool*(VInt value) => (bool*)value.m_value;
        public static explicit operator byte*(VInt value) => (byte*)value.m_value;
        public static explicit operator sbyte*(VInt value) => (sbyte*)value.m_value;
        public static explicit operator short*(VInt value) => (short*)value.m_value;
        public static explicit operator ushort*(VInt value) => (ushort*)value.m_value;
        public static explicit operator char*(VInt value) => (char*)value.m_value;
        public static explicit operator int*(VInt value) => (int*)value.m_value;
        public static explicit operator uint*(VInt value) => (uint*)value.m_value;
        public static explicit operator long*(VInt value) => (long*)value.m_value;
        public static explicit operator ulong*(VInt value) => (ulong*)value.m_value;
        public static explicit operator float*(VInt value) => (float*)value.m_value;
        public static explicit operator double*(VInt value) => (double*)value.m_value;
        #endregion

        #region 비교
        public static bool operator ==(VInt left, VInt right) => left.Equals(right);
        public static bool operator !=(VInt left, VInt right) => !left.Equals(right);

        public static bool operator <(VInt left, VInt right) => left.CompareTo(right) < 0;
        public static bool operator <=(VInt left, VInt right) => left.CompareTo(right) <= 0;

        public static bool operator >(VInt left, VInt right) => left.CompareTo(right) > 0;
        public static bool operator >=(VInt left, VInt right) => left.CompareTo(right) >= 0;
        #endregion

        #region 산술
        public static VInt operator +(VInt left, VInt right) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(*(long*)left.m_value + *(long*)right.m_value)
            : new VInt(*(int*)left.m_value + *(int*)right.m_value);
        public static VInt operator -(VInt left, VInt right) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(*(long*)left.m_value - *(long*)right.m_value)
            : new VInt(*(int*)left.m_value - *(int*)right.m_value);
        public static VInt operator *(VInt left, VInt right) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(*(long*)left.m_value * *(long*)right.m_value)
            : new VInt(*(int*)left.m_value * *(int*)right.m_value);
        public static VInt operator /(VInt left, VInt right) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(*(long*)left.m_value / *(long*)right.m_value)
            : new VInt(*(int*)left.m_value / *(int*)right.m_value);
        public static VInt operator %(VInt left, VInt right) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(*(long*)left.m_value % *(long*)right.m_value)
            : new VInt(*(int*)left.m_value % *(int*)right.m_value);
        #endregion

        #region 비트
        public static VInt operator <<(VInt left, int right) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(*(long*)left.m_value << right)
            : new VInt(*(int*)left.m_value << right);
        public static VInt operator >>(VInt left, int right) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(*(long*)left.m_value >> right)
            : new VInt(*(int*)left.m_value >> right);
        public static VInt operator &(VInt left, VInt right) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(*(long*)left.m_value & *(long*)right.m_value)
            : new VInt(*(int*)left.m_value & *(int*)right.m_value);
        public static VInt operator |(VInt left, VInt right) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(*(long*)left.m_value | *(long*)right.m_value)
            : new VInt(*(int*)left.m_value | *(int*)right.m_value);
        public static VInt operator ^(VInt left, VInt right) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(*(long*)left.m_value ^ *(long*)right.m_value)
            : new VInt(*(int*)left.m_value ^ *(int*)right.m_value);
        public static VInt operator ~(VInt left) =>
            IntPtr.Size == sizeof(long)
            ? new VInt(~*(long*)left.m_value)
            : new VInt(~*(int*)left.m_value);
        #endregion

        #region 연산자 확장 (32비트)
        #region 비교
        public static bool operator ==(VInt left, int right) => *(int*)left.m_value == right;
        public static bool operator !=(VInt left, int right) => *(int*)left.m_value != right;
        public static bool operator <(VInt left, int right) => *(int*)left.m_value < right;
        public static bool operator <=(VInt left, int right) => *(int*)left.m_value <= right;
        public static bool operator >(VInt left, int right) => *(int*)left.m_value > right;
        public static bool operator >=(VInt left, int right) => *(int*)left.m_value >= right;

        public static bool operator ==(int left, VInt right) => left == *(int*)right.m_value;
        public static bool operator !=(int left, VInt right) => left != *(int*)right.m_value;
        public static bool operator <(int left, VInt right) => left < *(int*)right.m_value;
        public static bool operator <=(int left, VInt right) => left <= *(int*)right.m_value;
        public static bool operator >(int left, VInt right) => left > *(int*)right.m_value;
        public static bool operator >=(int left, VInt right) => left >= *(int*)right.m_value;
        #endregion

        #region 산술
        public static VInt operator +(VInt left, int right) => new VInt(*(int*)left.m_value + right);
        public static VInt operator -(VInt left, int right) => new VInt(*(int*)left.m_value - right);
        public static VInt operator *(VInt left, int right) => new VInt(*(int*)left.m_value * right);
        public static VInt operator /(VInt left, int right) => new VInt(*(int*)left.m_value / right);
        public static VInt operator %(VInt left, int right) => new VInt(*(int*)left.m_value % right);
        #endregion
        #endregion
    }
}
