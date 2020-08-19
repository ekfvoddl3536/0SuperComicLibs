using System.Collections.Generic;

namespace SuperComicLib
{
    public static class ListExtension
    {
        public static string Convert2Str(this List<char> vs) => new string(vs.ToArray());
    }
}
