using System;
using System.Collections.Generic;
using System.Reflection;

namespace SuperComicWorld
{
    internal static class ValueContractHelper
    {
        private static readonly MethodInfo meth =
            typeof(ValueContractHelper).GetMethod(nameof(CheckValue));

        public static int Check(object fv, object ov, Type ft) => 
            (int)meth.MakeGenericMethod(ft).Invoke(null, new object[] { fv, ov });

        public static int CheckValue<T>(T left, T right) where T : struct =>
            Comparer<T>.Default.Compare(left, right);
    }
}
