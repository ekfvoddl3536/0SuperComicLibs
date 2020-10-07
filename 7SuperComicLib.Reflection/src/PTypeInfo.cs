//using System;
//using System.Collections.Generic;
//using System.Reflection;

//namespace SuperComicLib.Reflection
//{
//    public sealed unsafe class PTypeInfo : IEquatable<PTypeInfo>
//    {
//        private const int
//            UF_NORMAL = 0x1,
//            UF_ARRAY = 0x100,
//            UF_POINTER = 0x200,
//            UF_STRUCT = 0x1000,
//            UF_UNKNOW = int.MinValue;

//        private static readonly Dictionary<HashedString, IntPtr> map = new Dictionary<HashedString, IntPtr>(128);

//        private readonly RuntimeTypeInfo value;

//        #region constructor
//        public PTypeInfo(Type value)
//        {
//#if DEBUG
//            if (value == null)
//                throw new ArgumentNullException(nameof(value));
//#endif

//            while (value.IsPointer || value.IsArray)
//                value = value.GetElementType();

//            this.value = *Create(value);
//        }

//        public PTypeInfo(HashedString fullName, TypeAttributes attribute, PTypeInfo parent)
//        {
//            RuntimeTypeInfo* pparent =
//                parent == null
//                ? null
//                : map.TryGetValue(parent.value.name, out IntPtr result)
//                ? (RuntimeTypeInfo*)result
//                : throw new InvalidOperationException();

//            if (map.TryGetValue(fullName, out result))
//                value = *(RuntimeTypeInfo*)result;
//            else
//            {
//                RuntimeTypeInfo* me = RuntimeTypeInfo.Init(fullName, UF_NORMAL | UF_ARRAY | UF_UNKNOW,  (int)attribute, pparent);
//                map.Add(fullName, (IntPtr)me);

//                value = *me;
//            }
//        }

//        internal PTypeInfo(RuntimeTypeInfo* info) => value = *info;
//        #endregion

//        #region property
//        public HashedString Name => value.name;

//        public TypeAttributes Attributes => (TypeAttributes)value.type_flags;

//        public bool IsAllowMakePointer => (value.user_flags & UF_POINTER) != 0;

//        public bool IsAllowMakeArray => (value.user_flags & UF_ARRAY) != 0;

//        public bool IsValueType => (value.user_flags & UF_STRUCT) != 0;

//        public bool IsInvalid => value.user_flags == 0;

//        public PTypeInfo Parent =>
//            value.parent == null
//            ? null
//            : new PTypeInfo(value.parent);
//        #endregion

//        #region method
//        public bool Equals(PTypeInfo other) => value.Equals(other.value);

//        public override int GetHashCode() => value.GetHashCode();
//        #endregion

//        #region static
//        private static RuntimeTypeInfo* Create(Type value)
//        {
//            Stack<Type> stack = new Stack<Type>();
//            while (value.BaseType != null)
//            {
//                stack.Push(value);
//                value = value.BaseType;
//            }

//            RuntimeTypeInfo* me;
//            RuntimeTypeInfo* parent = null;

//        loop:
//            int attrb = (int)value.Attributes;
//            HashedString temp = new HashedString(value.FullName);

//            if (!map.TryGetValue(temp, out IntPtr result))
//            {
//                me = RuntimeTypeInfo.Init(temp, GetFlag(value), attrb, parent);
//                map.Add(temp, (IntPtr)me);
//            }
//            else
//                me = (RuntimeTypeInfo*)result;

//            if (stack.Count > 0)
//            {
//                parent = me;
//                value = stack.Pop();
//                goto loop;
//            }

//            return me;
//        }

//        private static int GetFlag(Type v)
//        {
//            int result = UF_NORMAL | UF_ARRAY;
//            if (v.IsUnmanaged_unsafe())
//                result |= UF_POINTER;
//            if (v.IsValueType)
//                result |= UF_STRUCT;

//            return result;
//        }

//        public static bool IsCached(HashedString fullName) => map.ContainsKey(fullName);

//        public static PTypeInfo TryGet(HashedString fullName) =>
//            map.TryGetValue(fullName, out IntPtr ptr)
//            ? new PTypeInfo((RuntimeTypeInfo*)ptr)
//            : null;
//        #endregion
//    }
//}
