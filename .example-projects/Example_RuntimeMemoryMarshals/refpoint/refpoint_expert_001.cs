using System;
using SuperComicLib.RuntimeMemoryMarshals; /* Add to 'using' */

namespace Example_RuntimeMemoryMarshals
{
    public static unsafe partial class RefpointExample
    {
        [Example("refpoint/expert/001", "Convenience and advanced features. (`unary operator+` and `fixed`)")]
        public static void Expert001()
        {
            object[] dummy = NewLargeArray();

            Console.WriteLine("GC Memory: " + GC.GetTotalMemory(false));

            int[] sample = MyUtils.RandomInts(48);

            //  This type of conversion checks the bounds of the array and allows safe access.
            ref refpoint<int> current = ref ILUnsafe.AsRefpoint(ref sample[0]);

            //  If you know the target .NET runtime in advance, you can take advantage of the
            //  following APIs for higher execution performance:

            //  [ Mono ]
            ref refpoint<int> mono = ref sample.refpoint_mono();

            //  [ CoreCLR ]
            ref refpoint<int> coreclr = ref sample.refpoint_clr();

            fixed (int* ptr = sample)
            {
                Console.WriteLine(" 'sample' Address(Hex): " + ((IntPtr)ptr).ToString("X12"));

                //  The reference address value can be read in pointer format using the 'address' property.
                //  However, the reference address converted to a pointer is not tracked by JIT, and its
                //  stability cannot be guaranteed over long periods of use.
                Console.WriteLine("'current' Address(Hex): " + ((IntPtr)current.address).ToString("X12"));

                Console.WriteLine();
            }

            fixed (int* ptr2 = current.add(4))
            {
                dummy = Destroy(dummy, sample);

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                GC.WaitForFullGCComplete();

                Console.WriteLine("GC Memory, After GC.Collect(): " + GC.GetTotalMemory(false));
                Console.WriteLine();

                Console.WriteLine("pinned 'current.add(4)' Address(Hex), After GC.Collect(): " + ((IntPtr)ptr2).ToString("X12"));
                Console.WriteLine("pinned 'current.add(4)', 'current' elementOffset diff: " + ILUnsafe.AsRefpoint(ref *ptr2).elementOffset(ref current));
                Console.WriteLine();
            }

            //  The `unary operator +` reads the value pointed to by the
            //  reference address into the specified type.
            int value1 = +current;
            int value2 = sample[0];

            // True
            bool isEQ = value1 == value2;
            Console.WriteLine("sample[0] == +current --> " + isEQ);

            sample[10] = -100;

            //  Through indexing, it can be accessed like an array
            //  without having to use the add method.
            Console.WriteLine(+current[10]);

            long acc = 0;
            for (int i = 0; i < sample.Length; i++)
                acc += +current[i];

            Console.WriteLine("Cumulative sum result: " + acc);
        }

        private static object[] Destroy(object[] dummy, int[] sample)
        {
            Random random = new Random();

            dummy[random.Next(dummy.Length)] = sample;

            return null;
        }

        private static object[] NewLargeArray() => new object[16777216 * 4];
    }
}
