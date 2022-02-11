using System.Runtime.CompilerServices;

namespace SuperComicLib.Arithmetic
{
    public static class CommonArithmeticHelper
    {
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
