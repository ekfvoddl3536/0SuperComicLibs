using System;
using System.Linq;
using System.Text;

namespace SuperComicLib.Numerics
{
    public sealed class X86Bits : Bits, IEquatable<X86Bits>
    {
        private const byte size = 32;
        private const uint flag = 1;

        private int m_length;
        private uint[] m_arr;

        #region constructors
        public X86Bits(uint[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(array));

            m_arr = array;
            m_length = array.Length;
        }

        public X86Bits(int initBits)
        {
            if (initBits < size)
                throw new ArgumentOutOfRangeException(nameof(initBits));
            if (initBits % size != 0)
                throw new ArgumentException("Size must be a multiple of 32", nameof(initBits));

            int temp = initBits / size;
            m_arr = new uint[temp];
            m_length = temp;
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

        #region overrides
        public override void Clear() => Array.Clear(m_arr, 0, m_length);

        public override void SetAll(bool value)
        {
            int x = m_length;
            if (value)
            {
                while (--x >= 0)
                    m_arr[x] = uint.MaxValue;
            }
            else
            {
                while (--x >= 0)
                    m_arr[x] = 0;
            }
        }

        public override void AND(Bits other)
        {
            if (!(other is X86Bits item) || item.m_length != m_length)
                return;

            int x = m_length;
            uint[] src = m_arr;
            uint[] dst = item.m_arr;
            while (--x >= 0)
                src[x] &= dst[x];
        }

        public override void OR(Bits other)
        {
            if (!(other is X86Bits item) || item.m_length != m_length)
                return;

            int x = m_length;
            uint[] src = m_arr;
            uint[] dst = item.m_arr;
            while (--x >= 0)
                src[x] |= dst[x];
        }

        public override void XOR(Bits other)
        {
            if (!(other is X86Bits item) || item.m_length != m_length)
                return;

            int x = m_length;
            uint[] src = m_arr;
            uint[] dst = item.m_arr;
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
            uint[] vs = m_arr;

            int len = count / size;
            vs.Push(len);

            int cnt = count % size;
            if (cnt > 0)
            {
                int x = m_length - 1;
                int cnt2 = size - cnt;
                while (--x >= len)
                {
                    ref uint tmp = ref vs[x + 1];
                    tmp = (tmp << cnt) | (vs[x] >> cnt2);
                }

                vs[len] <<= cnt;
            }
        }

        public override void RSHIFT(int count)
        {
            uint[] vs = m_arr;

            int len = count / size;
            vs.Pull(len);

            int cnt = count % size;
            if (cnt > 0)
            {
                int max = m_length - len - 1;
                int cnt2 = size - cnt;
                for (int x = 0; x < max; x++)
                {
                    ref uint tmp = ref vs[x];
                    tmp = (tmp >> cnt) | (vs[x + 1] << cnt2);
                }

                vs[max] >>= cnt;
            }
        }

        public override bool Equals(Bits other) => other is X86Bits item && Equals(item);

        public bool Equals(X86Bits other) =>
            m_length == other.m_length &&
            m_arr.SequenceEqual(other.m_arr);

        public override void CopyFrom(Bits other)
        {
            if (!(other is X86Bits item) || item.m_length != m_length)
                return;

            Array.Copy(item.m_arr, m_arr, m_length);
        }

        public override Bits Clone() => new X86Bits(ToArray());

        public override uint[] ToArray()
        {
            uint[] vs = new uint[m_length];
            Array.Copy(m_arr, vs, m_length);
            return vs;
        }
        #endregion

        #region other methods
        protected override void Dispose(bool disposing)
        {
            m_length = 0;
            m_arr = null;
        }

        public override bool Equals(object obj) =>
            obj is X86Bits item && Equals(item);

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString()
        {
            int x = m_length;
            StringBuilder sb = new StringBuilder(x * size);

            uint[] vs = m_arr;
            while (--x > 0)
            {
                sb.Append(vs[x].ToString("X8"));
                sb.Append(' ');
            }
            sb.Append(vs[0].ToString("X8"));

            return sb.ToString();
        }
        #endregion
    }
}
