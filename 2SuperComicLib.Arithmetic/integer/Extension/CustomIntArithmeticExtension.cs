// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
