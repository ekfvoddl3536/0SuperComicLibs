using System;

namespace SuperComicLib.CodeDesigner
{
    public abstract class CodeGeneratorBase : IDisposable
    {
        protected IExceptionHandler m_handler;
        protected INodeEnumerator m_enumerator;

        protected CodeGeneratorBase(IExceptionHandler handler, INode parsedNode)
        {
            m_handler = handler;
            m_enumerator = parsedNode.GetEnumerator();
        }

        /// <summary>
        /// 코드 생성하기
        /// </summary>
        /// <param name="previous_state">이전 상태</param>
        /// <param name="state">interrupt시 반환할 상태</param>
        /// <param name="argument">공유 인자, interrupt시 추가적으로 반환할 값 또는 로더가 일을 수행한 뒤 나온 값 (이전 상태에 따라 결정)</param>
        /// <returns>false는 끝</returns>
        public abstract void Generate(ScriptLoader owner);

        public virtual object Result { get; }

        // 핵심
        //  1. Generate는 Interruptible하다는 점
        //  2. ScriptLoader가 더 많은 일을 할 수 있게되었다는 점 (코드 생성 범위를 벗어난 일을 할 수 있게됨)
        //  3. 예를 들면 include
        //  4. state를 1로 잡고 argument에 string path인자를 넣어준 뒤 true를 반환
        //      4-a. 반환 전 사용되고 있는 모든 값을 필드에 박아서 저장
        //  5. 로더가 해당 스크립트를 로딩한 뒤 받은 type을 argument에 넣고 continue
        //  6. 나머지 작업을 처리한다

        /// <summary>
        /// 해제
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (m_handler != null)
            {
                m_enumerator.Dispose();
                m_enumerator = null;

                m_handler = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
