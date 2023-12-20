using System;
using System.IO;
using SuperComicLib.RuntimeMemoryMarshals; /* Add to 'using' */

namespace ExampleProject
{
    public static unsafe partial class ArrayrefExample
    {
        [Example("arrayref/expert/002", "Excellent use case")]
        public static void Expert002()
        {
            //  This example shows what the effect is when there is no conversion
            //  cost between managed and unmanaged arrays.

            //  Allocate 16KiB buffer
            arrayref<byte> buffer = new arrayref<byte>(16384);

            //  Conversion
            byte[] bytes = buffer.AsManaged();

            Stream stream = GetStream();

            //  Write 8-bytes
            *(long*)buffer.GetDataPointer() = 10;

            stream.Write(bytes, 0, 8);

            //  Reset
            stream = GetStream(stream);

            //  This operation ensures that 0 is output when data cannot be read.
            *(long*)buffer.GetDataPointer() = 0;

            int readCount = stream.Read(bytes, 0, 8);
            Console.WriteLine("readCount = " + readCount);

            Console.WriteLine("*(long*)buffer.GetDataPointer() = " + *(long*)buffer.GetDataPointer());

            // When you are finished using it, you must free the memory.
            buffer.Dispose();
        }

        //  You can use a file stream or any other stream.
        private static Stream GetStream(Stream previous = null)
        {
            if (previous != null)
            {
                previous.Seek(0, SeekOrigin.Begin);
                return previous;
            }

            return new MemoryStream(1024);
        }
    }
}
