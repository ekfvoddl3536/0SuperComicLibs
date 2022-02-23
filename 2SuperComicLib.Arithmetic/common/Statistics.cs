using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Arithmetic
{
    public static class Statistics
    {
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NormalDistribution(double x, double mean, double stddev)
        {
            double temp = Math.Pow(stddev, 2);
            return
                1 / Math.Sqrt(2 * Math.PI * temp) *
                Math.Exp(-Math.Pow(x - mean, 2) / (2 * temp));
        }
    }
}
