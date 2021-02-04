using System;
using System.Collections;

namespace SuperComicLib.Threading
{
    public static partial class ThreadFactory
    {
        public static IEnumerator Create(Action<IWorker> taskMethod) => new E0(taskMethod);

        public static IEnumerator Create(int threadCount, params Action<int, IMultiWorker>[] taskMethods) =>
            taskMethods == null
            ? throw new ArgumentNullException(nameof(taskMethods))
            : taskMethods.Length < 2
            ? throw new ArgumentOutOfRangeException(nameof(taskMethods))
            : threadCount < 2 || threadCount >= Environment.ProcessorCount
            ? throw new ArgumentOutOfRangeException($"out of range: {nameof(threadCount)}. min: 2, max: {Environment.ProcessorCount - 1}")
            : new E1(taskMethods, threadCount);
    }
}
