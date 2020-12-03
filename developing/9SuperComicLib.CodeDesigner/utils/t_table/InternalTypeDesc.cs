using System;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    internal sealed class InternalTypeDesc : ITypeDesc
    {
        internal Type normalType;
        internal List<Type> generics = new List<Type>();

        internal bool TryAdd(Type type)
        {
            if (type.IsGenericType)
                return TryAddGeneric(type);
            else if (normalType != null)
            {
                normalType = type;
                return true;
            }
            else
                return false;
        }

        private bool TryAddGeneric(Type type)
        {
            var list = generics;
            if (list.Contains(type))
                return false;

            list.Add(type);
            return true;
        }

        public Type NormalType => normalType;

        public Type GenericType(params Type[] types)
        {
#if DEBUG
            if (types == null || types.Length == 0)
                throw new ArgumentException();
#endif

            var iter = generics.GetEnumerator();
            int len = types.Length;

            while (iter.MoveNext())
                if (iter.Current.GetGenericArguments().Length == len)
                {
                    Type select = iter.Current;
                    iter.Dispose();

                    return select;
                }

            iter.Dispose();
            return null;
        }
    }
}
