namespace SuperComicLib
{
    public static class StringExtension
    {
        public static HashedString ToHashedString(this string str) => new HashedString(str);
    }
}
