using System.Reflection.Emit;
using SuperComicLib.Reflection;

namespace SuperComicLib.CodeDesigner
{
    public sealed unsafe class CILCode
    {
        public const int
            CODE_LABEL = 0x10_0000,
            CODE_LOCAL = 0x11_0000,
            CODE_EMIT_ACTION = 0x4000_0000;
        public const int IDX_TEMPLOCAL = 0x1_0000;

        public object operand;
        public int code;
        public int index;

        #region constructors
        public CILCode(OpCode code, object operand)
        {
            this.code = code.Value & 0xFFFF;
            this.operand = operand;
        }

        public CILCode(int index, OpCode code)
        {
            this.code = code.Value & 0xFFFF;
            this.index = index;
        }

        public CILCode(int code, object operand)
        {
            this.code = code;
            this.operand = operand;
        }

        public CILCode(int index, int code)
        {
            this.code = code;
            this.index = index;
        }

        public  CILCode(OpCode code, object operand, int index)
        {
            this.code = code.Value & 0xFFFF;
            this.operand = operand;
            this.index = index;
        }

        public CILCode(int code, object operand, int index)
        {
            this.code = code;
            this.operand = operand;
            this.index = index;
        }
        #endregion

        public bool IsCustomCode => code > 0xFFFF;

        public OpCode GetCode() => OpCodeConverter.GetCode(code);

        public void LabelToIndex(Label label) => index = *(int*)&label;

        public Label IndexToLabel() => new PublicLabel(index);
    }
}
