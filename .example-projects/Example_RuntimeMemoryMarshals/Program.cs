using System;

namespace ExampleProject
{
    public static partial class Program
    {
        public static int Main(string[] args)
        {
            //
            //  If you're looking for examples, look for source files in folder names that match the example theme.
            //  
            //  If you are looking for examples of 'refpoint<T>', check out the source files in the 'refpoint' folder.
            //

            var loadedExamples = LoadExamples(out int maxTL, out int maxSL);
            if (args.Length == 0)
            {
                PrintMenu(loadedExamples, maxTL, maxSL);
                return 0;
            }
            else if (args.Length > 1)
            {
                Console.WriteLine("too many arguments.");
                Console.WriteLine();

                return 1;
            }

            var input = GetTitleName(loadedExamples, args[0]);
            if (IsInvalidTitleName(input))
            {
                Console.WriteLine("Invalid name.");
                Console.WriteLine();

                return 2;
            }

            return TryInvoke(loadedExamples, input) - 1;
        }
    }
}
