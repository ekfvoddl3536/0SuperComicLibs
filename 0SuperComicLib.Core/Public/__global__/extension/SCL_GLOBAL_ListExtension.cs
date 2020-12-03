using System.Collections.Generic;

namespace SuperComicLib
{
    public static class SCL_GLOBAL_ListExtension
    {
        public static string Convert2Str(this List<char> vs) => new string(vs.ToArray());
    }
}
