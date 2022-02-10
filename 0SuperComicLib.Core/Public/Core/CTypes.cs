using System;

namespace SuperComicLib.Core
{
    public static class CTypes
    {
        public static readonly Type VOID_T = typeof(void);
        public static readonly Type BOOL_T = CachedType<bool>.Value;
        public static readonly Type SBYTE_T = CachedType<sbyte>.Value;
        public static readonly Type BYTE_T = CachedType<byte>.Value;
        public static readonly Type SHORT_T = CachedType<short>.Value;
        public static readonly Type INT_T = CachedType<int>.Value;
        public static readonly Type LONG_T = CachedType<long>.Value;
        public static readonly Type FLOAT_T = CachedType<float>.Value;
        public static readonly Type DOUBLE_T = CachedType<double>.Value;
        public static readonly Type STR_T = CachedType<string>.Value;

        public static readonly Type INTPTR_T = CachedType<IntPtr>.Value;

        public static readonly Type VALUETYPE_T = CachedType<ValueType>.Value;
        public static readonly Type OBJECT_T = CachedType<object>.Value;
    }

    public static class UCTypes
    {
        public static readonly Type SHORT_T = CachedType<ushort>.Value;
        public static readonly Type INT_T = CachedType<uint>.Value;
        public static readonly Type LONG_T = CachedType<ulong>.Value;
    }
}
