using System;

namespace SuperComicLib.Arithmetic
{
    public static unsafe class FastIntegerExtension
    {
        public static T PowBase2<T>(this T inst, int pow) where T : unmanaged, ICustomInteger
        {
            if (pow < 0)
                throw new ArgumentOutOfRangeException(nameof(pow));
            else if (pow >= inst.Length32)
                return default;

            T result;
            if (pow == 0)
                *(uint*)&result = 1;
            else
            {
                result = inst;
                ulong* ptr = (ulong*)&result;
                int max;
                if (BigIntArithmetic.IsZero(ptr, max = inst.Length64))
                    *(uint*)ptr = 1;

                BigIntArithmetic.LSHIFT(ptr, pow, max);
            }

            return result;
        }

        public static T PowBase2_Unsafe<T>(this T inst, int pow) where T : unmanaged, ICustomInteger
        {
            T result;
            if (pow <= 0)
                *(uint*)&result = 1;
            else
            {
                result = inst;
                BigIntArithmetic.LSHIFT((ulong*)&result, pow, inst.Length64);
            }

            return result;
        }
    }
}
