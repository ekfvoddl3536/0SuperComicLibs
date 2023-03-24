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
    /// 이 특성으로 지정된 값은 유효한 범위 내에 있어야 합니다
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ValidRangeAttribute : Attribute
    {
        /// <summary>
        /// 유효 범위 검사에 대한 종류
        /// </summary>
        public enum RangeType
        {
            /// <summary>
            /// 기본 값
            /// </summary>
            None = 0,

            /// <summary>
            /// 최소 값부터 최대 값
            /// </summary>
            Min_to_Max,

            /// <summary>
            /// 최소 값부터 Length
            /// </summary>
            Min_to_Length,

            /// <summary>
            /// 최대 값을 포함
            /// </summary>
            ModeFlag_IncludeMax = 4,
            
            /// <summary>
            /// 부호 없는 정수
            /// </summary>
            ModeFlag_Unsigned = 8
        }

        /// <summary>
        /// 최소 값
        /// </summary>
        public readonly UIntPtr Include_Minimum;

        private readonly UIntPtr _maximum;
        private readonly string _desc;
        private readonly RangeType _type;

        /// <summary>
        /// 설명 가져오기
        /// </summary>
        [return: NotNull]
        public string GetDescription()
        {
            if (_desc != null)
                return _desc;

            string
                min_str,
                max_str;

            var tv = _type;
            if ((tv & RangeType.ModeFlag_Unsigned) != 0)
            {
                min_str = ((ulong)Include_Minimum).ToString();
                max_str = ((ulong)_maximum).ToString();
            }
            else
            {
                min_str = ((long)Include_Minimum).ToString();
                max_str = ((long)_maximum).ToString();
            }

            if ((tv & RangeType.Min_to_Max) != 0)
            {
                string tmp = ((tv & RangeType.ModeFlag_IncludeMax) != 0) ? "Include" : "Exclude";
                return $"'{min_str}' (Include) to '{max_str}' ({tmp})";
            }
            else
                return $"'{min_str}' (Include) to 'Length' (Exclude)";
        }

        private ValidRangeAttribute(UIntPtr minimum, bool isUnsigned)
        {
            Include_Minimum = minimum;
            _type = RangeType.Min_to_Length | (isUnsigned ? RangeType.ModeFlag_Unsigned : 0);
        }

        private ValidRangeAttribute(UIntPtr include_minimum, UIntPtr maximum, bool include_maximum_value, bool isUnsigned)
        {
            Include_Minimum = include_minimum;
            _maximum = maximum;
            _type =
                (include_maximum_value ? RangeType.ModeFlag_IncludeMax : RangeType.Min_to_Max) |
                (isUnsigned ? RangeType.ModeFlag_Unsigned : 0);
        }

        /// <summary>
        /// 최소 값을 지정
        /// </summary>
        public ValidRangeAttribute(int minToLength_include_minimum) : this((UIntPtr)minToLength_include_minimum, false) { }

        /// <summary>
        /// 최소 값을 지정
        /// </summary>
        public ValidRangeAttribute(uint minToLength_include_minimum) : this((UIntPtr)minToLength_include_minimum, true) { }

        /// <summary>
        /// 최소 값을 지정
        /// </summary>
        public ValidRangeAttribute(long minToLength_include_minimum) : this((UIntPtr)minToLength_include_minimum, false) { }

        /// <summary>
        /// 최소 값을 지정
        /// </summary>
        public ValidRangeAttribute(ulong minToLength_include_minimum) : this((UIntPtr)minToLength_include_minimum, true) { }

        /// <summary>
        /// 최소 값, 최대 값, 최대 값 포함 여부를 지정
        /// </summary>
        public ValidRangeAttribute(int include_minimum, int maximum, bool include_maximum_value = false) : this((UIntPtr)include_minimum, (UIntPtr)maximum, include_maximum_value, false) { }

        /// <summary>
        /// 최소 값, 최대 값, 최대 값 포함 여부를 지정
        /// </summary>
        public ValidRangeAttribute(uint include_minimum, uint maximum, bool include_maximum_value = false) : this((UIntPtr)include_minimum, (UIntPtr)maximum, include_maximum_value, true) { }

        /// <summary>
        /// 최소 값, 최대 값, 최대 값 포함 여부를 지정
        /// </summary>
        public ValidRangeAttribute(long include_minimum, long maximum, bool include_maximum_value = false) : this((UIntPtr)include_minimum, (UIntPtr)maximum, include_maximum_value, false) { }

        /// <summary>
        /// 최소 값, 최대 값, 최대 값 포함 여부를 지정
        /// </summary>
        public ValidRangeAttribute(ulong include_minimum, ulong maximum, bool include_maximum_value = false) : this((UIntPtr)include_minimum, (UIntPtr)maximum, include_maximum_value, true) { }

        /// <summary>
        /// 복잡한 조건에 대한 사용자 설명을 지정
        /// </summary>
        public ValidRangeAttribute(string custom_range_description) => _desc = custom_range_description;

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public ValidRangeAttribute() : this(UIntPtr.Zero, false) { }
    }
}
