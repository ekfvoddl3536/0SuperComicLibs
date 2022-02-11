using System;
using System.Threading.Tasks;

namespace SuperComicLib.Threading
{
    public static class HybridParallel
    {
        public static void For(int fromInclusive, int toExclusive, Preference mode, Action<int> body)
        {
            int num_workers = HybridCPU.EffectiveProcessorCount(mode);

            uint count = (uint)(toExclusive - fromInclusive);
            uint rng_sz = count / (uint)num_workers;

            uint left_ = count - rng_sz * (uint)num_workers;
            Action[] workers = new Action[num_workers];

            for (int i = 0; i < num_workers;)
            {
                uint temp = CMath.Normal(left_); // 0 or 1
                left_ -= temp;
                temp += rng_sz;

                workers[i++] = new RangeWorker(body, fromInclusive, (int)temp, mode).Run;
                fromInclusive += (int)temp;
            }

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = num_workers }, workers);
        }

        private sealed class RangeWorker
        {
            public readonly Action<int> m_body;
            public readonly int m_fromInclusive;
            public readonly int m_count;
            public readonly Preference m_mode;

            public RangeWorker(Action<int> body, int fromInclusive, int count, Preference mode)
            {
                m_body = body;
                m_fromInclusive = fromInclusive;
                m_count = count;
                m_mode = mode;
            }

            public void Run()
            {
                HybridThreadContext.SetCurrentThreadPreference(m_mode);

                Action<int> body_ = m_body;
                for (int si_ = m_fromInclusive, cnt = m_count; --cnt >= 0;)
                    body_.Invoke(si_++);
            }
        }
    }
}
