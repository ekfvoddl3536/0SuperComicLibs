using System;

namespace SuperComicLib.Runtime
{
    public abstract class FastComparer<T> : IFastComparerEx<T>
    {
        private static readonly FastComparer<T> default_comparer = Create();

        public static IFastComparerEx<T> Default => default_comparer;

        private static FastComparer<T> Create()
        {
            Type t = typeof(T);
            if (t.IsPrimitive)
                switch (Type.GetTypeCode(t))
                {
                    case TypeCode.SByte:
                        return (FastComparer<T>)typeof(SByteComparer).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.Byte:
                        return (FastComparer<T>)typeof(ByteComparer).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.Int16:
                        return (FastComparer<T>)typeof(Int16Comparer).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.UInt16:
                        return (FastComparer<T>)typeof(UInt16Comparer).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.Int32:
                        return (FastComparer<T>)typeof(Int32Comparer).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.UInt32:
                        return (FastComparer<T>)typeof(UInt32Comparer).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.Int64:
                        return (FastComparer<T>)typeof(Int64Comparer).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.UInt64:
                        return (FastComparer<T>)typeof(UInt64Comparer).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.Single:
                        return (FastComparer<T>)typeof(SingleComparer).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.Double:
                        return (FastComparer<T>)typeof(DoubleComparer).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                        return (FastComparer<T>)typeof(GenericUniversalComparer<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null);
                }
            else if (typeof(IComparable<T>).IsAssignableFrom(t))
                return (FastComparer<T>)typeof(GenericUniversalComparer<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null);
            else if (typeof(IEquatable<T>).IsAssignableFrom(t))
                return (FastComparer<T>)typeof(GenericEqualsOnlyComparer<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null);
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type u = t.GetGenericArguments()[0];
                if (typeof(IComparable<>).MakeGenericType(u).IsAssignableFrom(u))
                    return (FastComparer<T>)typeof(GenericUniversalComparer<>).MakeGenericType(u).GetConstructor(Type.EmptyTypes).Invoke(null);
                else if (typeof(IEquatable<T>).IsAssignableFrom(t))
                    return (FastComparer<T>)typeof(GenericEqualsOnlyComparer<>).MakeGenericType(u).GetConstructor(Type.EmptyTypes).Invoke(null);
            }
            else if (t.IsEnum)
            {
                TypeCode ut = Type.GetTypeCode(Enum.GetUnderlyingType(t));

                switch (ut)
                {
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                        return (FastComparer<T>)typeof(EnumComparer<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.Byte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                        return (FastComparer<T>)typeof(UnsignedEnumComparer<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.Int64:
                        return (FastComparer<T>)typeof(LongEnumComparer<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null);
                    case TypeCode.UInt64:
                        return (FastComparer<T>)typeof(UnsignedLongEnumComparer<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null);
                }
            }
            else if (t.IsValueType)
                return (FastComparer<T>)typeof(NativeComp<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null);

            return (FastComparer<T>)typeof(ObjectUniversalComparer<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes).Invoke(null);
        }

        public abstract bool Greater(T left, T right);

        public abstract bool GreatOrEquals(T left, T right);

        public abstract bool Lesser(T left, T right);

        public abstract bool LessOrEquals(T left, T right);

        public abstract bool EqualsAB(T left, T right);

        public virtual bool NotEqualsAB(T left, T right) => !EqualsAB(left, right);
    }
}
