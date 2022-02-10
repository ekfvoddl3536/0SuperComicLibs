using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.LowLevel
{
    public unsafe struct NativeInstance<T> : IDisposable
        where T : class
    {
        internal static readonly NativeClass<T> inst = NativeClass<T>.Instance;
        internal static readonly IntPtr typehnd = typeof(T).TypeHandle.Value;
        internal static readonly int size = *((int*)typehnd + (sizeof(int) + NativeClass.PtrSize_i));

        private byte* m_ptr;

        private NativeInstance(byte* ptr) => m_ptr = ptr;

        #region methods & properties
        public bool IsInvalid => m_ptr == null;

        public T Value => inst.Cast(m_ptr + NativeClass.PtrSize_i);

        public byte* Raw => m_ptr;

        public byte* AsPointer() => m_ptr + (NativeClass.PtrSize_i << 1);

        public byte* AsPointer(int field_offset) => AsPointer() + field_offset;
        #endregion

        #region override
        public void Dispose()
        {
            if (m_ptr != null)
            {
                T value = Value;
                if (value is IDisposable d)
                    d.Dispose();

                Marshal.FreeHGlobal((IntPtr)m_ptr);
                m_ptr = null;
            }
        }

        public override bool Equals(object obj) => false;

        public override int GetHashCode() => 
            m_ptr == null 
            ? -1
            : Value.GetHashCode();
        #endregion

        #region static methods
        public static bool operator ==(NativeInstance<T> left, NativeInstance<T> right) => left.m_ptr == right.m_ptr;
        public static bool operator !=(NativeInstance<T> left, NativeInstance<T> right) => left.m_ptr != right.m_ptr;

        public static NativeInstance<T> Dup(T obj)
        {
            if (obj == null)
                return default;

            int ptrsize = NativeClass.PtrSize_i;
            byte* ptr = NativeClass.Internal_Alloc(size, true);
            *(IntPtr*)(ptr + ptrsize) = typehnd;

            TypedReference tr = __makeref(obj);
            NativeClass.memcpblk.Invoke(**(byte***)&tr + ptrsize, ptr + (ptrsize << 1), (uint)(size - ptrsize));

            return new NativeInstance<T>(ptr);
        }

        public static NativeInstance<T> Alloc()
        {
            byte* ptr = NativeClass.Internal_Alloc(size, true);
            *(IntPtr*)(ptr + NativeClass.PtrSize_i) = typehnd;

            return new NativeInstance<T>(ptr);
        }

        public static NativeInstance<T> Alloc(int additional_size)
        {
            if (additional_size < 0)
                throw new InvalidOperationException();

            byte* ptr = NativeClass.Internal_Alloc(size + additional_size, true);
            *(IntPtr*)(ptr + NativeClass.PtrSize_i) = typehnd;

            return new NativeInstance<T>(ptr);
        }
        #endregion
    }
}
