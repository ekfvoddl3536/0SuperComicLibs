namespace SuperComicLib.IO.AdvancedParallel
{
    public delegate void ParallelReadHandler(ref OffsetParallelStream remote_stream, uint worker_index);
}
