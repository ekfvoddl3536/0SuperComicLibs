namespace SuperComicLib.Arithmetic
{
    public static class CommonArithmeticHelper
    {
        #region counter
        public static int BitHighZero(uint value)
        {
            if (value == 0)
                return 32;

            int bit = 0;
            if (value < 0x1_0000) // FFFF
            {
                bit += 16;
                value <<= 16;
            }
            if (value < 0x100_0000) // FF FFFF
            {
                bit += 8;
                value <<= 8;
            }
            if (value < 0x1000_0000) // FFF FFFF
            {
                bit += 4;
                value <<= 4;
            }
            if (value < 0x4000_0000) // 3FFF FFFF
            {
                bit += 2;
                value <<= 2;
            }

            return
                value <= int.MaxValue // 7FFF FFFF
                ? bit + 1
                : bit;
        }

        public static int BitHighZero(ulong value) =>
            value <= uint.MaxValue
            ? 32 + BitHighZero((uint)value)
            : BitHighZero((uint)(value >> 32));

        public static int BitLowZero(uint value)
        {
            int bit = 0;
            if ((value & 0xFFFF) == 0)
            {
                bit += 16;
                value >>= 16;
            }
            if ((value & 0xFF) == 0)
            {
                bit += 8;
                value >>= 8;
            }
            if ((value & 0xF) == 0)
            {
                bit += 4;
                value >>= 4;
            }
            if ((value & 0x3) == 0)
            {
                bit += 2;
                value >>= 2;
            }

            return
                (value & 1) == 0
                ? bit + 1
                : bit;
        }

        public static int BitLowZero(ulong value) =>
            value > uint.MaxValue
            ? 32 + BitLowZero((uint)(value >> 32))
            : BitLowZero((uint)value);
        #endregion

        #region carry add / mul
        public static uint CarrySum(ref uint current, uint add, uint carry)
        {
            ulong result = (ulong)current + add + carry;
            current = (uint)result;
            return (uint)(result >> 32);
        }

        public static uint CarrySum(ref uint current, uint carry) => 
            (uint)(((ulong)current + carry) >> 32);

        public static uint CarryMul(ref uint current, uint mul, uint carry)
        {
            ulong result = (ulong)current * mul + carry;
            current = (uint)result;
            return (uint)(result >> 32);
        }
        #endregion

        #region borrow sub / div
        public static uint BorrowSub(ref uint current, uint sub, uint borrow)
        {
            ulong result = (ulong)current - sub - borrow;
            current = (uint)result;
            return (uint)-(int)(result >> 32);
        }

        public static uint BorrowSub(ref uint current, uint borrow)
        {
            ulong result = (ulong)current - borrow;
            current = (uint)result;
            return (uint)-(int)(result >> 32);
        }
        #endregion

        public static unsafe void FastNOT(uint* value, int size)
        {
            ulong* ptr = (ulong*)value;
            for (; size >= 2; size -= 2, ptr++)
                *ptr = ~*ptr;

            if (size > 0)
                *(uint*)ptr ^= uint.MaxValue;
        }
    }
}
