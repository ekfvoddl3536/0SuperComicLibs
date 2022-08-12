using System;
using System.Collections.Generic;
using System.Reflection;
using SuperComicLib.Collections;

namespace SuperComicLib.Runtime
{
    public static class UnmanagedTypeExtension
    {
        private const BindingFlags flg = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private static readonly HashSet<Type> bag = new HashSet<Type>();

        public static bool IsUnmanaged(this Type type)
        {
            while (type.IsPointer || type.IsArray)
                type = type.GetElementType();

            if (type.IsPrimitive || type.IsEnum || bag.Contains(type))
                return true;

            if (type.IsValueType)
            {
                HashSet<Type> done = new HashSet<Type>();
                Stack<IValueIterator<FieldInfo>> stack = new Stack<IValueIterator<FieldInfo>>();

                IValueIterator<FieldInfo> e1 = type.GetFields(flg).Begin();
            loop:
                for (; e1.IsAlive; e1.Add())
                {
                    Type tmp = e1.Value.FieldType;
                    if (tmp.IsPointer)
                        continue;
                    else if (!tmp.IsValueType)
                        return false;
                    else if (!tmp.IsPrimitive && done.Add(tmp))
                    {
                        stack.Push(e1);
                        e1.Add();

                        e1 = tmp.GetFields(flg).Begin();
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
