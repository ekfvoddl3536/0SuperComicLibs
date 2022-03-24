namespace SuperComicLib
{
    public interface IBitset
    {
        int Count();
    }

    public interface ICalculable<T> where T : unmanaged
    {
        T ADD(T other);
        T SUB(T other);
        T MUL(T other);
        T DIV(T other);
        T MOD(T other);
    }

    public interface IBitCalculable<T> : ICalculable<T> where T : unmanaged
    {
        T OR(T other);
        T AND(T other);
        T XOR(T other);
        T NOT();
        T LSHIFT(int shift);
        T RSHIFT(int shift);
    }
}
