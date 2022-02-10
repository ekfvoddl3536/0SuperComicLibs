using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Threading
{
    using static HybridCPU_NativeMethods;
    public abstract class HybridThreadContext
    {
        protected readonly Delegate m_delegate;
        protected readonly IntPtr m_mask;

        protected HybridThreadContext(Delegate thread_work, Preference mode)
        {
            m_delegate = thread_work;
            m_mask = GetMask(mode);
        }

        protected void PARAM_RUN(object arg)
        {
            if (m_mask != IntPtr.Zero)
                SetThreadAffinityMask(GetCurrentThread(), m_mask);

            ((Action<object>)m_delegate).Invoke(arg);
        }

        protected void NORMAL_RUN()
        {
            if (m_mask != IntPtr.Zero)
                SetThreadAffinityMask(GetCurrentThread(), m_mask);

            ((Action)m_delegate).Invoke();
        }

        public abstract void Start();

        internal static IntPtr GetMask(Preference mode)
        {
            var c = HybridCPU.ProcessorCount;
            switch (mode)
            {
                case Preference.BigCores:
                    return (IntPtr)(Mask(c.bigCores) << (c.littleCores * HybridCPU.headLittleCores));

                case Preference.LittleCores:
                    return (IntPtr)(Mask(c.littleCores) << (c.bigCores * (HybridCPU.headLittleCores ^ 1)));

                default:
                    return (IntPtr)Mask(Environment.ProcessorCount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Mask(int idx) => (1UL << idx) - 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCurrentThreadPreference(Preference mode) => 
            SetThreadAffinityMask(GetCurrentThread(), GetMask(mode));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCurrentThreadAffinity(int __absolute__processors_index) => 
            SetThreadAffinityMask(GetCurrentThread(), (IntPtr)Mask(__absolute__processors_index));
    }
}