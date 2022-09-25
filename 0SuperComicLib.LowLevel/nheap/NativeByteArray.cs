using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.LowLevel
{
    [StructLayout(LayoutKind.Sequential, Pack = sizeof(int))]
    public unsafe struct NativeByteArray : IDisposable, IEquatable<NativeByteArray>, IComparable<NativeByteArray>
    {
        private byte* m_ptr;
        private int m_size;

        public NativeByteArray(int init_size, bool initDefaultValue = false)
        {
            m_ptr = NativeClass.Internal_Alloc(init_size, initDefaultValue);
            m_size = init_size;
        }

        public NativeByteArray(IntPtr HGlobalPTR, int size)
        {
            m_ptr = (byte*)HGlobalPTR;
            m_size = size;
        }

        public NativeByteArray(IntPtr HGlobalPTR) : this(HGlobalPTR, -1) { }

        public bool IsInvalid => m_ptr == null && m_size == 0;
        
        public int HeapSize => m_size;

        public ref byte this[int offset] => ref m_ptr[offset];

        public ref T Value<T>(int offset) where T : unmanaged => ref *(T*)(m_ptr + offset);

        public int CompareTo(NativeByteArray other)
        {
            if (IsInvalid || other.IsInvalid)
                throw new InvalidOperationException();

            int sz = m_size;
            if (sz != -1)
            {
                int sz2 = other.m_size;
                if (sz2 != -1 && sz2 == sz)
                    return NativeClass.Internal_CompareTo_Un_S(m_ptr, other.m_ptr, (uint)sz);
            }
            throw new IndexOutOfRangeException();
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)m_ptr);
            m_ptr = null;
            m_size = 0;
        }

        public bool Equals(NativeByteArray right) =>
            m_ptr == right.m_ptr &&
            m_size == right.m_size;

        public override bool Equals(object obj) =>
            obj is NativeByteArray right
            ? Equals(right)
            : obj is IEquatable<NativeByteArray> eq && eq.Equals(this);

        public override int GetHashCode() =>
            m_ptr != null
            ? ((IntPtr)m_ptr).GetHashCode()
            : -1;

        public static bool operator ==(NativeByteArray left, NativeByteArray right) => left.m_ptr == right.m_ptr;
        public static bool operator !=(NativeByteArray left, NativeByteArray right) => left.m_ptr != right.m_ptr;

        public static bool operator <(NativeByteArray left, NativeByteArray right) => left.CompareTo(right) < 0;
        public static bool operator <=(NativeByteArray left, NativeByteArray right) => left.CompareTo(right) <= 0;
        public static bool operator >(NativeByteArray left, NativeByteArray right) => left.CompareTo(right) > 0;
        public static bool operator >=(NativeByteArray left, NativeByteArray right) => left.CompareTo(right) >= 0;
    }
}
