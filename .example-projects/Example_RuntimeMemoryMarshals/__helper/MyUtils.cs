using System;

namespace ExampleProject
{
    internal static class MyUtils
    {
        // Generate random ints
        public static int[] RandomInts(int count)
        {
            int[] sample = new int[count];

            FillRandomValues(sample);

            return sample;
        }

        public static void FillRandomValues(int[] target)
        {
            Random random = new Random();

            for (int i = 0; i < target.Length; i++)
                target[i] = random.Next();
        }
    }
}
