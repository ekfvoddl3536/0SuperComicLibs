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
