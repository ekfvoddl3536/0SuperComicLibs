using System;
using System.Linq;
using SuperComicLib.RuntimeMemoryMarshals; /* Add to 'using' */

namespace ExampleProject
{
    public static unsafe partial class RefpointExample
    {
        [Example("refpoint/basic/002", "enumerate elements")]
        public static void Basic002()
        {
            int[] sample = MyUtils.RandomInts(20);

            ref refpoint<int> first = ref sample.refpoint();
            ref refpoint<int> last = ref first.add(sample.Length);

            // forward.
            for (; first != last; first = ref first.inc)
            {
                Console.WriteLine(first.value);

                // Since it is a ref return, value can be assigned.
                first.value = 1;
            }

            // Accumulates by adding all the values in the array.
            int total = sample.Aggregate((acc, val) => acc + val);

            // If all values are modified to 1, this result is True.
            bool isLengthEQ = total == sample.Length;

            Console.WriteLine("is all values are 1 --> " + isLengthEQ);
        }
    }
}
