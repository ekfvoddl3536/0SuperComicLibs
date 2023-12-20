using System;
using SuperComicLib;
using SuperComicLib.RuntimeMemoryMarshals; /* Add to 'using' */

namespace ExampleProject
{
    public static unsafe partial class ArrayrefExample
    {
        [Example("arrayref/basic/003", "Methods for operating with unmanaged code")]
        public static void Basic003()
        {
            arrayref<int> values = new arrayref<int>(1024);

            int[] arr = values.AsManaged();
            MyUtils.FillRandomValues(arr);

            // TIPS: When fixed is needed, high-performance scenarios can be achieved by calling the 'GetDataPointer' method instead.
            int* ptr = (int*)values.GetDataPointer();

            // True
            Console.WriteLine("(arr[0] == ptr[0]) = " + (arr[0] == ptr[0]));
            Console.WriteLine();

            arr[0] = 10;
            Console.WriteLine("Executed 'arr[0] = 10'");
            Console.WriteLine("ptr[0] = " + ptr[0]);
            Console.WriteLine("(arr[0] == ptr[0]) = " + (arr[0] == ptr[0]));
            Console.WriteLine();

            ptr[0] = -10;
            Console.WriteLine("Executed 'ptr[0] = -10'");
            Console.WriteLine("arr[0] = " + arr[0]);
            Console.WriteLine("(arr[0] == ptr[0]) = " + (arr[0] == ptr[0]));
            Console.WriteLine();

            //  Using the 'AsSpan' extension method, it can be converted to 'NativeSpan<T>'.
            //  A variety of memory manipulation functions are available, including memory initialization.
            //  For details, refer to the 'NativeSpan/...' example.
            NativeSpan<int> span = values.AsSpan();

            span[0] = 20;
            Console.WriteLine("Executed 'span[0] = 20'");
            Console.WriteLine("arr[0] = " + arr[0]);
            Console.WriteLine("ptr[0] = " + ptr[0]);
            Console.WriteLine("(arr[0] == ptr[0]) = " + (arr[0] == ptr[0]));
            Console.WriteLine();

            // When you are finished using it, you must free the memory.
            values.Dispose();
        }
    }
}
