namespace SuperComicLib
{
    using static DataObject_Helper;
    public static class StringExtension_1
    {

        public static string[] SplitEqual2(this string str) => str.Split(split_char_0, 2);
        public static string[] SplitSpace2(this string str) => str.Split(split_char_1, 2);


        public static string[] SplitSpace2Rm(this string str) => str.Split(split_char_1, 2, opt);
    }
}
