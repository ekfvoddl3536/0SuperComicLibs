using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SuperComicLib.Threading
{
    public sealed class BatchLoopState
    {
        public const int STATE_BREAK = 1, STATE_STOP = 2;

		internal volatile int m_state;
        internal VolatileInt32 m_lowestBreak;

		public void Stop()
        {
			if (Interlocked.CompareExchange(ref m_state, STATE_STOP, 0) == STATE_BREAK)
				throw new InvalidOperationException("called 'Break' already.");
        }

		public void Break()
        {
			if (Interlocked.CompareExchange(ref m_state, STATE_BREAK, 0) == STATE_STOP)
				throw new InvalidOperationException("called 'Stop' already.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool NeedBreak(int index) =>
            m_state == STATE_STOP ||
            m_state == STATE_BREAK && index >= m_lowestBreak.Value;
    }
}