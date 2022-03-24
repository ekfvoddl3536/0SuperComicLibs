//using System;
//using System.Runtime.InteropServices;
//using System.Text;

//namespace SuperComicLib.Arithmetic
//{
//    [StructLayout(LayoutKind.Sequential, Pack = sizeof(ushort))]
//    public readonly unsafe struct Half
//    {
//        private const int flag_exp = 0x7C_00;
//        private const int flag_man = 0x03_FF;

//        public static readonly Half PositiveInfinity = new Half(flag_exp);
//        public static readonly Half NegativeInfinity = -PositiveInfinity;
//        public static readonly Half MaxValue = new Half(0x78_FF);
//        public static readonly Half MinValue = -MaxValue;
//        // public static readonly Half LargestSubnormal = new Half(0x3_FF);
//        public static readonly Half NaN = new Half(flag_man | 1);

//        private readonly ushort v;

//        private Half(int v) => this.v = (ushort)v;

//        private Half(int s, int e, int m)
//        {
//            if (e == flag_exp) // inf
//                v = (ushort)(s | e);
//            else
//                v = (ushort)(s | e | m);
//        }

//        #region override
//        public override bool Equals(object obj) => throw new Exception();

//        public override int GetHashCode() => v;

//        public override string ToString()
//        {
//            int v = this.v;
//            if (v == 0)
//                return "0";

//            StringBuilder sb = new StringBuilder(64);

//            if (v > short.MaxValue)
//                sb.Append('-');

//            int exp = ((v & flag_exp) >> 10) - 25;
//            int man = 0x400 | (v & flag_man);
//            HalfToString.Integer(sb, exp, man);
//            HalfToString.Mantissa(sb, exp, man);

//            return sb.ToString();
//        }
//        #endregion

//        #region type convert
//        public static implicit operator Half(float v)
//        {
//            int k = *(int*)&v;
//            if (k == 0)
//                return default;

//            int e = (k & 0x7F80_0000) >> 23;

//            if (e > 142)
//                return
//                    k < 0
//                    ? NegativeInfinity
//                    : PositiveInfinity;
//            else if (e < 112)
//                return NaN;

//            return
//                new Half(
//                    (k >> 16) & short.MinValue,
//                    (e - 112) << 10,
//                    (k & 0x7F_FFFF) >> 13);
//        }
//        public static implicit operator Half(int v)
//        {
//            if (v == 0)
//                return default;

//            int absK = Math.Abs(v);

//            int k = 31 - absK.FLS();
//            if (k > 15)
//                return
//                    v < 0
//                    ? NegativeInfinity
//                    : PositiveInfinity;

//            int data = absK & ((1 << k) - 1);
//            if (k > 10)
//                data >>= k - 10;

//            return
//                new Half(
//                    (v >> 16) & short.MinValue,
//                    (k + 15) << 10,
//                    data);
//        }

//        public static explicit operator float(Half value)
//        {
//            int v = value.v;

//            int exp = ((v & flag_exp) >> 10) - 15;
//            int k =
//                ((v & 0x8000) << 16) |
//                ((exp + 127) << 23) |
//                ((v & flag_man) << 13);

//            return *(float*)&k;
//        }
//        #endregion

//        #region static method
//        public static bool IsNaN(Half value) =>
//            (value.v & (flag_exp | flag_man)) > flag_exp;

//        public static bool IsInfinity(Half value) =>
//            (value.v & (flag_exp | flag_man)) == flag_exp;

//        public static bool IsPositiveInfinity(Half value) => value.v == flag_exp;
//        public static bool IsNegativeInfinity(Half value) => value.v == (0x8000 | flag_exp);
//        #endregion

//        #region compare
//        public static bool operator ==(Half left, Half right) => left.v == right.v;
//        public static bool operator !=(Half left, Half right) => left.v != right.v;

//        public static bool operator >(Half left, Half right) => !IsNaN(left) && (IsNaN(right) || left.v > right.v);
//        public static bool operator <(Half left, Half right) => !IsNaN(right) && (IsNaN(left) || left.v < right.v);

//        public static bool operator >=(Half left, Half right) => left == right || left > right;
//        public static bool operator <=(Half left, Half right) => left == right || left < right;
//        #endregion

//        #region arthmetic
//        public static Half operator -(Half value) => new Half(value.v ^ 0x8000);
//        public static Half operator +(Half value) => value;

//        public static Half operator +(Half left, Half right) => (float)left + (float)right;
//        public static Half operator -(Half left, Half right) => (float)left - (float)right;

//        public static Half operator *(Half left, Half right) => (float)left * (float)right;
//        public static Half operator /(Half left, Half right) => (float)left / (float)right;
//        public static Half operator %(Half left, Half right) => (float)left % (float)right;
//        #endregion
//    }
//}
