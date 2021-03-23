using System.Reflection.Emit;
using SuperComicLib.Reflection;

namespace SuperComicLib.CodeDesigner
{
    public sealed unsafe class CILCode
    {
        public const int
            CCODE_LABEL = 0x10_0000,
            CCODE_LOCAL = 0x11_0000,
            CCODE_LOAD_FIELD = 0x12_0000;
        public const int TEMP_INDEX = 0x1_0000;

        public object operand;
        public int code;
        public int state;

        #region constructors
        public CILCode(int code, object operand)
        {
            this.code = code;
            this.operand = operand;
        }

        public CILCode(int code, int state)
        {
            this.code = code;
            this.state = state;
        }

        public CILCode(int code, int state, object operand)
        {
            this.code = code;
            this.state = state;
            this.operand = operand;
        }

        public CILCode(OpCode code, object operand) : this((int)(uint)code.Value, operand)
        {
        }

        public CILCode(OpCode code, int state) : this((int)(uint)code.Value, state)
        {
        }

        public  CILCode(OpCode code, int state, object operand) : this((int)(uint)code.Value, state, operand)
        {
        }
        #endregion

        public bool IsCustomCode => (uint)code > 0xFFFF;

        public OpCode Value => OpCodeConverter.GetCode(code);

        public void LabelToIndex(Label label) => state = *(int*)&label;

        public Label IndexToLabel()
        {
            long value = 0x1_0000_0000 | (uint)state;
            return *(Label*)&value;
        }
    }
}
