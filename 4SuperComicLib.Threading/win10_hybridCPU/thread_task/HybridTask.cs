using System;
using System.Threading;
using System.Threading.Tasks;

namespace SuperComicLib.Threading
{
    public sealed class HybridTask : HybridThreadContext, IDisposable
    {
        private Task m_task;

        #region action constructor
        public HybridTask(Action action, Preference mode) : base(action, mode) =>
            m_task = new Task(NORMAL_RUN);
        public HybridTask(Action action, CancellationToken cancellationToken, Preference mode) : base(action, mode) =>
            m_task = new Task(NORMAL_RUN, cancellationToken);
        public HybridTask(Action action, TaskCreationOptions creationOptions, Preference mode) : base(action, mode) =>
            m_task = new Task(NORMAL_RUN, creationOptions);
        public HybridTask(
            Action action,
            CancellationToken cancellationToken,
            TaskCreationOptions creationOptions,
            Preference mode)
            : base(action, mode)
        {
            m_task = new Task(NORMAL_RUN, cancellationToken, creationOptions);
        }
        #endregion

        #region action<obj> constructor
        public HybridTask(Action<object> action, object state, Preference mode) : base(action, mode) =>
            m_task = new Task(PARAM_RUN, state);
        public HybridTask(Action<object> action, object state, CancellationToken cancellationToken, Preference mode) : base(action, mode) =>
            m_task = new Task(PARAM_RUN, state, cancellationToken);
        public HybridTask(Action<object> action, object state, TaskCreationOptions creationOptions, Preference mode) : base(action, mode) =>
            m_task = new Task(PARAM_RUN, state, creationOptions);
        public HybridTask(
            Action<object> action,
            object state,
            CancellationToken cancellationToken,
            TaskCreationOptions creationOptions,
            Preference mode)
            : base(action, mode)
        {
            m_task = new Task(PARAM_RUN, state, cancellationToken, creationOptions);
        }
        #endregion

        #region method & properties
        public Task Task => m_task;

        public override void Start() => m_task.Start();
        #endregion

        #region disposing
        ~HybridTask()
        {
            m_task.Dispose();
        }

        public void Dispose()
        {
            m_task.Dispose();
            m_task = null;

            GC.SuppressFinalize(this);
        }
        #endregion

        #region static
        public static HybridTask StartNew(Action action, Preference mode) => StartNew(action, default, 0, mode);

        public static HybridTask StartNew(Action action, CancellationToken cancellationToken, Preference mode) => 
            StartNew(action, cancellationToken, 0, mode);

        public static HybridTask StartNew(Action action, TaskCreationOptions creationOptions, Preference mode) =>
            StartNew(action, default, creationOptions, mode);

        public static HybridTask StartNew(
            Action action, 
            CancellationToken cancellationToken, 
            TaskCreationOptions creationOptions,
            Preference mode)
        {
            var result = new HybridTask(action, cancellationToken, creationOptions, mode);
            result.Start();
            return result;
        }

        public static HybridTask StartNew(Action<object> action, object state, Preference mode) => StartNew(action, state, default, 0, mode);

        public static HybridTask StartNew(Action<object> action, object state, CancellationToken cancellationToken, Preference mode) =>
            StartNew(action, state, cancellationToken, 0, mode);

        public static HybridTask StartNew(Action<object> action, object state, TaskCreationOptions creationOptions, Preference mode) =>
            StartNew(action, state, default, creationOptions, mode);

        public static HybridTask StartNew(
            Action<object> action,
            object state,
            CancellationToken cancellationToken,
            TaskCreationOptions creationOptions,
            Preference mode)
        {
            var result = new HybridTask(action, state, cancellationToken, creationOptions, mode);
            result.Start();
            return result;
        }
        #endregion
    }
}