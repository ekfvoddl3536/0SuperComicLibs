using System;
using System.Reflection;

namespace SuperComicLib
{
    public static class DataObject_Helper
    {
        public static readonly char[] split_char_0 = new char[] { '=' };
        public static readonly char[] split_char_1 = new char[] { ' ' };
        public static readonly char[] split_char_2 = new char[] { ':' };

        public static readonly string[] split_nl = new string[] { Environment.NewLine };

        public const StringSplitOptions opt = StringSplitOptions.RemoveEmptyEntries;
        public const BindingFlags stbflag = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        public const string sbool = "bool";
        public const string s32 = "int";
        public const string s64 = "long";
        public const string sr4 = "float";
        public const string sr8 = "double";
        public const string sstr = "string";
    }
}
