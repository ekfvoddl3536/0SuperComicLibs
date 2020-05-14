public static class SCL_CORE_BooleanExtension
{
    public static int ToInt(this bool value) => 
        SuperComicLib.Core.Bool2Int._func(value);
}