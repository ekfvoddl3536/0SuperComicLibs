using System;
using System.Linq;
using System.Text;

namespace SuperComicLib.Numerics
{
    public sealed unsafe class Amd64Bits : Bits
    {
        private const byte size = 64;
        private const ulong flag = 1;

        private int m_length;
        private ulong[] m_arr;

        #region constructors
        public Amd64Bits(ulong[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(array));

            m_arr = array;
            m_length = array.Length;
        }

        public Amd64Bits(int initBits)
        {
            if (initBits < size)
                throw new ArgumentOutOfRangeException(nameof(initBits));
            if (initBits % size != 0)
                throw new ArgumentException("Size must be a multiple of 64", nameof(initBits));

            int temp = initBits / size;
            m_arr = new ulong[temp];
            m_length = temp;
        }

        public Amd64Bits(ulong[] array, int arrayCount)
        {
            m_arr = array;
            m_length = arrayCount;
        }
        #endregion

        #region overrides
        public override void Clear() => Array.Clear(m_arr, 0, m_length);

        public override void SetAll(bool value)
        {
            int x = m_length;
            if (value)
            {
                while (--x >= 0)
                    m_arr[x] = ulong.MaxValue;
            }
            else
            {
                while (--x >= 0)
                    m_arr[x] = 0;
            }
        }

        public override void AND(Bits other)
        {
            if (!(other is Amd64Bits item) || item.m_length != m_length)
                return;

            int x = m_length;
            ulong[] src = m_arr;
            ulong[] dst = item.m_arr;
            while (--x >= 0)
                src[x] &= dst[x];
        }

        public override void OR(Bits other)
        {
            if (!(other is Amd64Bits item) || item.m_length != m_length)
                return;

            int x = m_length;
            ulong[] src = m_arr;
            ulong[] dst = item.m_arr;
            while (--x >= 0)
                src[x] |= dst[x];
        }

        public override void XOR(Bits other)
        {
            if (!(other is Amd64Bits item) || item.m_length != m_length)
                return;

            int x = m_length;
            ulong[] src = m_arr;
            ulong[] dst = item.m_arr;
            while (--x >= 0)
                src[x] ^= dst[x];
        }

        public override void NOT()
        {
            int x = m_length;
            while (--x >= 0)
                m_arr[x] = ~m_arr[x];
        }

        public override void LSHIFT(int count)
        {
            ulong[] vs = m_arr;

            int lsh = count & 0x3F;
            int rsh = 64 - lsh;

            int di = m_length - 1;
            int si = di - (count >> 6);

            for (; si > 0; si--, di--)
                vs[di] =
                    (vs[si] << lsh) |
                    (vs[si - 1] >> rsh);

            vs[di] = vs[si] << lsh;

            while (--di >= 0)
                vs[di] = 0;
        }

        public override void RSHIFT(int count)
        {
            ulong[] vs = m_arr;

            int rsh = count & 0x3F;
            int lsh = 64 - rsh;

            int di = 0;
            int si = di + (count >> 6);

            int max = m_length;
            for (max--; si < max; si++, di++)
                vs[di] =
                    (vs[si] >> rsh) |
                    (vs[si + 1] << lsh);

            vs[di] = vs[si] >> rsh;

            while (++di <= max)
                vs[di] = 0;
        }

        public override bool Equals(Bits other) => other is Amd64Bits item && Equals(item);

        public bool Equals(Amd64Bits other) =>
            m_length == other.m_length &&
            m_arr.SequenceEqual(other.m_arr);

        public override void CopyFrom(Bits other)
        {
            if (!(other is Amd64Bits item) || item.m_length != m_length)
                return;

            Array.Copy(item.m_arr, m_arr, m_length);
        }

        public override Bits Clone() => new Amd64Bits(ToLongArray());

        public override unsafe uint[] ToArray()
        {
            int x = m_length;
            ulong[] src = m_arr;
            uint[] vs = new uint[x * 2];
            fixed (uint* ptr = &vs[0])
            {
                ulong* dptr = (ulong*)ptr;
                while (--x >= 0)
                    dptr[x] = src[x];
            }

            return vs;
        }

        public ulong[] ToLongArray()
        {
            ulong[] vs = new ulong[m_length];
            Array.Copy(m_arr, vs, m_length);
            return vs;
        }
        #endregion

        #region default methods
        public override int Length => m_length;

        public override bool this[int bitPosition]
        {
            get => InternalGet(bitPosition);
            set => InternalSet(bitPosition, value);
        }

        private void InternalSet(int bitPosition, bool value)
        {
            int index = bitPosition / size;
            if (index < m_length && index >= 0)
                if (value)
                    m_arr[index] |= flag << (bitPosition % size);
                else
                    m_arr[index] &= ~(flag << (bitPosition % size));
        }

        private bool InternalGet(int bitPosition)
        {
            int index = bitPosition / size;
            return
                index < m_length && 
                index >= 0 &&
                (m_arr[index] & (flag << (bitPosition % size))) != 0;
        }

        public override void MarkBit(int bitPosition)
        {
            int index = bitPosition / size;
            if (index < m_length && index >= 0)
                m_arr[index] |= flag << (bitPosition % size);
        }
        #endregion

        #region other methods
        protected override void Dispose(bool disposing)
        {
            m_length = 0;
            m_arr = null;
        }

        public override string ToString()
        {
            int x = m_length;
            StringBuilder sb = new StringBuilder(x * size);

            ulong[] vs = m_arr;
            while (--x > 0)
            {
                sb.Append(vs[x].ToString("X16"));
                sb.Append(' ');
            }
            sb.Append(vs[0].ToString("X16"));

            return sb.ToString();
        }
        #endregion
    }
}
