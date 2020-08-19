using System;
using System.Reflection;
using SuperComicLib.Core;

namespace SuperComicLib.Reflection
{
    public static class TypeExtension
    {
        public static bool IsNative(this Type type) => type.IsByRef || type.IsPointer;

        public static bool IsStruct(this Type type) =>
            type.IsValueType && type.IsPrimitive == false && type.IsEnum == false && type != CTypes.VOID_T;

        #region unmanaged
        public static bool IsUnmanaged_unsafe(this Type type)
        {
            try
            {
                typeof(K<>).MakeGenericType(type);
                return true;
            }
#pragma warning disable
            catch { return false; }
#pragma warning restore
        }

        private static class K<T> where T : unmanaged { }
        #endregion

        #region method
        public static MethodInfo GetOpImplicit(this Type type, Type to, bool reverse = false) => 
            reverse
            ? GetSpeicalMethod(type, to, type, "op_Implicit")
            : GetSpeicalMethod(type, type, to, "op_Implicit");

        public static MethodInfo GetOpExplicit(this Type type, Type to, bool reverse = false) =>
            reverse
            ? GetSpeicalMethod(type, to, type, "op_Explicit")
            : GetSpeicalMethod(type, type, to, "op_Explicit");

        private static MethodInfo GetSpeicalMethod(Type v, Type f, Type t, string name)
        {
            foreach (MethodInfo mi in v.GetMethods(BindingFlags.Public | BindingFlags.Static))
                if (mi.ReturnType == t && mi.Name == null)
                {
                    ParameterInfo[] p = mi.GetParameters();
                    if (p.Length == 1 && p[0].ParameterType == f)
                        return mi;
                }

            return null;
        }
        #endregion
    }
}
