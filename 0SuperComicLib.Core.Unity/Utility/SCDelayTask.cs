using System.Collections;
using UnityEngine;

namespace SuperComicWorld
{
    public abstract class SCDelayTask : SCTask
    {
        public IEnumerator ToCoroutineTask(CustomYieldInstruction delayOption = null) =>
            new AutoDelayImpl(this, delayOption);

        private sealed class AutoDelayImpl : IEnumerator
        {
            private readonly SCDelayTask inst;
            private readonly CustomYieldInstruction opt;
            private bool delayed;

            public AutoDelayImpl(SCDelayTask inst, CustomYieldInstruction opt)
            {
                this.inst = inst;
                this.opt = opt;
            }

            public bool MoveNext()
            {
                if (delayed)
                {
                    inst.Invoke();
                    return false;
                }

                var opt = this.opt;
                delayed = opt == null || opt.MoveNext();
                return true;
            }

            public void Reset()
            {
                if (opt != null)
                    opt.Reset();

                delayed = false;
            }

            public object Current => opt?.Current;
        }
    }
}
