using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    public sealed class LabelManager
    {
        private int m_currentLabels;
        private readonly Stack<int> m_brLabels = new Stack<int>();
        private readonly Stack<int> m_exitLabels = new Stack<int>();

        public int CurrentLabels => m_currentLabels;

        public int CountBrLabels => m_brLabels.Count;

        public int CountExitLabels => m_exitLabels.Count;

        public int PopBrLabel() => m_brLabels.Pop();

        public int PushBrLabel()
        {
            int result = m_currentLabels++;
            m_brLabels.Push(result);
            return result;
        }

        public int PopExitLabel() => m_exitLabels.Pop();

        public int PushExitLabel()
        {
            int result = m_currentLabels++;
            m_exitLabels.Push(result);
            return result;
        }
    }
}
