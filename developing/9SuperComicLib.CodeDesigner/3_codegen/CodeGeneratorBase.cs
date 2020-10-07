using System;

namespace SuperComicLib.CodeDesigner
{
    public abstract class CodeGeneratorBase : IDisposable
    {
        protected IExceptionHandler m_handler;

        protected CodeGeneratorBase(IExceptionHandler handler) => m_handler = handler;

        public void Generate(INode parsedNode)
        {
            if (parsedNode == null)
                throw new ArgumentNullException(nameof(parsedNode));
            if (parsedNode.ChildCount == 0)
                throw new ArgumentException();

            if (OnPrepare())
                OnGenerate(parsedNode.GetEnumerator());
        }

        public bool TryGenerate(INode parsedNode)
        {
            try
            {
                Generate(parsedNode);
                return true;
            }
#pragma warning disable
            catch
            {
                return false;
            }
#pragma warning restore
        }

        protected abstract void OnGenerate(INodeEnumerator nodeEnumerator);

        protected virtual bool OnPrepare() => true;

        protected virtual void Dispose(bool disposing) => m_handler = null;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
