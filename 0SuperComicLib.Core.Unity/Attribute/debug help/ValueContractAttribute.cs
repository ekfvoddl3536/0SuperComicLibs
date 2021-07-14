using System;
using System.Collections;

namespace SuperComicWorld
{
    public sealed class ValueContractAttribute : FieldBaseAttribute
    {
        public string Message;
        public bool ThrowException;
        internal readonly object otherValue;
        internal readonly CompareMode mode;

        /// <summary>
        /// 필드가 좌측 피연산자가 됩니다.
        /// <para/>
        /// 가령 '<see cref="CompareMode.Lesser"/>'의 경우
        /// '필드 값이 <paramref name="otherValue"/> 보다 작다'라는 뜻이 됩니다.
        /// </summary>
        public ValueContractAttribute(object otherValue, CompareMode mode)
        {
            this.otherValue = otherValue;

            if (mode < CompareMode.Equals || mode > CompareMode.GreatOrEqual)
                // 범위 초과
                this.mode = CompareMode.Equals;
            else if (otherValue.GetType().IsValueType == false && mode > CompareMode.Inequals)
                // 부적절한 비교연산자
                this.mode =
                    mode >= CompareMode.Greater
                    ? CompareMode.Equals
                    : CompareMode.Inequals;
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

            CompareMode mode = this.mode;

            if (mode == CompareMode.Equals) return compResult == 0;
            else if (mode == CompareMode.Inequals) return compResult != 0;
            else if (mode == CompareMode.Lesser) return compResult < 0;
            else if (mode == CompareMode.LessOrEqual) return compResult <= 0;
            else if (mode == CompareMode.Greater) return compResult > 0;
            else
                return compResult >= 0;
        }

        public enum CompareMode
        {
            Equals,
            Inequals,

            Lesser,
            LessOrEqual,

            Greater,
            GreatOrEqual,
        }
    }
}