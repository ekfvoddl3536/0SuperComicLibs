// MIT License
//
// Copyright (c) 2019-2024. SuperComic (ekfvoddl3535@naver.com)
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
    /// <see cref="FastContract"/>가 실패함
    /// </summary>
    public sealed class FastContractFailException : Exception
    {
        /// <summary>
        /// 기본 메시지로 오류 생성
        /// </summary>
        public FastContractFailException() : base("[SuperComicLib::CodeContracts] Contract Fail") { }

        /// <summary>
        /// 지정된 메시지로 오류 생성
        /// </summary>
        public FastContractFailException(string message) : base(message) { }

        /// <summary>
        /// 지정된 메시지와 내부 오류 정보를 사용해 오류 생성
        /// </summary>
        public FastContractFailException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}