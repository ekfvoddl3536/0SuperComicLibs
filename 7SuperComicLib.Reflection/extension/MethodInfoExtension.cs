using System;
using System.Reflection;

namespace SuperComicLib.Reflection
{
    public static class MethodInfoExtension
    {
        private const MethodAttributes attrb = MethodAttributes.Virtual | MethodAttributes.Final;
        public static bool IsStructVirtual(this MethodInfo method, Type type) =>
            type.IsValueType && (method.Attributes & attrb) == MethodAttributes.Virtual;
    }
}
