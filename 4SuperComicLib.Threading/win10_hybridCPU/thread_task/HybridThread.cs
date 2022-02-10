using System;
using System.Threading;

namespace SuperComicLib.Threading
{
    public sealed class HybridThread : HybridThreadContext
    {
        private readonly Thread m_thread;

        public HybridThread(Action start, Preference mode) : base(start, mode) => m_thread = new Thread(NORMAL_RUN);

        public HybridThread(Action<object> start, Preference mode) : base(start, mode) => m_thread = new Thread(PARAM_RUN);

        public Thread Thread => m_thread;

        public override void Start() => m_thread.Start();

        public void Start(object parameter) => m_thread.Start(parameter);
    }
}