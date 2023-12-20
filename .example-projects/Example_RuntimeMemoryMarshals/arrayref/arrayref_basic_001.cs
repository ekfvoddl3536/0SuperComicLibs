using System;
using SuperComicLib.RuntimeMemoryMarshals; /* Add to 'using' */

namespace ExampleProject
{
    public static partial class ArrayrefExample
    {
        [Example("arrayref/basic/001", "Basic usage of 'arrayref<>'")]
        public static void Basic001()
        {
            arrayref<int> values = new arrayref<int>(1024);

            values[0] = 10;

            Console.WriteLine("values[0] = " + values[0]);

            //  The 'AsManaged' method has no conversion costs,
            //  and does not create a new managed array.
            int[] arr = values.AsManaged();
            MyUtils.FillRandomValues(arr);

            //  The managed array converted by the 'AsManaged' method
            //  and the array data of 'x' refer to the same location,
            //  so the results are reflected immediately without any feedback.
            // True
            Console.WriteLine("arr = values.AsManaged();");
            Console.WriteLine("(arr[0] == values[0]) = " + (arr[0] == values[0]));

            // Like an array, you can enumerate it using foreach.
            long acc1 = 0;
            foreach (int x in values)
                acc1 += x;

            // You can enumerate using an index. In this case, no array bounds checking is performed,
            // so if you need array checking, use the at method instead.
            long acc2 = 0;
            for (int i = 0; i < values.Length; i++)
            {
                acc2 += values[i];

                //  This is a fairly safe method as array bounds are checked automatically.
                //  This loop guarantees that index i is less than the range of the array, so it is not recommended.
                // acc2 += values.at(i);
            }

            Console.WriteLine("acc1 = " + acc1);
            Console.WriteLine("acc2 = " + acc2);
            Console.WriteLine();

            // True
            Console.WriteLine("(acc1 == acc2) = " + (acc1 == acc2));

            // When you are finished using it, you must free the memory.
            values.Dispose();
        }
    }
}
