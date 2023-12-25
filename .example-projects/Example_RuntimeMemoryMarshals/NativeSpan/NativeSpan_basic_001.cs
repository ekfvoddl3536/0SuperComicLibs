using System;
using Microsoft.SqlServer.Server;
using SuperComicLib; /* Add to 'using' */

namespace ExampleProject
{
    public static unsafe partial class NativeSpanExample
    {
        [Example("NativeSpan/basic/001", "Basic usage")]
        public static void Basic001()
        {
            int[] sample = MyUtils.RandomInts(10);

            fixed (int* ptr = sample)
            {
                //  'NativeSpan<T>' refers to other fixed range of memory
                //  and cannot allocate new memory space on its own.
                NativeSpan<int> span = new NativeSpan<int>(ptr, sample.Length);

                int max = int.MinValue;
                for (int i = 0; i < span.Length; i++)
                {
                    max = Math.Max(max, span[i]);

                    //  This is a fairly safe method as array bounds are checked automatically.
                    //  This loop guarantees that index i is less than the range of the array, so it is not recommended.
                    // max = Math.Max(max, span.at(i));
                }

                Console.WriteLine("Max = " + max);
                Console.WriteLine();

                NativeSpan<byte> bytes = span.Cast<int, byte>();
                Console.WriteLine("span.Length = " + span.Length);
                Console.WriteLine("span.capacity = " + span.capacity());
                Console.WriteLine("bytes.Length = " + bytes.Length);
                Console.WriteLine("bytes.capacity = " + bytes.capacity());
                Console.WriteLine();

                NativeConstSpan<int> subspan = span.Slice(5, 5);
                Console.WriteLine("subspan = span.Slice(5, 5);");
                Console.WriteLine("(subspan[0] == span[5]) = " + (subspan[0] == span[5]));
                Console.WriteLine();

                //  'NativeSpan<T>' has no ownership of the target memory address by default,
                //  and therefore has no right to free the memory in principle.
                //  Therefore, there is no need to free memory for normal use.
            }
        }
    }
}
