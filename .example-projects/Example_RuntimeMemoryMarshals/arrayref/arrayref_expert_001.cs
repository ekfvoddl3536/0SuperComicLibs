using System;
using SuperComicLib.RuntimeMemoryMarshals; /* Add to 'using' */

namespace ExampleProject
{
    public static unsafe partial class ArrayrefExample
    {
        [Example("arrayref/expert/001", "C++ STL-like API")]
        public static void Expert001()
        {
            arrayref<int> values = new arrayref<int>(1024);

            MyUtils.FillRandomValues(values.AsManaged());

            arrayref<int>.iterator begin = values.begin();
            arrayref<int>.iterator end = values.end();

            // forward
            long fsum = 0;
            for (; begin != end; begin++)
                fsum += begin.value;

            // dummy print
            Console.WriteLine(fsum);


            arrayref<int>.reverse_iterator rbegin = values.rbegin();
            arrayref<int>.reverse_iterator rend = values.rend();

            // backward
            long bsum = 0;
            for (; rbegin != rend; rbegin++)
                bsum += rbegin.value;

            // dummy print
            Console.WriteLine(bsum);


            // conversion
            arrayref<int>.const_iterator iterator = begin;

            // '.base'
            arrayref<int>.iterator forward = rbegin.@base();


            // When you are finished using it, you must free the memory.
            values.Dispose();
        }
    }
}
