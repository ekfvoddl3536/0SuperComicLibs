using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.LowLevel
{
    public unsafe struct NativeInstance<T> : IDisposable
        where T : class
    {
        private static readonly NativeClass<T> inst = NativeClass<T>.Instance;
        private byte* m_ptr;

        internal NativeInstance(byte* ptr) => m_ptr = ptr;

        public bool IsInvalid => m_ptr == null;

        public T Value => inst.Cast(m_ptr + NativeClass.PtrSize_i);

        public byte* Pointer => m_ptr;

        public void Dispose()
        {
            T value = Value;
            if (value is IDisposable d)
                d.Dispose();

            Marshal.FreeHGlobal((IntPtr)m_ptr);
            m_ptr = null;
        }

        public override bool Equals(object obj) => false;

        public override int GetHashCode() => 
            m_ptr == null 
            ? -1
            : Value.GetHashCode();

        public static bool operator ==(NativeInstance<T> left, NativeInstance<T> right) => left.m_ptr == right.m_ptr;
        public static bool operator !=(NativeInstance<T> left, NativeInstance<T> right) => left.m_ptr != right.m_ptr;
    }
}
