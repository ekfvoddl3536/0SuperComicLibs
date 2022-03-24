using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public readonly unsafe struct BitArray : IEquatable<BitArray>, ICloneable
    {
        public readonly uint[] Values;

        public BitArray(BitArray other)
        {
            System.Diagnostics.Contracts.Contract.Requires(other.Values != null);
            System.Diagnostics.Contracts.Contract.Requires(other.Values.Length > 0);
            
            Values = (uint[])other.Values.Clone();
        }

        public BitArray(int memory_size)
        {
            System.Diagnostics.Contracts.Contract.Requires(memory_size > 0);

            Values = new uint[(int)CMath.Max((uint)memory_size, 1u)];
        }

        #region method 1
        public int Length => Values.Length;

        public bool this[int bitPosition]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                uint temp = (Values[bitPosition >> 5] >> (bitPosition & 0x1F)) & 1;
                return *(bool*)&temp;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                ref uint k = ref Values[bitPosition >> 5];
                k ^= k ^ ((uint)(*(byte*)&value & 1) << (bitPosition & 0x1F));
            }
        }

        public void Clear() => Array.Clear(Values, 0, Values.Length);

        public void SetAll(bool value)
        {
            uint val = (uint)~((*(byte*)&value & 1) - 1);
            
            var arr = Values;
            for (int i = arr.Length; --i >= 0;)
                arr[i] = val;
        }
        #endregion

        #region method 2
        public void AND(BitArray other)
        {
            uint[] left = Values;
            uint[] right = other.Values;

            for (int i = (int)CMath.Max((uint)left.Length, (uint)right.Length); --i >= 0;)
                left[i] &= right[i];
        }

        public void OR(BitArray other)
        {
            uint[] left = Values;
            uint[] right = other.Values;

            for (int i = (int)CMath.Max((uint)left.Length, (uint)right.Length); --i >= 0;)
                left[i] |= right[i];
        }

        public void XOR(BitArray other)
        {
            uint[] left = Values;
            uint[] right = other.Values;

            for (int i = (int)CMath.Max((uint)left.Length, (uint)right.Length); --i >= 0;)
                left[i] ^= right[i];
        }

        public void NOT()
        {
            uint[] left = Values;

            for (int i = left.Length; --i >= 0;)
                left[i] = ~left[i];
        }

        public void LSHIFT(int count)
        {
            uint[] left = Values;

            int i = left.Length;
            int arr_idx = (int)CMath.Min((uint)i, (uint)(count >> 5));

            Array.Copy(left, 0, left, arr_idx, i - arr_idx);
            Array.Clear(left, 0, arr_idx);

            int shift = count & 0x1F;
            if (--i >= 0 && shift > 0)
            {
                left[i] <<= shift;

                int u_shift = 32 - shift;

                for (uint temp; --i >= arr_idx;)
                {
                    temp = left[i] >> u_shift;
                    left[i + 1] |= temp;

                    left[i] <<= shift;
                }
            }
        }

        public void RSHIFT(int count)
        {
            uint[] left = Values;

            int i = left.Length;
            int arr_idx = (int)CMath.Min((uint)i, (uint)(count >> 5));

            int shift = --i - arr_idx;

            Array.Copy(left, i, left, shift, shift + 1);
            Array.Clear(left, i, arr_idx);

            arr_idx = shift;
            shift = count & 0x1F;

            if (shift > 0)
            {
                left[i = 0] >>= shift;

                int u_shift = 32 - shift;

                for (uint temp; ++i <= arr_idx;)
                {
                    temp = left[i] << u_shift;
                    left[i - 1] |= temp;

                    left[i] >>= shift;
                }
            }
        }
        #endregion

        #region method 3
        public bool Equals(BitArray other)
        {
            var left = Values;
            var right = other.Values;

            int x = left.Length;
            if (x != right.Length)
                return false;

            while (--x >= 0)
                if (left[x] != right[x])
                    return false;

            return true;
        }

        object ICloneable.Clone() => new BitArray(this);
        #endregion

        #region override
        public override int GetHashCode()
        {
            int hashCode = 0;

            var left = Values;
            for (int i = left.Length; --i >= 0;)
                hashCode ^= (int)left[i];

            return hashCode;
        }

        public override bool Equals(object obj) => 
            obj is BitArray other && Equals(other);
        #endregion

        #region math operator
        public static BitArray operator &(BitArray left, BitArray right)
        {
            BitArray v = new BitArray(left);
            v.AND(right);
            return v;
        }
        public static BitArray operator |(BitArray left, BitArray right)
        {
            BitArray v = new BitArray(left);
            v.OR(right);
            return v;
        }
        public static BitArray operator ^(BitArray left, BitArray right)
        {
            BitArray v = new BitArray(left);
            v.XOR(right);
            return v;
        }
        public static BitArray operator ~(BitArray left)
        {
            BitArray v = new BitArray(left);
            v.NOT();
            return v;
        }
        public static BitArray operator >>(BitArray left, int shift)
        {
            BitArray v = new BitArray(left);
            v.RSHIFT(shift);
            return v;
        }
        public static BitArray operator <<(BitArray left, int shift)
        {
            BitArray v = new BitArray(left);
            v.LSHIFT(shift);
            return v;
        }
        #endregion

        #region comparsion operator
        public static bool operator ==(BitArray left, BitArray right) => left.Equals(right);
        public static bool operator !=(BitArray left, BitArray right) => !left.Equals(right);
        #endregion
    }
}
