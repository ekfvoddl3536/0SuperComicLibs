namespace SuperComicLib
{
    public static unsafe class BitCounter
    {
        public static int POPCNT(this ulong value) => POPCNT(*(long*)&value);

        public static int POPCNT(this long value)
        {
            int* ptr = (int*)&value;
            return POPCNT(ptr[0]) + POPCNT(ptr[1]);
        }

        public static int POPCNT(this uint value) => POPCNT(*(int*)&value);

        public static int POPCNT(this int value)
        {
            int c = value - ((value >> 1) & 0x5555_5555);
            c = (c & 0x3333_3333) + ((c >> 2) & 0x3333_3333);
            return (((c + (c >> 4)) & 0x0F0F_0F0F) * 0x0101_0101) >> 24;
        }

        public static int POPCNT_S(this sbyte value) => POPCNT_S(*(byte*)&value);

        public static int POPCNT_S(this byte value)
        {
            int c = value - ((value >> 1) & 0x55);
            c = ((c >> 2) & 0x33) + (c & 0x33);
            return ((c >> 4) + c) & 0x0F;
        }

        public static int POPCNT_S(this short value) => POPCNT_S(*(ushort*)&value);

        public static int POPCNT_S(this ushort value)
        {
            int c = value - ((value >> 1) & 0x5555);
            c = (c & 0x3333) + ((c >> 2) & 0x3333);
            return (((c + (c >> 4)) & 0x0F0F) * 0x0101) >> 8;
        }
    }
}
