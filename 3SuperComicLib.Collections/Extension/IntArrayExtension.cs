namespace SuperComicLib.Collections
{
    public static class IntArrayExtension
    {
        public static int BinarySearch(this int[] vs, int index, int length, int value)
        {
            int hi = index + length - 1;
            while (index <= hi)
            {
                int mid = index + ((hi - index) >> 1);

                int current = vs[mid];

                if (current == value)
                    return mid;
                else if (current < value)
                    index = mid + 1;
                else
                    hi = mid - 1;
            }

            return ~index;
        }

        public static int BinarySearch(this int[] vs, int value) => BinarySearch(vs, 0, vs.Length, value);
    }
}
