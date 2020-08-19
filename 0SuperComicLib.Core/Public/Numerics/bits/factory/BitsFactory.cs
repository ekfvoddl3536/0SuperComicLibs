using System;

namespace SuperComicLib.Numerics
{
    public static class BitsFactory
    {
        private static readonly IRuntimeBitArray inst =
            IntPtr.Size == sizeof(long)
            ? Amd64Factory.Default
            : X86Factory.Default;

        public static Bits Create(int numberOfBits) => inst.Create(numberOfBits);

        public static Bits CreateX86(int numberOfBits) => X86Factory.Default.Create(numberOfBits);

        public static int ToLength(int n) => inst.ToLength(n);

        public static int ToLengthX86(int n) => X86Factory.Default.ToLength(n);

        #region instance
        private sealed class Amd64Factory : IRuntimeBitArray
        {
            private static readonly IRuntimeBitArray inst = new Amd64Factory();

            public static IRuntimeBitArray Default => inst;

            private Amd64Factory() { }

            public Bits Create(int numberOfBits)
            {
                if (numberOfBits % 64 != 0)
                    numberOfBits += numberOfBits % 64;

                return new Amd64Bits(numberOfBits);
            }

            public int ToLength(int n) =>
                n > 0
                ? (n - 1) / 64 + 1
                : 0;
        }

        private sealed class X86Factory : IRuntimeBitArray
        {
            private static readonly IRuntimeBitArray inst = new X86Factory();

            public static IRuntimeBitArray Default => inst;

            private X86Factory() { }

            public Bits Create(int numberOfBits)
            {
                if (numberOfBits % 32 != 0)
                    numberOfBits += numberOfBits % 32;

                return new X86Bits(numberOfBits);
            }

            public int ToLength(int n) =>
                n > 0
                ? (n - 1) / 32 + 1
                : 0;
        }
        #endregion
    }
}
