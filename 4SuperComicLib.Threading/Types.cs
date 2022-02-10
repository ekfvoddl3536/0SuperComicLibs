using System;

namespace SuperComicLib.Threading
{
    public interface IWorker
    {
        Awaiter Invoke(Action action);
    }

    public interface IMultiWorker
    {
        Awaiter Invoke(int handle, Action action);
    }

    public interface ILoopState
    {
        void Break();

        void Stop();
    }
}