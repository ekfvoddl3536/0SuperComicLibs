namespace SuperComicLib.IO.AdvancedParallel
{
    internal interface IParallelFileReader
    {
        int UpdateBuffer(byte[] buffer, ref Range64 offset);
    }
}
