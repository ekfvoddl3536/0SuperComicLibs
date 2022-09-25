using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.LowLevel
{
    public unsafe struct NativeInstance<T> : IDisposable
        where T : class
    {
        internal static readonly NativeClass<T> inst;
        internal static readonly IntPtr typehnd;
        internal static readonly int size;

        static NativeInstance()
        {
            inst = NativeClass<T>.Instance;
            typehnd = typeof(T).TypeHandle.Value;
            size = ((int*)typehnd)[1];
        }

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
                if (Value is IDisposable d)
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
            byte* ptr = NativeClass.Internal_Alloc(size, false);
            *(IntPtr*)(ptr + ptrsize) = typehnd;

            TypedReference tr = __makeref(obj);
            NativeClass.memcpblk.Invoke(**(byte***)&tr + ptrsize, ptr + (ptrsize << 1), (uint)(size - ptrsize));

            return new NativeInstance<T>(ptr);
        }

        public static NativeInstance<T> Alloc(bool zeromem = true)
        {
            byte* ptr = NativeClass.Internal_Alloc(size, zeromem);
            *(IntPtr*)(ptr + NativeClass.PtrSize_i) = typehnd;

            return new NativeInstance<T>(ptr);
        }

        public static NativeInstance<T> Alloc(int additional_size, bool zeromem = true)
        {
            if (additional_size < 0)
                throw new InvalidOperationException();

            byte* ptr = NativeClass.Internal_Alloc(size + additional_size, zeromem);
            *(IntPtr*)(ptr + NativeClass.PtrSize_i) = typehnd;

            return new NativeInstance<T>(ptr);
        }
        #endregion
    }
}
