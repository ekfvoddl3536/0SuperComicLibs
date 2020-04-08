namespace SuperComicLib.Runtime
{
    public interface IFastComparerEx<T>
    {
        bool Lesser(T left, T right);

        bool LessOrEquals(T left, T right);

        bool Greater(T left, T right);

        bool GreatOrEquals(T left, T right);

        bool EqualsAB(T left, T right);

        bool NotEqualsAB(T left, T right);
    }
}
