using System;
using System.Text;

namespace SuperComicLib.Text
{
    public readonly unsafe struct StringMarshalBuilder
    {
        public readonly Encoding defaultEncoding;

        public StringMarshalBuilder(Encoding encoding) => 
            defaultEncoding = encoding ?? throw new ArgumentNullException(nameof(encoding));

        public IntPtr ToHGlobalLPSTR(string value)
        {
            if (value is null)
                return IntPtr.Zero;

            defaultEncoding.GetMaxByteCount(1);
            return StringMarshal.ToHGlobalLPSTR_internal(value, defaultEncoding);
        }

        public IntPtr ToCoTaskMemLPSTR(string value)
        {
            if (value is null)
                return IntPtr.Zero;

            return StringMarshal.ToCoTaskMemLPSTR_internal(value, defaultEncoding);
        }
    }
}
