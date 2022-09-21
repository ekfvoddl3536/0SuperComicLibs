using System;

namespace SuperComicLib.CodeContracts
{
    /// <summary>
    /// 이 특성으로 표시된 메소드나 속성은 예외가 발생할 가능성이 극히 낮은 안전한 작업만을 수행합니다 (예: 변수에 대한 상수 시프트 연산, 검사된 범위 내에서 정수/부동소수 사칙연산)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class NoExceptAttribute : Attribute
    {
        /// <summary>
        /// 억제된 예외 타입 목록입니다.<para/>
        /// 빈 배열인 경우, 모든 예외가 억제됨을 의미합니다.
        /// </summary>
        public readonly Type[] Exceptions;

        public NoExceptAttribute() => Exceptions = Array.Empty<Type>();

        /// <param name="exceptions">억제된 예외 타입 목록입니다. 빈 배열또는 null을 지정하는 경우, 모든 예외를 억제함을 의미합니다.</param>
        public NoExceptAttribute(params Type[] exceptions) => Exceptions = exceptions ?? Array.Empty<Type>();
    }
}
