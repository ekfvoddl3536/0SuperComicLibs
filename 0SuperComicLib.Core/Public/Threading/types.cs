namespace SuperComicLib.Threading
{
    public interface ISpinLockBase
    {
        bool IsLocked { get; }

        bool TryEnter(int millisecondsTimeout);

        void Enter();

        void Exit();
    }
}
