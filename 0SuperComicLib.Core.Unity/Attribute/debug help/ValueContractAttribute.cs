#region LICENSE
/* MIT License
 * 
 * Copyright (c) 2021 SuperComic <ekfvoddl3535@naver.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE
 */
#endregion

using System;
using System.Collections;

namespace SuperComicWorld
{
    public sealed class ValueContractAttribute : FieldBaseAttribute
    {
        public string Message;
        public bool ThrowException;
        internal readonly object otherValue;
        internal readonly ValueCompareMode mode;

        /// <summary>
        /// 필드가 좌측 피연산자가 됩니다.
        /// <para/>
        /// 가령 '<see cref="ValueCompareMode.Lesser"/>'의 경우
        /// '필드 값이 <paramref name="otherValue"/> 보다 작다'라는 뜻이 됩니다.
        /// </summary>
        public ValueContractAttribute(object otherValue, ValueCompareMode mode)
        {
            this.otherValue = otherValue;

            if (mode < ValueCompareMode.Equals || mode > ValueCompareMode.GreatOrEqual)
                // 범위 초과
                this.mode = ValueCompareMode.Equals;
            else if (otherValue.GetType().IsValueType == false && mode > ValueCompareMode.Inequals)
                // 부적절한 비교연산자
                this.mode =
                    mode >= ValueCompareMode.Greater
                    ? ValueCompareMode.Equals
                    : ValueCompareMode.Inequals;
            else
                this.mode = mode;
        }

        public bool IsValidContract(Type fieldType) =>
            !fieldType.IsValueType || otherValue != null && otherValue.GetType() == fieldType;

        public bool Check(object fieldValue, Type fieldType)
        {
            int compResult =
                fieldType.IsValueType
                ? ValueContractHelper.Check(fieldValue, otherValue, fieldType)
                : Comparer.Default.Compare(fieldValue, otherValue);

            ValueCompareMode mode = this.mode;

            if (mode == ValueCompareMode.Equals) return compResult == 0;
            else if (mode == ValueCompareMode.Inequals) return compResult != 0;
            else if (mode == ValueCompareMode.Lesser) return compResult < 0;
            else if (mode == ValueCompareMode.LessOrEqual) return compResult <= 0;
            else if (mode == ValueCompareMode.Greater) return compResult > 0;
            else
                return compResult >= 0;
        }
    }
}