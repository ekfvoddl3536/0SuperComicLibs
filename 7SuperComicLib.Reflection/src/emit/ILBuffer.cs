using System.Reflection.Emit;

namespace SuperComicLib.Reflection
{
    public sealed class ILBuffer : ILBufferBase
    {
        #region instance
        public int offset;
        public ExceptionBlockInfo blockInfo;

        #region constructor
        public ILBuffer()
        {
        }

        public ILBuffer(OpCode code) : base(code)
        {
        }

        public ILBuffer(OpCode code, object operand) : base(code, operand)
        {
        }
        #endregion

        #region method
        protected override void Dispose(bool disposing)
        {
            offset = 0;
            blockInfo = default;
            base.Dispose(disposing);
        }
        #endregion
        #endregion
    }
}
