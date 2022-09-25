using System;
using System.Collections.Generic;
using System.Reflection;

namespace SuperComicLib.Runtime
{
    public static class UnmanagedTypeExtension
    {
        private static readonly HashSet<Type> bag = new HashSet<Type>();

        public static bool IsUnmanaged(this Type type)
        {
            const BindingFlags FLAG = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            if (type.IsPointer || type.IsPrimitive || type.IsEnum || bag.Contains(type))
                return true;

            if (type.IsValueType)
            {
                HashSet<Type> done = new HashSet<Type>();
                Stack<Memory<FieldInfo>> stack = new Stack<Memory<FieldInfo>>();

                Memory<FieldInfo> e1 = type.GetFields(FLAG);

            loop:
                var arr = e1._source;
                for (int i = e1._start; (uint)i < (uint)arr.Length; i++)
                {
                    Type tmp = arr[i].FieldType;
                    if (tmp.IsPointer)
                        continue;
                    else if (!tmp.IsValueType)
                        return false;
                    else if (!tmp.IsPrimitive && done.Add(tmp))
                    {
                        stack.Push(new Memory<FieldInfo>(arr, i + 1));

                        e1 = tmp.GetFields(FLAG);
                        goto loop;
                    }
                }

                if (stack.Count > 0)
                {
                    e1 = stack.Pop();
                    goto loop;
                }

                bag.Add(type);
                return true;
            }

            return false;
        }

    }
}
