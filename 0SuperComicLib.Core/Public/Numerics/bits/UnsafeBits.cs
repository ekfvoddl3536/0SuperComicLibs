using System;
using System.Text;

namespace SuperComicLib.Numerics
{
    public sealed unsafe class UnsafeBits : Bits, IEquatable<UnsafeBits>
    {
        private const byte size = 32;
        private const uint flag = 1;

        private int m_length;
        private uint* m_ptr;

        public UnsafeBits(uint* ptr, int length)
        {
#if DEBUG
            if (ptr == null)
                throw new ArgumentNullException(nameof(ptr));
            if (length == 0)
                throw new ArgumentOutOfRangeException(nameof(length));
#endif
            m_ptr = ptr;
            m_length = length;
        }

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
                    m_ptr[index] |= flag << (bitPosition % size);
                else
                    m_ptr[index] &= ~(flag << (bitPosition % size));
        }

        private bool InternalGet(int bitPosition)
        {
            int index = bitPosition / size;
            return
                index < m_length &&
                index >= 0 &&
                (m_ptr[index] & (flag << (bitPosition % size))) != 0;
        }

        public override void MarkBit(int bitPosition)
        {
            int index = bitPosition / size;
            if (index < m_length && index >= 0)
                m_ptr[index] |= flag << (bitPosition % size);
        }
        #endregion

        #region overrides
        public override void Clear()
        {
            int x = m_length;
            while (--x >= 0)
                m_ptr[x] = 0;
        }

        public override void SetAll(bool value)
        {
            int x = m_length;
            if (value)
            {
                while (--x >= 0)
                    m_ptr[x] = uint.MaxValue;
            }
            else
            {
                while (--x >= 0)
                    m_ptr[x] = 0;
            }
        }

        public override void AND(Bits other)
        {
            if (!(other is UnsafeBits item) || item.m_length != m_length)
                return;

            int x = m_length;
            uint* src = m_ptr;
            uint* dst = item.m_ptr;
            while (--x >= 0)
                src[x] &= dst[x];
        }

        public override void OR(Bits other)
        {
            if (!(other is UnsafeBits item) || item.m_length != m_length)
                return;

            int x = m_length;
            uint* src = m_ptr;
            uint* dst = item.m_ptr;
            while (--x >= 0)
                src[x] |= dst[x];
        }

        public override void XOR(Bits other)
        {
            if (!(other is UnsafeBits item) || item.m_length != m_length)
                return;

            int x = m_length;
            uint* src = m_ptr;
            uint* dst = item.m_ptr;
            while (--x >= 0)
                src[x] ^= dst[x];
        }

        public override void NOT()
        {
            int x = m_length;
            while (--x >= 0)
                m_ptr[x] = ~m_ptr[x];
        }

        public override void LSHIFT(int count) => throw new NotSupportedException();

        public override void RSHIFT(int count) => throw new NotSupportedException();

        public override bool Equals(Bits other) => other is UnsafeBits item && Equals(item);

        public bool Equals(UnsafeBits other)
        {
            if (m_length == other.m_length)
            {
                uint* me = m_ptr;
                uint* ot = other.m_ptr;
                for (int x = m_length; --x >= 0;)
                    if (me[x] != ot[x])
                        return false;

                return true;
            }
            return false;
        }

        public override void CopyFrom(Bits other)
        {
            if (!(other is UnsafeBits item) || item.m_length != m_length)
                return;

            uint* src = item.m_ptr;
            uint* dst = m_ptr;
            for (int x = m_length; --x >= 0;)
                dst[x] = src[x];
        }

        public override Bits Clone() => throw new NotSupportedException();

        public override uint[] ToArray()
        {
            int x = m_length;
            uint[] vs = new uint[x];
            fixed (uint* ptr = &vs[0])
            {
                uint* src = m_ptr;
                while (--x >= 0)
                    ptr[x] = src[x];
            }
            return vs;
        }
        #endregion

        #region other methods
        protected override void Dispose(bool disposing)
        {
            m_length = 0;
            m_ptr = null;
        }

        public override bool Equals(object obj) =>
            obj is UnsafeBits item && Equals(item);

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString()
        {
            int x = m_length;
            StringBuilder sb = new StringBuilder(x * size);

            uint* vs = m_ptr;
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
