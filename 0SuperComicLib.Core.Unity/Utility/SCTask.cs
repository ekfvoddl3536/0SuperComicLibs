using System;

namespace SuperComicWorld
{
    public abstract class SCTask
    {
        private EventHandler m_onCompleted;
        private bool m_isCompleted;

        public bool IsComplete => m_isCompleted;

        public void Invoke()
        {
            OnInvoke();
            
            m_isCompleted = true;
            m_onCompleted?.Invoke(this, EventArgs.Empty);
        }

        protected abstract void OnInvoke();

        public event EventHandler OnCompleted
        {
            add => m_onCompleted += value;
            remove => m_onCompleted -= value;
        }
    }
}
