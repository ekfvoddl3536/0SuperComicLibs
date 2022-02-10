using System.Runtime.CompilerServices;

public static class SUPERCOMICLIB_CORE__BooleanExtension__
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int ToInt(this bool value) => *(sbyte*)&value;
}