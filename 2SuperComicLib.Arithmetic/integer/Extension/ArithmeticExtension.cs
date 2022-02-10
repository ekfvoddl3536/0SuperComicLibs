namespace SuperComicLib.Arithmetic
{
    public static unsafe class ArithmeticExtension
    {
        public unsafe static T DIVMOD_EX<T>(this ref T value_MODResult, in T den) where T : unmanaged, ICustomInteger
        {
            T divResult = default;
            fixed (T* num1 = &value_MODResult)
            fixed (T* num2 = &den)
                BigIntArithmetic.DIVMOD((ulong*)num1, (ulong*)num2, (ulong*)&divResult, value_MODResult.Length64);

            return divResult;
        }

        /// <summary>
        /// ADD FAST (Extension)
        /// </summary>
        public unsafe static void ADDF_EX<T>(this ref T source, int addvalue) where T : unmanaged, ICustomInteger
        {
            fixed (T* ptr = &source)
                BigIntArithmetic.AddFast((uint*)ptr, (uint)addvalue, source.Length32);
        }

        /// <summary>
        /// SUB FAST (Extension)
        /// </summary>
        public unsafe static void SUBF_EX<T>(this ref T source, int addvalue) where T : unmanaged, ICustomInteger
        {
            fixed (T* ptr = &source)
                BigIntArithmetic.SubFast((uint*)ptr, (uint)addvalue, source.Length32);
        }
    }
}
