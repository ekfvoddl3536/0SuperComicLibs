using System;
using System.Collections.Generic;
using System.Reflection;

namespace SuperComicLib.Runtime
{
    public static class UnmanagedTypeExtension
    {
        public static bool IsUnmanaged(this Type type)
        {
            const BindingFlags FLAG = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            if (type.IsPrimitive || type.IsPointer || type.IsEnum)
                return true;

            if (!type.IsValueType)
                return false;

            var stack = new Stack<Type>(16);

            for (; ; )
            {
                var flds = type.GetFields(FLAG);
                for (int i = 0; i < flds.Length; ++i)
                {
                    type = flds[i].FieldType;
                    // find class type
                    if (!type.IsValueType)
                        return false;
                    // no reference type, but, is struct
                    else if (!type.IsPrimitive && !type.IsPointer && !type.IsEnum)
                        stack.Push(type);
                }

                if (stack.Count == 0)
                    break;

                type = stack.Pop();
            }

            return true;
        }
    }
}
