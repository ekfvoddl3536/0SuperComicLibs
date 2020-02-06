using System.Reflection;

namespace SuperComicLib.Reflection
{
    internal static class MethodBaseExtension
    {
        internal static string GetMethodName(this MethodBase meth, int count, bool replaced)
        {
            string res = $"{meth.Name}_Patch{count}";
            if (replaced)
                res += "_MOD";
            return res;
        }
    }
}
