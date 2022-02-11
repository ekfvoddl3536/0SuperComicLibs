using System;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Threading
{
    using static HybridCPU_NativeMethods;
    public abstract class HybridThreadContext
    {
        protected readonly Delegate m_delegate;
        protected readonly UIntPtr m_mask;

        protected HybridThreadContext(Delegate thread_work, Preference mode)
        {
            m_delegate = thread_work;
            m_mask = GetMask(mode);
        }

        protected void PARAM_RUN(object arg)
        {
            if (m_mask != UIntPtr.Zero)
                SetThreadAffinityMask(GetCurrentThread(), m_mask);

            ((Action<object>)m_delegate).Invoke(arg);
        }

        protected void NORMAL_RUN()
        {
            if (m_mask != UIntPtr.Zero)
                SetThreadAffinityMask(GetCurrentThread(), m_mask);

            ((Action)m_delegate).Invoke();
        }

        public abstract void Start();

        internal static UIntPtr GetMask(Preference mode)
        {
            var c = HybridCPU.ProcessorCount;
            switch (mode)
            {
                case Preference.BigCores:
                    return (UIntPtr)(Mask(c.bigCores) << (c.littleCores * HybridCPU.headLittleCores));

                case Preference.LittleCores:
                    return (UIntPtr)(Mask(c.littleCores) << (c.bigCores * (HybridCPU.headLittleCores ^ 1)));

                default:
                    return (UIntPtr)Mask(Environment.ProcessorCount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Mask(int idx) => (1UL << idx) - 1;

        #region 핵심 static method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCurrentThreadPreference(Preference mode) => SetThreadAffinityMask(GetCurrentThread(), GetMask(mode));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCurrentThreadAffinity(int __absolute__processors_index) =>
            SetThreadAffinityMask(GetCurrentThread(), (UIntPtr)Mask(__absolute__processors_index));

        #region 64+ logical processor
        /// <summary>
        /// [64bit Only] Use in environments with more than 64 logical processors
        /// </summary>
        public static unsafe void SetCurrentThreadPreferenceW64(Preference mode)
        {
            ProcessorCountEx cpu = HybridCPU.ProcessorCount;

            switch (HybridCPU.headLittleCores)
            {
                // littleCore head
                case 1:
                    __SetGroupAffinity(cpu.bigCores, cpu.littleCores, ((uint)mode >> 1) ^ 1, (uint)mode ^ 1);
                    break;

                // bigCore head
                default:
                    __SetGroupAffinity(cpu.littleCores, cpu.bigCores, (uint)mode ^ 1, ((uint)mode >> 1) ^ 1);
                    break;  

                // // bigCore head
                // case (int)Preference.BigCores:
                //     __SetGroupAffinity(cpu.bigCores, cpu.littleCores, 1, 0);
                //     break;
                // 
                // case (int)Preference.LittleCores:
                //     __SetGroupAffinity(cpu.bigCores, cpu.littleCores, 0, 1);
                //     break;
                // 
                // // littleCore head
                // case (int)(Preference.Normal + 3):
                //     __SetGroupAffinity(cpu.littleCores, cpu.bigCores, 1, 1);
                //     break;
                // 
                // case (int)(Preference.BigCores + 3):
                //     __SetGroupAffinity(cpu.littleCores, cpu.bigCores, 0, 1);
                //     break;
                // 
                // case (int)(Preference.LittleCores + 3):
                //     __SetGroupAffinity(cpu.littleCores, cpu.bigCores, 1, 0);
                //     break;
                // 
                // // bigCore head (normal --> default)
                // default:
                //     __SetGroupAffinity(cpu.bigCores, cpu.littleCores, 1, 1);
                //     break;
            }
        }

        /// <summary>
        /// [64bit Only] Use in environments with more than 64 logical processors
        /// </summary>
        public static unsafe void SetCurrentThreadAffinityW64(int __absolute__processors_index)
        {
            int g_cnt = __absolute__processors_index >> 6,
                i = 0;

            IntPtr handle = GetCurrentThread();
            GROUP_AFFINITY ptr_temp;

            while (i < g_cnt)
            {
                ptr_temp = new GROUP_AFFINITY(UIntPtr.Zero, i++);
                SetThreadGroupAffinity(handle, &ptr_temp, null);
            }

            ptr_temp = new GROUP_AFFINITY(
                (UIntPtr)Mask(__absolute__processors_index & 0x3F),
                g_cnt);
            SetThreadGroupAffinity(handle, &ptr_temp, null);

            g_cnt = Environment.ProcessorCount >> 6;
            while (i < g_cnt)
            {
                ptr_temp = new GROUP_AFFINITY(UIntPtr.Zero, i++);
                SetThreadGroupAffinity(handle, &ptr_temp, null);
            }
        }

#pragma warning disable IDE1006 // 명명 스타일
        private static unsafe void __SetGroupAffinity(int hc, int lc, uint hc_sel, uint lc_sel)
        {
            IntPtr handle = GetCurrentThread();
            GROUP_AFFINITY ptr_temp;

            int ptr0_index = 0;

            // low position cores
            int g_idx = lc >> 6;
            while (--g_idx >= 0)
            {
                ptr_temp = new GROUP_AFFINITY((UIntPtr)(ulong.MaxValue * lc_sel), ptr0_index++);
                SetThreadGroupAffinity(handle, &ptr_temp, null);
            }

            // pc 중첩 요구 공간 vs ec 차지 후 잉여 공간
            int _diff = CMath.Min(hc & 0x3F, 64 - (lc & 0x3F));

            // combine
            ptr_temp = new GROUP_AFFINITY(
                (UIntPtr)((Mask(lc) * lc_sel) | ((Mask(_diff) * hc_sel) << lc)),
                ptr0_index++);
            SetThreadGroupAffinity(handle, &ptr_temp, null);

            // high position cores
            g_idx = hc >> 6;
            while (--g_idx >= 0)
            {
                ptr_temp = new GROUP_AFFINITY((UIntPtr)(ulong.MaxValue * hc_sel), ptr0_index++);
                SetThreadGroupAffinity(handle, &ptr_temp, null);
            }

            // 상위에 남은 공간이 존재
            if ((hc & 0x3F) - _diff > 0)
            {
                ptr_temp = new GROUP_AFFINITY(
                    (UIntPtr)(Mask(_diff) * hc_sel),
                    ptr0_index);
                SetThreadGroupAffinity(handle, &ptr_temp, null);
            }
            // NOTE: a[i] = i; 연산에서 a[i]가 우선됨
            // 'load ptr, load idx, load value, set value' 순서
        }
#pragma warning restore IDE1006
        #endregion
        #endregion

        #region 기능 확장 static method
        /// <summary>
        /// 선호 스레드를 지정하여 작업을 수행합니다.
        /// </summary>
        public static void Invoke(Preference mode, Action work)
        {
            IntPtr handle = GetCurrentThread();
            
            UIntPtr prev_mask = SetThreadAffinityMask(handle, GetMask(mode));
            work.Invoke();

            SetThreadAffinityMask(handle, prev_mask);
        }

        /// <summary>
        /// 선호 스레드를 지정하여 작업을 수행합니다.
        /// </summary>
        public static void Invoke<T>(Preference mode, Action<T> work, T arg)
        {
            IntPtr handle = GetCurrentThread();

            UIntPtr prev_mask = SetThreadAffinityMask(handle, GetMask(mode));
            work.Invoke(arg);

            SetThreadAffinityMask(handle, prev_mask);
        }

        /// <summary>
        /// 선호 스레드를 지정하여 작업을 수행합니다.
        /// </summary>
        public static TResult Invoke<TIn, TResult>(Preference mode, Func<TIn, TResult> work, TIn arg)
        {
            IntPtr handle = GetCurrentThread();

            UIntPtr prev_mask = SetThreadAffinityMask(handle, GetMask(mode));
            TResult result = work.Invoke(arg);

            SetThreadAffinityMask(handle, prev_mask);

            return result;
        }
        #endregion
    }
}