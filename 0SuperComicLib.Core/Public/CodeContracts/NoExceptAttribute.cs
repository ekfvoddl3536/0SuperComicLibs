// MIT License
//
// Copyright (c) 2019-2023. SuperComic (ekfvoddl3535@naver.com)
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

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public NoExceptAttribute() => Exceptions = Array.Empty<Type>();

        /// <param name="exceptions">억제된 예외 타입 목록입니다. 빈 배열또는 null을 지정하는 경우, 모든 예외를 억제함을 의미합니다.</param>
        public NoExceptAttribute(params Type[] exceptions) => Exceptions = exceptions ?? Array.Empty<Type>();
    }
}
