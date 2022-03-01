using System.Runtime.CompilerServices;

namespace SuperComicLib.Collections
{
    public readonly ref struct Index2D
    {
        public readonly int Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Index2D(int rowIndex, int colSize) => Value = rowIndex * colSize;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Index2D(int rowIndex, int colIndex, int colSize) =>
            Value = rowIndex * colSize + colIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(Index2D v) => v.Value;
    }
}
