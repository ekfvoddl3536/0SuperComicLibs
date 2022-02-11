namespace SuperComicLib.Collections
{
    internal sealed class RangeArrayStream<T> : ArrayStream<T>
    {
        private int m_st;
        private int m_ed;

        internal RangeArrayStream(T[] src, int begin, int end)
        {
            arr = src;
            len = src.Length;
            idx = begin;
            m_st = begin;
            m_ed = end;
        }

        public override bool EndOfStream => idx < m_st || idx >= m_ed;

        public override int Length => m_ed - m_st;

        public override void Reset() => idx = m_st;

        protected override void Dispose(bool disposing)
        {
            m_st = 0;
            m_ed = 0;
            base.Dispose(disposing);
        }
    }
}
