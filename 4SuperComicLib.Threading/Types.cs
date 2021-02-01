using System;

namespace SuperComicLib.Threading
{
    public interface IWorker
    {
        Awaiter Invoke(Action action);
    }
}
