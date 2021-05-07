using System;

namespace SuperComicLib.Core
{
    public static class PRNG
    {
        private const int B = 8;
        private static readonly Random rnd = new Random();

        public static decimal NextDecimal() =>
            new decimal(
                (rnd.Next() << B) | rnd.Next(1 << B),
                (rnd.Next() << B) | rnd.Next(1 << B),
                (rnd.Next() << B) | rnd.Next(1 << B),
                (rnd.Next() & 1) == 0,
                (byte)rnd.Next(29));

        public static double NextDouble() => 
            rnd.NextDouble();

        public static float NextFloat() =>
            (float)(rnd.Next() % ushort.MaxValue) / (ushort.MaxValue - 1);

        public static int NextMax(int max) =>
            rnd.Next() % max;

        public static int Next(int min, int max) =>
            rnd.Next(min, max);

        public static int Next() =>
            rnd.Next();

        public static int NextSign() =>
            1 - ((rnd.Next() & 1) << 1);

        public static int Generate(int seed)
        {
            uint value = (uint)rnd.Next();

            for (uint u_seed = (uint)seed; u_seed != 0;)
                unchecked
                {
                    value = value * u_seed + value + 3;

                    u_seed =
                        (u_seed << 1) &
                        ((u_seed & byte.MaxValue) - value * 3);
                }

            return (int)value;
        }

        public static int NextInt() =>
            (rnd.Next() << 8) | (rnd.Next() & 0xFF);

        public static int Generate() => Generate(NextInt());

        public static void NextBytes(byte[] buffer) =>
            rnd.NextBytes(buffer);
    }
}
