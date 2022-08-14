using System.IO;

namespace SuperComicLib.IO
{
    internal static unsafe class ByteStreamHelper
    {
        public static int IndexOfNewLine(Stream src, byte* pbuf, int count)
        {
            for (int i = 0; i < count; i++, pbuf++)
                if (*pbuf == '\r')
                {
                    if (i + 1 < count)
                    {
                        if (pbuf[1] == '\n')
                            return i + 1;
                    }
                    else if (src.ReadByte() == '\n')
                        return i + 1;

                    return i;
                }
                else if (*pbuf == '\n')
                    return i;

            return -1;
        }
    }
}
