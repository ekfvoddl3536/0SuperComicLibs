namespace SuperComicLib.Numerics
{
    internal interface IRuntimeBitArray
    {
        Bits Create(int numberOfBits);
        
        int ToLength(int n);
    }
}
