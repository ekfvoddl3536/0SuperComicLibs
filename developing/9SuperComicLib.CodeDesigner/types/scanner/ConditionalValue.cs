using System;
using System.Runtime.InteropServices;
using SuperComicLib.Core;

namespace SuperComicLib.CodeDesigner
{
    public enum ConstantType
    {
        NONE,
        BOOLEAN,
        INT32,
        UINT32
    }

    [StructLayout(LayoutKind.Explicit)]
    public readonly struct ConditionalValue : IEquatable<ConstantType>, IEquatable<ConditionalValue>, IComparable<ConditionalValue>
    {
        public static readonly ConditionalValue Empty = default;
        public static readonly ConditionalValue TRUE = new ConditionalValue(ConstantType.BOOLEAN, 1);
        public static readonly ConditionalValue FALSE = new ConditionalValue(ConstantType.BOOLEAN, 0);

        [FieldOffset(0)]
        public readonly ConstantType attrb;
        [FieldOffset(4)]
        public readonly int value_i;
        [FieldOffset(4)]
        public readonly uint value_u;

        #region Constructor
        private ConditionalValue(ConstantType type, int v)
        {
            attrb = type;
            value_u = 0u;
            value_i = v;
        }

        public ConditionalValue(int v)
        {
            attrb = ConstantType.INT32;
            value_u = 0u;
            value_i = v;
        }

        public ConditionalValue(uint v)
        {
            attrb = ConstantType.UINT32;
            value_i = 0;
            value_u = v;
        }
        #endregion

        #region method
        public int CompareTo(ConditionalValue other) =>
            attrb == ConstantType.INT32
            ? other.value_i.CompareTo(value_i)
            : other.value_u.CompareTo(value_u);

        public bool Equals(ConditionalValue other) =>
            other.attrb == attrb &&
            other.value_u == value_u;

        public bool Equals(ConstantType other) => attrb == other;

        public override bool Equals(object obj) =>
            obj is ConditionalValue k && k == this;

        public override int GetHashCode()
        {
            int result = 7;
            IntHash.Combine(ref result, (int)attrb);
            IntHash.Combine(ref result, value_i);

            return result;
        }
        #endregion

        #region override
        public static bool operator <(ConditionalValue left, ConditionalValue right) => left.CompareTo(right) < 0;
        public static bool operator >(ConditionalValue left, ConditionalValue right) => left.CompareTo(right) > 0;
        public static bool operator <=(ConditionalValue left, ConditionalValue right) => left.CompareTo(right) <= 0;
        public static bool operator >=(ConditionalValue left, ConditionalValue right) => left.CompareTo(right) >= 0;

        public static bool operator ==(ConditionalValue left, ConditionalValue right) => left.Equals(right);
        public static bool operator !=(ConditionalValue left, ConditionalValue right) => !left.Equals(right);
        #endregion

        #region static
        public static ConditionalValue Parse(string value) =>
            int.TryParse(value, out int v1)
            ? new ConditionalValue(v1)
            : uint.TryParse(value, out uint v2)
            ? new ConditionalValue(v2)
            : Empty;

        public static bool TryParse(string value, out ConditionalValue result)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result = Empty;
                return value != null;
            }

            result = Parse(value);
            return result.attrb != 0;
        }
        #endregion
    }
}
