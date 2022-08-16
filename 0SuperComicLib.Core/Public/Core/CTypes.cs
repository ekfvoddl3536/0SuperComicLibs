using System;

namespace SuperComicLib.Core
{
    public static class CTypes
    {
        public static readonly Type VOID_T = typeof(void);
        public static readonly Type BOOL_T = typeof(bool);
        public static readonly Type SBYTE_T = typeof(sbyte);
        public static readonly Type BYTE_T = typeof(byte);
        public static readonly Type SHORT_T = typeof(short);
        public static readonly Type INT_T = typeof(int);
        public static readonly Type LONG_T = typeof(long);
        public static readonly Type FLOAT_T = typeof(float);
        public static readonly Type DOUBLE_T = typeof(double);
        public static readonly Type STR_T = typeof(string);

        public static readonly Type INTPTR_T = typeof(IntPtr);

        public static readonly Type VALUETYPE_T = typeof(ValueType);
        public static readonly Type OBJECT_T = typeof(object);
    }

    public static class UCTypes
    {
        public static readonly Type SHORT_T = typeof(ushort);
        public static readonly Type INT_T = typeof(uint);
        public static readonly Type LONG_T = typeof(ulong);
    }
}
