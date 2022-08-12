using SuperComicLib.Core;
using SuperComicLib.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperComicLib.Collections.Concurrent
{
    public abstract class ParallelEnumerate<T> : IDisposable
    {
        public void Open(IEnumerable<T> files) =>
            Open(files, Environment.ProcessorCount);

        public void Open(IEnumerable<T> files, int threadCount)
        {
            System.Diagnostics.Contracts.Contract.Requires(files != null);
            System.Diagnostics.Contracts.Contract.Requires(threadCount > 0 && threadCount <= Environment.ProcessorCount);

            IEnumerator<T> origin = files.GetEnumerator();
            if (origin == null)
                throw new ArgumentNullException($"{nameof(files)}.{nameof(files.GetEnumerator)}() --> null");

            OnBeginEnumerate(threadCount);

            ConcurrentEnumerator<T> vs = new ConcurrentEnumerator<T>(origin);
            Share<VolatileInt32> token = new Share<VolatileInt32>();

            Action[] methods = new Action[threadCount];
            for (int x = 0; x < threadCount; x++)
                methods[x] = new Worker(this, vs, token, x).Run;

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = threadCount }, methods);

            vs.Dispose();

            OnEndEnumerate();
        }

        /// <summary>
        /// Parallel item enumerate
        /// </summary>
        /// <returns>return false => break loop</returns>
        protected abstract bool OnEnumerate(T current, int thread_idx);

        protected virtual void OnBeginEnumerate(int threadCount)
        {
        }

        protected virtual void OnExitThread(int thread_idx)
        {
        }

        protected virtual void OnEndEnumerate()
        {
        }

        #region dispose
        ~ParallelEnumerate()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion

        #region inner class
        private sealed class Worker
        {
            public readonly ParallelEnumerate<T> owner;
            public readonly ConcurrentEnumerator<T> vs;
            public readonly Share<VolatileInt32> tokenSrc;
            public readonly int thread_idx;

            public Worker(
                ParallelEnumerate<T> owner,
                ConcurrentEnumerator<T> vs,
                Share<VolatileInt32> tokenSrc,
                int thread_idx)
            {
                this.owner = owner;
                this.vs = vs;
                this.tokenSrc = tokenSrc;

                this.thread_idx = thread_idx;
            }

            public void Run()
            {
                var _inst = owner;
                var _iter = vs;
                var _idx = thread_idx;
                ref var _tok = ref tokenSrc.Value;

                while (_iter.MoveNext(out T current_) && _tok == 0)
                    if (!_inst.OnEnumerate(current_, _idx))
                    {
                        _tok.Value = 1;
                        break;
                    }

                _inst.OnExitThread(_idx);
            }
        }
        #endregion
    }
}
