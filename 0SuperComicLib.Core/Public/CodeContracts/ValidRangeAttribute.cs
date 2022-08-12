using System;

namespace SuperComicLib.CodeContracts
{
    /// <summary>
    /// 이 특성으로 지정된 값은 유효한 범위 내에 있어야 합니다
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ValidRangeAttribute : Attribute
    {
        public enum RangeType
        {
            None = 0,

            Min_to_Max,
            Min_to_Length,

            ModeFlag_IncludeMax = 4,
            ModeFlag_Unsigned = 8
        }

        public readonly UIntPtr Include_Minimum;
        private readonly UIntPtr _maximum;
        private readonly string _desc;
        private readonly RangeType _type;

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

        public ValidRangeAttribute(int minToLength_include_minimum) : this((UIntPtr)minToLength_include_minimum, false) { }

        public ValidRangeAttribute(uint minToLength_include_minimum) : this((UIntPtr)minToLength_include_minimum, true) { }

        public ValidRangeAttribute(long minToLength_include_minimum) : this((UIntPtr)minToLength_include_minimum, false) { }

        public ValidRangeAttribute(ulong minToLength_include_minimum) : this((UIntPtr)minToLength_include_minimum, true) { }

        public ValidRangeAttribute(int include_minimum, int maximum, bool include_maximum_value = false) : this((UIntPtr)include_minimum, (UIntPtr)maximum, include_maximum_value, false) { }

        public ValidRangeAttribute(uint include_minimum, uint maximum, bool include_maximum_value = false) : this((UIntPtr)include_minimum, (UIntPtr)maximum, include_maximum_value, true) { }

        public ValidRangeAttribute(long include_minimum, long maximum, bool include_maximum_value = false) : this((UIntPtr)include_minimum, (UIntPtr)maximum, include_maximum_value, false) { }

        public ValidRangeAttribute(ulong include_minimum, ulong maximum, bool include_maximum_value = false) : this((UIntPtr)include_minimum, (UIntPtr)maximum, include_maximum_value, true) { }
        
        public ValidRangeAttribute(string custom_range_description) => _desc = custom_range_description;

        public ValidRangeAttribute() : this(UIntPtr.Zero, false) { }
    }
}
