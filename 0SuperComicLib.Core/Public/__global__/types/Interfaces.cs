namespace SuperComicLib
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}
