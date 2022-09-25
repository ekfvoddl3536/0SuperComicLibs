using System;
using System.Runtime.InteropServices;

namespace SuperComicLib.LowLevel
{
    public readonly unsafe struct NativeString : IDisposable
    {
        private readonly char* m_ptr;

        public NativeString(string value)
        {
            if (value == null)
                m_ptr = null;
            else
            {
                int size = value.Length;
                byte* ptr = NativeClass.Internal_Alloc(sizeof(char) * (size + 1), false);

                fixed (char* src = value)
                    NativeClass.Internal_memcpyff((byte*)src, 0, ptr, 0, (uint)(sizeof(char) * size));

                char* buf = (char*)ptr;
                buf[size] = char.MinValue;
                m_ptr = buf;
            }
        }

        public int Length()
        {
            char* ptr = m_ptr;
            if (ptr == null)
                throw new ObjectDisposedException(nameof(NativeString));

            for (int i = 0; ; i++)
                if (ptr[i] == char.MinValue)
                    return i;
        }

        public ref char this[int idx] => ref m_ptr[idx];

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => ((IntPtr)m_ptr).GetHashCode();
        public override string ToString() => new string(m_ptr);

        public void Dispose()
        {
            if (m_ptr == null)
                throw new ObjectDisposedException(nameof(NativeString));

            Marshal.FreeHGlobal((IntPtr)m_ptr);
        }

        public static implicit operator NativeString(string v) => new NativeString(v);

        #region equals
        public static bool operator ==(NativeString left, NativeString right)
        {
            char* pl = left.m_ptr, pr = right.m_ptr;
            if (pl == null || pr == null)
                return pl == null && pr == null;

            for (int i = 0; ; i++)
                if (pl[i] != pr[i])
                    return false;
                else if (pl[i] == char.MinValue)
                    return true;
        }
        public static bool operator !=(NativeString left, NativeString right)
        {
            char* pl = left.m_ptr, pr = right.m_ptr;
            if (pl == null || pr == null)
                return !(pl == null && pr == null);

            for (int i = 0; ; i++)
                if (pl[i] != pr[i])
                    return true;
                else if (pl[i] == char.MinValue)
                    return false;
        }

        public static bool operator ==(NativeString left, string right)
        {
            fixed (char* pr = right)
            {
                char* pl = left.m_ptr;
                if (pl == null || pr == null)
                    return pl == null && pr == null;

                int length = right.Length;
                for (int i = 0; i < length; i++)
                    if (pl[i] != pr[i] ||
                        pl[i] == char.MinValue)
                        return false;

                return pl[length] == char.MinValue;
            }
        }
        public static bool operator !=(NativeString left, string right)
        {
            fixed (char* pr = right)
            {
                char* pl = left.m_ptr;
                if (pl == null || pr == null)
                    return !(pl == null && pr == null);

                int length = right.Length;
                for (int i = 0; i < length; i++)
                    if (pl[i] != pr[i] ||
                        pl[i] == char.MinValue)
                        return true;

                return pl[length] != char.MinValue;
            }
        }

        public static bool operator ==(string left, NativeString right) => right == left;
        public static bool operator !=(string left, NativeString right) => right != left;
        #endregion
    }
}
