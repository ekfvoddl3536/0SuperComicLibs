using System;
using SuperComicLib.RuntimeMemoryMarshals; /* Add to 'using' */

namespace Example_RuntimeMemoryMarshals
{
    public static unsafe partial class RefpointExample
    {
        [Example("refpoint/basic/001", "basic refpoint<> usage")]
        public static void Basic001()
        {
            int[] sample = MyUtils.RandomInts(1024);

            // Same as: ref sample[8]. but, array bounds checking is skipped.
            ref int idx8value = ref sample.refdata(8);

            // Unlike ref int, a type that allows address operations like a pointer.
            ref refpoint<int> idx7value = ref sample.refpoint(10);

            // WARNING: this should always be passed with the ref keyword.
            idx7value = ref idx7value.sub(3);

            // can be called sequentially.
            ref refpoint<byte> cast14value = ref idx7value.add(7).cast<byte>();

            // True
            bool isIndex7 = idx7value == cast14value.cast<int>().sub(7);
            Console.WriteLine("isIndex7 -> " + isIndex7);

            // True
            bool isindex8ValueEQ = sample[8] == idx8value;
            Console.WriteLine("isindex8ValueEQ -> " + isindex8ValueEQ);
        }
    }
}
