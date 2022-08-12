using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SuperComicLib.Text
{
    public static unsafe class StringMarshal
    {
        public static IntPtr ToHGlobalLPSTR(string value, Encoding encoding)
        {
            if (value is null)
                return IntPtr.Zero;

            return ToHGlobalLPSTR_internal(value, encoding ?? Encoding.Default);
        }

        public static IntPtr ToHGlobalLPSTR_internal(string notnull_value, Encoding notnull_encoding)
        {
            int nb = notnull_value.Length + 1;
            long lnb = ((long)nb << 1) + nb; // nb * 3
            nb = (int)lnb;

            if (nb != lnb)
                throw new ArgumentOutOfRangeException(nameof(notnull_value));

            var ptr = Marshal.AllocHGlobal((IntPtr)nb);

            StringToAnsiString_internal(notnull_value, notnull_encoding, (byte*)ptr, nb);
            return ptr;
        }

        public static IntPtr ToCoTaskMemLPSTR(string value, Encoding encoding)
        {
            if (value is null)
                return IntPtr.Zero;

            return ToCoTaskMemLPSTR_internal(value, encoding ?? Encoding.Default);
        }

        public static IntPtr ToCoTaskMemLPSTR_internal(string notnull_value, Encoding notnull_encoding)
        {
            int nb = notnull_value.Length + 1;
            long lnb = ((long)nb << 1) + nb; // nb * 3
            nb = (int)lnb;

            if (nb != lnb)
                throw new ArgumentOutOfRangeException(nameof(notnull_value));

            var ptr = Marshal.AllocCoTaskMem(nb);

            StringToAnsiString_internal(notnull_value, notnull_encoding, (byte*)ptr, nb);
            return ptr;
        }

        private static int StringToAnsiString_internal(string s, Encoding enc, byte* buf, int bufLen)
        {
            fixed (char* first = s)
            {
                int convBytes = enc.GetBytes(first, s.Length, buf, bufLen);
                buf[convBytes] = 0;

                return convBytes;
            }
        }
    }
}
