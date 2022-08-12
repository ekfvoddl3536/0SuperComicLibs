using System;
using System.IO;
using System.Text;

namespace SuperComicLib.Text
{
    public static unsafe class AutoEncoding
    {
        public static Encoding Detect(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            if (info.Exists == false)
                throw new FileNotFoundException();

            byte* ptr = stackalloc byte[4];
            int numRead = 0;

            FileStream stream = info.OpenRead();
            do
            {
                int read = stream.ReadByte();
                if (read < 0)
                    break;

                ptr[numRead++] = (byte)read;
            } while (numRead < 5);
            stream.Close();

            uint data = *(uint*)ptr;
            if (numRead >= 4)
            {
                if (data == 0xFF_FE_00_00)
                    return Encoding.GetEncoding("utf-32BE");
                else if (data == 0xFE_FF)
                    return Encoding.UTF32;
            }

            if (numRead >= 2)
            {
                if (data == 0xFF_FE)
                    return Encoding.BigEndianUnicode;
                else if (data == 0xFE_FF)
                    return Encoding.Unicode;
            }

            if (numRead >= 3)
            {
                if (data == 0xBF_BB_EF)
                    return Encoding.UTF8;
                else if (data == 0x76_2F_2B)
                    return Encoding.UTF7;
            }

            throw new NotSupportedException();
        }
    }
}
