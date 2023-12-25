using System;
using System.Runtime.InteropServices;
using SuperComicLib; /* Add to 'using' */

namespace ExampleProject
{
    public static unsafe partial class NativeSpanExample
    {
        [Example("NativeSpan/basic/002", "Basic usage")]
        public static void Basic002()
        {
            byte* ptr = (byte*)Marshal.AllocHGlobal(1024);

            //  You can also initialize using a starting address (inclusive)
            //  and an ending address (exclusive).
            //
            //  The range length is calculated as (end address - start address),
            //  so if the end address and start address are the same, the length is 0.
            NativeSpan<byte> span = new NativeSpan<byte>(ptr, ptr + 1024);

            span.Clear();

            span.Fill(1);

            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
