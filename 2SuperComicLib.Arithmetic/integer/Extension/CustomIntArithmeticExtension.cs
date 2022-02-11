namespace SuperComicLib.Arithmetic
{
    public static unsafe class CustomIntArithmeticExtension
    {
        public static T DIVMOD_EX<T>(this ref T value_MODResult, in T den) where T : unmanaged, ICustomInteger
        {
            T divResult = default;
            fixed (T* num1 = &value_MODResult)
            fixed (T* num2 = &den)
                BigIntArithmetic.DIVMOD((ulong*)num1, (ulong*)num2, (ulong*)&divResult, value_MODResult.Length64);

            return divResult;
        }

        public static void AddInt<T>(this ref T source, int value) where T : unmanaged, ICustomInteger
        {
            fixed (T* ptr = &source)
                BigIntArithmetic.AddFast((uint*)ptr, (uint)value, source.Length32);
        }

        public static void SubInt<T>(this ref T source, int value) where T : unmanaged, ICustomInteger
        {
            fixed (T* ptr = &source)
                BigIntArithmetic.SubFast((uint*)ptr, (uint)value, source.Length32);
        }

        public static void LogicORInt<T>(this ref T source, int value) where T : unmanaged, ICustomInteger
        {
            fixed (T* ptr = &source)
                *(int*)&ptr |= value;
        }

        public static void LogicANDInt<T>(this ref T source, int value) where T : unmanaged, ICustomInteger
        {
            fixed (T* ptr = &source)
                *(int*)&ptr &= value;
        }

        public static void LogicXORInt<T>(this ref T source, int value) where T : unmanaged, ICustomInteger
        {
            fixed (T* ptr = &source)
                *(int*)&ptr ^= value;
        }
    }
}
