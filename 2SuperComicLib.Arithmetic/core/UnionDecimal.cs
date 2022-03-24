using System;
using System.Runtime.InteropServices;

namespace SuperComicLib
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct UnionDecimal : IFormattable
    {
        #region QWORD SIZE (BASE)
        [FieldOffset(0)]
        public long value_i64;
        [FieldOffset(0)]
        public ulong value_u64;
        [FieldOffset(0)]
        public double value_f64;
        #endregion

        #region DWORD SIZE
        [FieldOffset(0)]
        public int value_i32_0;
        [FieldOffset(0)]
        public uint value_u32_0;
        [FieldOffset(0)]
        public float value_f32_0;
        [FieldOffset(1)]
        public int value_i32_1;
        [FieldOffset(1)]
        public uint value_u32_1;
        [FieldOffset(1)]
        public float value_f32_1;
        [FieldOffset(2)]
        public int value_i32_2;
        [FieldOffset(2)]
        public uint value_u32_2;
        [FieldOffset(2)]
        public float value_f32_2;
        [FieldOffset(3)]
        public int value_i32_3;
        [FieldOffset(3)]
        public uint value_u32_3;
        [FieldOffset(3)]
        public float value_f32_3;
        [FieldOffset(4)]
        public int value_i32_4;
        [FieldOffset(4)]
        public uint value_u32_4;
        [FieldOffset(4)]
        public float value_f32_4;
        #endregion

        #region WORD SIZE
        [FieldOffset(0)]
        public short value_i16_0;
        [FieldOffset(0)]
        public ushort value_u16_0;
        [FieldOffset(0)]
        public char value_c0;
        [FieldOffset(1)]
        public short value_i16_1;
        [FieldOffset(1)]
        public ushort value_u16_1;
        [FieldOffset(1)]
        public char value_c1;
        [FieldOffset(2)]
        public short value_i16_2;
        [FieldOffset(2)]
        public ushort value_u16_2;
        [FieldOffset(2)]
        public char value_c2;
        [FieldOffset(3)]
        public short value_i16_3;
        [FieldOffset(3)]
        public ushort value_u16_3;
        [FieldOffset(3)]
        public char value_c3;
        [FieldOffset(4)]
        public short value_i16_4;
        [FieldOffset(4)]
        public ushort value_u16_4;
        [FieldOffset(4)]
        public char value_c4;
        [FieldOffset(5)]
        public short value_i16_5;
        [FieldOffset(5)]
        public ushort value_u16_5;
        [FieldOffset(5)]
        public char value_c5;
        [FieldOffset(6)]
        public short value_i16_6;
        [FieldOffset(6)]
        public ushort value_u16_6;
        [FieldOffset(6)]
        public char value_c6;
        #endregion

        #region BYTE SIZE
        [FieldOffset(0)]
        public sbyte value_i8_0;
        [FieldOffset(0)]
        public byte value_u8_0;
        [FieldOffset(0)]
        public bool value_b0;
        [FieldOffset(1)]
        public sbyte value_i8_1;
        [FieldOffset(1)]
        public byte value_u8_1;
        [FieldOffset(1)]
        public bool value_b1;
        [FieldOffset(2)]
        public sbyte value_i8_2;
        [FieldOffset(2)]
        public byte value_u8_2;
        [FieldOffset(2)]
        public bool value_b2;
        [FieldOffset(3)]
        public sbyte value_i8_3;
        [FieldOffset(3)]
        public byte value_u8_3;
        [FieldOffset(3)]
        public bool value_b3;
        [FieldOffset(4)]
        public sbyte value_i8_4;
        [FieldOffset(4)]
        public byte value_u8_4;
        [FieldOffset(4)]
        public bool value_b4;
        [FieldOffset(5)]
        public sbyte value_i8_5;
        [FieldOffset(5)]
        public byte value_u8_5;
        [FieldOffset(5)]
        public bool value_b5;
        [FieldOffset(6)]
        public sbyte value_i8_6;
        [FieldOffset(6)]
        public byte value_u8_6;
        [FieldOffset(6)]
        public bool value_b6;
        [FieldOffset(7)]
        public sbyte value_i8_7;
        [FieldOffset(7)]
        public byte value_u8_7;
        [FieldOffset(7)]
        public bool value_b7;
        #endregion

        #region constructors
        public UnionDecimal(long value) : this() => value_i64 = value;
        public UnionDecimal(ulong value) : this() => value_u64 = value;
        public UnionDecimal(double value) : this() => value_f64 = value;

        public UnionDecimal(int value) : this() => value_i32_0 = value;
        public UnionDecimal(uint value) : this() => value_u32_0 = value;
        public UnionDecimal(float value) : this() => value_f32_0 = value;

        public UnionDecimal(short value) : this() => value_i16_0 = value;
        public UnionDecimal(ushort value) : this() => value_u16_0 = value;
        public UnionDecimal(char value) : this() => value_c0 = value;

        public UnionDecimal(sbyte value) : this() => value_i8_0 = value;
        public UnionDecimal(byte value) : this() => value_u8_0 = value;
        public UnionDecimal(bool value) : this() => value_b0 = value;
        #endregion

        public override bool Equals(object obj) => obj is UnionDecimal mask && mask.value_u64 == value_u64;
        public override int GetHashCode() => value_u64.GetHashCode();
        public override string ToString() => value_u64.ToString();
        public string ToString(string format) => value_u64.ToString(format);
        public string ToString(string format, IFormatProvider formatProvider) => value_u64.ToString(format, formatProvider);

        public UnionDecimal Clone() => new UnionDecimal(value_u64);

        public T GetValue<T>(int idx) where T : unmanaged
        {
            if (idx < 0 || sizeof(T) + idx >= sizeof(long))
                throw new InvalidOperationException();
            fixed (byte* ptr = &value_u8_0)
                return *(T*)(ptr + idx);
        }
        public void SetValue<T>(int idx, T value) where T : unmanaged
        {
            if (idx < 0 || sizeof(T) + idx >= sizeof(long))
                throw new InvalidOperationException();
            fixed (byte* ptr = &value_u8_0)
                *(T*)(ptr + idx) = value;
        }
        public ref T RefValue<T>(int idx) where T : unmanaged
        {
            if (idx < 0 || sizeof(T) + idx >= sizeof(long))
                throw new InvalidOperationException();
            fixed (byte* ptr = &value_u8_0)
                return ref *(T*)(ptr + idx);
        }
        public T* AsPointer<T>(int idx) where T : unmanaged
        {
            if (idx < 0 || sizeof(T) + idx >= sizeof(long))
                throw new InvalidOperationException();
            fixed (byte* ptr = &value_u8_0)
                return (T*)(ptr + idx);
        }

        #region OPERATORS (BITMASK)
        public static bool operator ==(UnionDecimal left, UnionDecimal right) => left.value_u64 == right.value_u64;
        public static bool operator !=(UnionDecimal left, UnionDecimal right) => left.value_u64 != right.value_u64;
        public static bool operator <(UnionDecimal left, UnionDecimal right) => left.value_u64 < right.value_u64;
        public static bool operator >(UnionDecimal left, UnionDecimal right) => left.value_u64 > right.value_u64;
        public static bool operator <=(UnionDecimal left, UnionDecimal right) => left.value_u64 <= right.value_u64;
        public static bool operator >=(UnionDecimal left, UnionDecimal right) => left.value_u64 >= right.value_u64;
        #endregion

        #region disable
#if false
        #region OPERATORS (INT64)
        public static bool operator ==(BitMask left, long right) => left.value_i64 == right;
        public static bool operator !=(BitMask left, long right) => left.value_i64 != right;
        public static bool operator <(BitMask left, long right) => left.value_i64 < right;
        public static bool operator >(BitMask left, long right) => left.value_i64 > right;
        public static bool operator <=(BitMask left, long right) => left.value_i64 <= right;
        public static bool operator >=(BitMask left, long right) => left.value_i64 >= right;

        public static BitMask operator |(BitMask left, long right) => new BitMask { value_i64 = left.value_i64 | right };
        public static BitMask operator &(BitMask left, long right) => new BitMask { value_i64 = left.value_i64 & right };
        public static BitMask operator ^(BitMask left, long right) => new BitMask { value_i64 = left.value_i64 ^ right };

        public static BitMask operator +(BitMask left, long right) => new BitMask { value_i64 = left.value_i64 + right };
        public static BitMask operator -(BitMask left, long right) => new BitMask { value_i64 = left.value_i64 - right };
        public static BitMask operator *(BitMask left, long right) => new BitMask { value_i64 = left.value_i64 * right };
        public static BitMask operator /(BitMask left, long right) => new BitMask { value_i64 = left.value_i64 / right };
        public static BitMask operator %(BitMask left, long right) => new BitMask { value_i64 = left.value_i64 % right };
        #endregion
        
        #region OPERATORS (UINT64)
        public static bool operator ==(BitMask left, ulong right) => left.value_u64 == right;
        public static bool operator !=(BitMask left, ulong right) => left.value_u64 != right;
        public static bool operator <(BitMask left, ulong right) => left.value_u64 < right;
        public static bool operator >(BitMask left, ulong right) => left.value_u64 > right;
        public static bool operator <=(BitMask left, ulong right) => left.value_u64 <= right;
        public static bool operator >=(BitMask left, ulong right) => left.value_u64 >= right;

        public static BitMask operator |(BitMask left, ulong right) => new BitMask { value_u64 = left.value_u64 | right };
        public static BitMask operator &(BitMask left, ulong right) => new BitMask { value_u64 = left.value_u64 & right };
        public static BitMask operator ^(BitMask left, ulong right) => new BitMask { value_u64 = left.value_u64 ^ right };

        public static BitMask operator +(BitMask left, ulong right) => new BitMask { value_u64 = left.value_u64 + right };
        public static BitMask operator -(BitMask left, ulong right) => new BitMask { value_u64 = left.value_u64 - right };
        public static BitMask operator *(BitMask left, ulong right) => new BitMask { value_u64 = left.value_u64 * right };
        public static BitMask operator /(BitMask left, ulong right) => new BitMask { value_u64 = left.value_u64 / right };
        public static BitMask operator %(BitMask left, ulong right) => new BitMask { value_u64 = left.value_u64 % right };
        #endregion
        
        #region OPERATORS (FLOAT64)
        public static bool operator ==(BitMask left, double right) => left.value_f64 == right;
        public static bool operator !=(BitMask left, double right) => left.value_f64 != right;
        public static bool operator <(BitMask left, double right) => left.value_f64 < right;
        public static bool operator >(BitMask left, double right) => left.value_f64 > right;
        public static bool operator <=(BitMask left, double right) => left.value_f64 <= right;
        public static bool operator >=(BitMask left, double right) => left.value_f64 >= right;
        #endregion
        
        #region OPERATORS (INT32)
        public static bool operator ==(BitMask left, int right) => left.value_i32_0 == right;
        public static bool operator !=(BitMask left, int right) => left.value_i32_0 != right;
        public static bool operator <(BitMask left, int right) => left.value_i32_0 < right;
        public static bool operator >(BitMask left, int right) => left.value_i32_0 > right;
        public static bool operator <=(BitMask left, int right) => left.value_i32_0 <= right;
        public static bool operator >=(BitMask left, int right) => left.value_i32_0 >= right;

        public static BitMask operator |(BitMask left, int right) => new BitMask(left.value_u64) { value_i32_0 = left.value_i32_0 | right };
        public static BitMask operator &(BitMask left, int right) => new BitMask(left.value_u64) { value_i32_0 = left.value_i32_0 & right };
        public static BitMask operator ^(BitMask left, int right) => new BitMask(left.value_u64) { value_i32_0 = left.value_i32_0 ^ right };

        public static BitMask operator +(BitMask left, int right) => new BitMask(left.value_u64) { value_i32_0 = left.value_i32_0 + right };
        public static BitMask operator -(BitMask left, int right) => new BitMask(left.value_u64) { value_i32_0 = left.value_i32_0 - right };
        public static BitMask operator *(BitMask left, int right) => new BitMask(left.value_u64) { value_i32_0 = left.value_i32_0 * right };
        public static BitMask operator /(BitMask left, int right) => new BitMask(left.value_u64) { value_i32_0 = left.value_i32_0 / right };
        public static BitMask operator %(BitMask left, int right) => new BitMask(left.value_u64) { value_i32_0 = left.value_i32_0 % right };
        #endregion
        
        #region OPERATORS (UINT32)
        public static bool operator ==(BitMask left, uint right) => left.value_u32_0 == right;
        public static bool operator !=(BitMask left, uint right) => left.value_u32_0 != right;
        public static bool operator <(BitMask left, uint right) => left.value_u32_0 < right;
        public static bool operator >(BitMask left, uint right) => left.value_u32_0 > right;
        public static bool operator <=(BitMask left, uint right) => left.value_u32_0 <= right;
        public static bool operator >=(BitMask left, uint right) => left.value_u32_0 >= right;

        public static BitMask operator |(BitMask left, uint right) => new BitMask(left.value_u64) { value_u32_0 = left.value_u32_0 | right };
        public static BitMask operator &(BitMask left, uint right) => new BitMask(left.value_u64) { value_u32_0 = left.value_u32_0 & right };
        public static BitMask operator ^(BitMask left, uint right) => new BitMask(left.value_u64) { value_u32_0 = left.value_u32_0 ^ right };

        public static BitMask operator +(BitMask left, uint right) => new BitMask(left.value_u64) { value_u32_0 = left.value_u32_0 + right };
        public static BitMask operator -(BitMask left, uint right) => new BitMask(left.value_u64) { value_u32_0 = left.value_u32_0 - right };
        public static BitMask operator *(BitMask left, uint right) => new BitMask(left.value_u64) { value_u32_0 = left.value_u32_0 * right };
        public static BitMask operator /(BitMask left, uint right) => new BitMask(left.value_u64) { value_u32_0 = left.value_u32_0 / right };
        public static BitMask operator %(BitMask left, uint right) => new BitMask(left.value_u64) { value_u32_0 = left.value_u32_0 % right };
        #endregion
        
        #region OPERATORS (FLOAT32)
        public static bool operator ==(BitMask left, float right) => left.value_f32_0 == right;
        public static bool operator !=(BitMask left, float right) => left.value_f32_0 != right;
        public static bool operator <(BitMask left, float right) => left.value_f32_0 < right;
        public static bool operator >(BitMask left, float right) => left.value_f32_0 > right;
        public static bool operator <=(BitMask left, float right) => left.value_f32_0 <= right;
        public static bool operator >=(BitMask left, float right) => left.value_f32_0 >= right;
        #endregion
        
        #region OPERATORS (INT16)
        public static bool operator ==(BitMask left, short right) => left.value_i16_0 == right;
        public static bool operator !=(BitMask left, short right) => left.value_i16_0 != right;
        public static bool operator <(BitMask left, short right) => left.value_i16_0 < right;
        public static bool operator >(BitMask left, short right) => left.value_i16_0 > right;
        public static bool operator <=(BitMask left, short right) => left.value_i16_0 <= right;
        public static bool operator >=(BitMask left, short right) => left.value_i16_0 >= right;
        #endregion
        
        #region OPERATORS (UINT16)
        public static bool operator ==(BitMask left, ushort right) => left.value_u16_0 == right;
        public static bool operator !=(BitMask left, ushort right) => left.value_u16_0 != right;
        public static bool operator <(BitMask left, ushort right) => left.value_u16_0 < right;
        public static bool operator >(BitMask left, ushort right) => left.value_u16_0 > right;
        public static bool operator <=(BitMask left, ushort right) => left.value_u16_0 <= right;
        public static bool operator >=(BitMask left, ushort right) => left.value_u16_0 >= right;
        #endregion
        
        #region OPERATORS (CHAR)
        public static bool operator ==(BitMask left, char right) => left.value_c0 == right;
        public static bool operator !=(BitMask left, char right) => left.value_c0 != right;
        public static bool operator <(BitMask left, char right) => left.value_c0 < right;
        public static bool operator >(BitMask left, char right) => left.value_c0 > right;
        public static bool operator <=(BitMask left, char right) => left.value_c0 <= right;
        public static bool operator >=(BitMask left, char right) => left.value_c0 >= right;
        #endregion
        
        #region OPERATORS (UINT8)
        public static bool operator ==(BitMask left, byte right) => left.value_u8_0 == right;
        public static bool operator !=(BitMask left, byte right) => left.value_u8_0 != right;
        public static bool operator <(BitMask left, byte right) => left.value_u8_0 < right;
        public static bool operator >(BitMask left, byte right) => left.value_u8_0 > right;
        public static bool operator <=(BitMask left, byte right) => left.value_u8_0 <= right;
        public static bool operator >=(BitMask left, byte right) => left.value_u8_0 >= right;
        #endregion
        
        #region OPERATORS (INT8)
        public static bool operator ==(BitMask left, sbyte right) => left.value_i8_0 == right;
        public static bool operator !=(BitMask left, sbyte right) => left.value_i8_0 != right;
        public static bool operator <(BitMask left, sbyte right) => left.value_i8_0 < right;
        public static bool operator >(BitMask left, sbyte right) => left.value_i8_0 > right;
        public static bool operator <=(BitMask left, sbyte right) => left.value_i8_0 <= right;
        public static bool operator >=(BitMask left, sbyte right) => left.value_i8_0 >= right;
        #endregion
        
        #region BIT-OPERATORS
public static BitMask operator <<(BitMask mask, int shift) => new BitMask { value_u64 = mask.value_u64 << shift };
public static BitMask operator >>(BitMask mask, int shift) => new BitMask { value_u64 = mask.value_u64 >> shift };
public static BitMask operator !(BitMask mask) => new BitMask { value_u64 = ~mask.value_u64 };
public static BitMask operator ~(BitMask mask) => new BitMask { value_u64 = mask.value_u64 };
        #endregion
#endif
        #endregion

        public static implicit operator bool(UnionDecimal mask) => mask.value_b0;
        public static implicit operator byte(UnionDecimal mask) => mask.value_u8_0;
        public static implicit operator sbyte(UnionDecimal mask) => mask.value_i8_0;
        public static implicit operator ushort(UnionDecimal mask) => mask.value_u16_0;
        public static implicit operator short(UnionDecimal mask) => mask.value_i16_0;
        public static implicit operator char(UnionDecimal mask) => mask.value_c0;
        public static implicit operator uint(UnionDecimal mask) => mask.value_u32_0;
        public static implicit operator int(UnionDecimal mask) => mask.value_i32_0;
        public static implicit operator float(UnionDecimal mask) => mask.value_f32_0;
        public static implicit operator ulong(UnionDecimal mask) => mask.value_u64;
        public static implicit operator long(UnionDecimal mask) => mask.value_i64;
        public static implicit operator double(UnionDecimal mask) => mask.value_f64;

        public static explicit operator bool*(UnionDecimal mask) => &mask.value_b0;
        public static explicit operator byte*(UnionDecimal mask) => &mask.value_u8_0;
        public static explicit operator sbyte*(UnionDecimal mask) => &mask.value_i8_0;
        public static explicit operator ushort*(UnionDecimal mask) => &mask.value_u16_0;
        public static explicit operator short*(UnionDecimal mask) => &mask.value_i16_0;
        public static explicit operator char*(UnionDecimal mask) => &mask.value_c0;
        public static explicit operator uint*(UnionDecimal mask) => &mask.value_u32_0;
        public static explicit operator int*(UnionDecimal mask) => &mask.value_i32_0;
        public static explicit operator float*(UnionDecimal mask) => &mask.value_f32_0;
        public static explicit operator ulong*(UnionDecimal mask) => &mask.value_u64;
        public static explicit operator long*(UnionDecimal mask) => &mask.value_i64;
        public static explicit operator double*(UnionDecimal mask) => &mask.value_f64;

        public static implicit operator UnionDecimal(bool data) => new UnionDecimal { value_b0 = data };
        public static implicit operator UnionDecimal(byte data) => new UnionDecimal { value_u8_0 = data };
        public static implicit operator UnionDecimal(sbyte data) => new UnionDecimal { value_i8_0 = data };
        public static implicit operator UnionDecimal(char data) => new UnionDecimal { value_c0 = data };
        public static implicit operator UnionDecimal(ushort data) => new UnionDecimal { value_u16_0 = data };
        public static implicit operator UnionDecimal(short data) => new UnionDecimal { value_i16_0 = data };
        public static implicit operator UnionDecimal(int data) => new UnionDecimal { value_i32_0 = data };
        public static implicit operator UnionDecimal(uint data) => new UnionDecimal { value_u32_0 = data };
        public static implicit operator UnionDecimal(float data) => new UnionDecimal { value_f32_0 = data };
        public static implicit operator UnionDecimal(ulong data) => new UnionDecimal { value_u64 = data };
        public static implicit operator UnionDecimal(long data) => new UnionDecimal { value_i64 = data };
        public static implicit operator UnionDecimal(double data) => new UnionDecimal { value_f64 = data };
        public static implicit operator UnionDecimal(string data) => new UnionDecimal { value_i32_0 = data.GetHashCode(), value_i32_4 = data.Length };
    }
}
