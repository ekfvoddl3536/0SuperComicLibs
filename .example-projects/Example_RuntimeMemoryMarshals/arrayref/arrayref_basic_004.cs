using System;
using System.Linq;
using SuperComicLib;
using SuperComicLib.RuntimeMemoryMarshals; /* Add to 'using' */

namespace Example_RuntimeMemoryMarshals
{
    public static unsafe partial class ArrayrefExample
    {
        [Example("arrayref/basic/004", "Creating subarrays and copies")]
        public static void Basic004()
        {
            arrayref<int> values = new arrayref<int>(1024);

            int[] arr = values.AsManaged();
            MyUtils.FillRandomValues(arr);

            //  Get a subarray. This method does not create a new array.
            arrayrefSegment<int> segment = values.Slice(10, 10);

            Console.WriteLine("(segment[0] == arr[10]) = " + (segment[0] == arr[10]));
            Console.WriteLine();

            values[10] = 200;
            Console.WriteLine("Executed 'values[10] = 200'");
            Console.WriteLine("segment[0] = " + segment[0]);
            Console.WriteLine("(segment[0] == arr[10]) = " + (segment[0] == arr[10]));
            Console.WriteLine();

            //  The following methods create a subarray or a copy of the original array.
            int[] toArray1_segment = segment.ToArray(); // `System.Linq` extension
            int[] toArray2_segment = segment.AsSpan().ToArray();

            int[] toArray1_values = values.ToArray(); // `System.Linq` extension
            int[] toArray2_values = values.AsSpan().ToArray();

            toArray1_segment[0] = 100;
            Console.WriteLine("Executed 'toArray1_segment[10] = 100'");
            Console.WriteLine("segment[0] = " + segment[0]);
            Console.WriteLine("toArray2_segment[0] = " + toArray2_segment[0]);
            Console.WriteLine();

            toArray1_values[0] = -100;
            Console.WriteLine("Executed 'toArray1_values[10] = -100'");
            Console.WriteLine("values[0] = " + values[0]);
            Console.WriteLine("toArray2_values[0] = " + toArray2_values[0]);
            Console.WriteLine();

            // When you are finished using it, you must free the memory.
            values.Dispose();
        }
    }
}
