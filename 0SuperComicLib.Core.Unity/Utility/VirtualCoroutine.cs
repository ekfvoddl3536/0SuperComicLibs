using System;
using System.Collections;

namespace SuperComicWorld
{
    public class VirtualCoroutine : IEnumerator
    {
        private readonly IEnumerator _baseCoroutine;
        private EventHandler _prefix;
        private EventHandler _postfix;
        private int _stateCode;

        public VirtualCoroutine(IEnumerator baseRoutine) =>
            _baseCoroutine = baseRoutine;

        public event EventHandler Prefix
        {
            add => _prefix += value;
            remove => _prefix -= value;
        }

        public event EventHandler Postfix
        {
            add => _postfix += value;
            remove => _postfix -= value;
        }

        bool IEnumerator.MoveNext()
        {
            if (_stateCode == 0)
            {
                _prefix?.Invoke(this, EventArgs.Empty);
                _stateCode = 1;
            }

            if (_baseCoroutine.MoveNext())
                return true;

            _postfix.Invoke(this, EventArgs.Empty);
            return false;
        }

        void IEnumerator.Reset()
        {
            _stateCode = 0;
            _baseCoroutine.Reset();
        }

        object IEnumerator.Current => _baseCoroutine.Current;

        ~VirtualCoroutine()
        {
            if (_baseCoroutine is IDisposable d)
                d.Dispose();
        }
    }
}
