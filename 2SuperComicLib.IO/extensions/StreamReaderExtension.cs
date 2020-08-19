using System;
using System.IO;

namespace SuperComicLib
{
    public static class StreamReaderExtension
    {
        public static string MoveNext(this StreamReader reader)
        {
            if (reader.EndOfStream)
                return null;

            string now;
            do
                now = reader.ReadLine().Trim();
            while (reader.EndOfStream == false && now.Length == 0);

            return now;
        }

        public static string MoveNext(this StreamReader reader, ref int count)
        {
            if (reader.EndOfStream)
                return null;

            string now;
            do
            {
                now = reader.ReadLine().Trim();
                count++;
            }
            while (reader.EndOfStream == false && now.Length == 0);

            return now;
        }

        public static bool EndOfLine(this StreamReader sr)
        {
            int read = sr.Peek();
            if (read == '\r')
            {
                sr.Read();
                if (sr.Peek() == '\n')
                    sr.Read();

                return true;
            }
            if (read == '\n')
            {
                sr.Read();
                return true;
            }
            return false;
        }
    }
}
