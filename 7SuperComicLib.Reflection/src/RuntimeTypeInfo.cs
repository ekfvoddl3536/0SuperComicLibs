//using System;
//using System.Runtime.InteropServices;
//using SuperComicLib.Core;

//namespace SuperComicLib.Reflection
//{
//    [StructLayout(LayoutKind.Sequential, Pack = sizeof(int))]
//    internal readonly unsafe struct RuntimeTypeInfo : IEquatable<RuntimeTypeInfo>
//    {
//        private static readonly IntPtr size = new IntPtr(sizeof(RuntimeTypeInfo));

//        internal readonly int user_flags;
//        internal readonly int type_flags;
//        internal readonly HashedString name;
//        internal readonly RuntimeTypeInfo* parent;

//        public bool Equals(RuntimeTypeInfo other) =>
//            user_flags == other.user_flags &&
//            type_flags == other.type_flags &&
//            name == other.name &&
//            parent == other.parent;

//        public override int GetHashCode()
//        {
//            int result = 7;
//            IntHash.Combine(ref result, user_flags);
//            IntHash.Combine(ref result, type_flags);
//            IntHash.Combine(ref result, name.GetHashCode());

//            return result;
//        }

//        #region static
//        public static RuntimeTypeInfo* Init(HashedString name, int user_flags, int type_flags, RuntimeTypeInfo* parent)
//        {
//            Alloc* ptr = (Alloc*)Marshal.AllocHGlobal(size);
            
//            ref Alloc value = ref *ptr;
//            value.user_flags = user_flags;
//            value.type_flags = type_flags;
//            value.name = name;
//            value.parent = parent;

//            return (RuntimeTypeInfo*)ptr;
//        }

//        [StructLayout(LayoutKind.Sequential, Pack = sizeof(int))]
//        private unsafe struct Alloc
//        {
//            public int user_flags;
//            public int type_flags;
//            public HashedString name;
//            public RuntimeTypeInfo* parent;
//        }
//        #endregion
//    }
//}
