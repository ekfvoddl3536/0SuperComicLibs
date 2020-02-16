using System;

namespace SuperComicLib.Reflection
{
    internal static class TypeExtension
    {
        internal static bool IsStruct(this Type type) =>
            type.IsValueType && type.IsPrimitive == false && type.IsEnum == false && type != Helper.VOID_T;

        internal static bool IsNative(this Type type) => type.IsByRef || type.IsPointer;
    }
}
