using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SuperComicLib
{
    public static class SCL_GLOBAL_ListExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Convert2Str(this List<char> vs) => new string(vs.ToArray());
    }
}
