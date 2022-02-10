using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SuperComicLib.Threading
{
    public static class TaskExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDisposed_s(this Task task) =>
            !task.IsFaulted && !task.IsCanceled && !task.IsCompleted && task.Status == TaskStatus.RanToCompletion;

        // public static unsafe bool IsDisposed(this Task task)
        // {
        //     TypedReference tr = __makeref(task);
        //     return (*(int*)(**(byte***)&tr + IntPtr.Size * 7 + sizeof(int)) & 0x40000) != 0;
        // }
    }
}
