using System;
using System.Collections.Generic;
using System.Linq;
using SuperComicLib.RuntimeMemoryMarshals; /* Add to 'using' */

namespace Example_RuntimeMemoryMarshals
{
    public static partial class ArrayrefExample
    {
        [Example("arrayref/basic/002", "More examples of using `System.Linq`")]
        public static void Basic002()
        {
            arrayref<int> values = new arrayref<int>(1024);

            int[] arr = values.AsManaged();
            MyUtils.FillRandomValues(arr);

            Console.WriteLine("values.Any(x => x > 500) = " + values.Any(x => x > 500));
            Console.WriteLine("values.Max() = " + values.Max());

            //  'arrayref<T>' is an implementation of IEnumerable<T>, casting is possible.
            IEnumerable<int> linq = values;

            IEnumerable<int> take = linq.Skip(32).Take(10).OrderBy(x => x);

            Console.WriteLine();

            foreach (int x in take)
                Console.Write(x + ", ");

            Console.WriteLine();

            // When you are finished using it, you must free the memory.
            values.Dispose();
        }
    }
}
