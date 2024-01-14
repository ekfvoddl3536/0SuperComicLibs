using System;
using System.IO;
using SuperComicLib;
using SuperComicLib.Runtime; /* Add to 'using' */

namespace ExampleProject
{
    public static unsafe partial class ArrayrefExample
    {
        [Example("arrayref/expert/002.2", "Excellent use case, variation 1. (without pointer)")]
        public static void Exper002_2()
        {
            //  Allocate 16KiB buffer
            arrayref<byte> buffer = new arrayref<byte>(16384);

            //  If you want all data to be initialized to 0, like a managed array, do this:
            //  case 1 (recommend, FAST and SIMPLE):
            buffer.AsSpan().Clear();

            //  case 2:
            Array.Clear(buffer.AsManaged(), 0, buffer.Length);

            byte[] bytes = buffer.AsManaged();

            Stream stream = GetStream();

            NativeSpan<byte> span = buffer.GetDataReferenceAsSpan();

            span.getAs<long>(0) = 10;
            span.getAs<double>(8) = 330;

            //  Useful when a large amount of data needs to be assigned to the same type.
            NativeSpan<int> intspan = span.Slice(16, 1024).Cast<int>();
            for (int i = 0; i < 1024; ++i)
                intspan[i] = i + 1;

            stream.Write(bytes, 0, 16 + 1024);

            //  Reset
            stream = GetStream(stream);

            buffer.AsSpan().Clear();

            int readCount = stream.Read(bytes, 0, bytes.Length);
            Console.WriteLine("readCount = " + readCount);

            long v0 = span.getAs<long>(0);
            double v1 = span.getAs<double>(8);

            Console.WriteLine("span.getAs<long>(0) = " + v0);
            Console.WriteLine("span.getAs<double>(8) = " + v1);

            // When you are finished using it, you must free the memory.
            buffer.Dispose();
        }
    }
}
