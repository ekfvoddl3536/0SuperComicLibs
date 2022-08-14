namespace SuperComicLib.IO
{
    public enum ResolverState
    {
        Fail = -1,
        Continue = 0,
        Success = 1,
    }

    public enum StreamStateControl
    {
        None,
        Restart,
        PushAndReturn,
    }
}
