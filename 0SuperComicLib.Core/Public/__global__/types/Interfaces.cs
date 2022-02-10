namespace SuperComicLib
{
    public interface IConvertible<out T>
    {
        T ConvertTo();
    }

    public interface ICloneable<out T>
    {
        T Clone();
    }
}
