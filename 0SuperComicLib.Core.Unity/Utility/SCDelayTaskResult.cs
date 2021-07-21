using System;

namespace SuperComicWorld
{
    public abstract class SCDelayTask<TResult> : SCDelayTask
    {
        private TResult m_result;

        public TResult Result
        {
            get
            {
                if (IsComplete == false)
                    throw new InvalidOperationException("Task not completed.");

                return m_result;
            }
        }

        protected override sealed void OnInvoke() => m_result = OnInvokeResult();

        protected abstract TResult OnInvokeResult();
    }
}
