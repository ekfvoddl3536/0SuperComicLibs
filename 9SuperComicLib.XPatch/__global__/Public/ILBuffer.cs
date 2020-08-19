using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.XPatch
{
    public sealed class ILBuffer : IDisposable
    {
        #region statics
        public static readonly Dictionary<int, OpCode> size_1_codes = new Dictionary<int, OpCode>();
        public static readonly Dictionary<int, OpCode> size_2_codes = new Dictionary<int, OpCode>();

        static ILBuffer()
        {
            foreach (FieldInfo fd in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OpCode code = (OpCode)fd.GetValue(null);
                int first = code.Value & 0xFF_00;
                if (first == 0xFE_00)
                    size_2_codes[code.Value & 0xFF] = code;
                else
                    size_1_codes[code.Value] = code;
            }
        }

        public static OpCode Find(ushort value)
        {
            int first = value & 0xFF_00;
            return
                first == 0xFE_00
                ? size_2_codes[value & 0xFF]
                : size_1_codes[value];
        }
        #endregion

        #region instance
        public OpCode opCode;
        public object operand;
        public int offset;
        public PublicLabel label;
        public ExceptionBlockInfo blockInfo;

        #region constructor
        public ILBuffer() { }

        public ILBuffer(OpCode opCode) => this.opCode = opCode;

        public ILBuffer(OpCode opCode, object operand)
        {
            this.opCode = opCode;
            this.operand = operand;
        }
        #endregion

        #region method
        public int ILSize()
        {
            int codeSize = opCode.Size;
            OperandType type = opCode.OperandType;
            return
                type == OperandType.InlineNone // 피연산자 없음
                ? codeSize
                : type == OperandType.InlineSwitch
                ? codeSize + (1 + ((int[])operand).Length) * 4
                : type == OperandType.InlineI8 || type == OperandType.InlineR
                ? codeSize + sizeof(long)
                : type == OperandType.InlineVar
                ? codeSize + sizeof(ushort)
                : type == OperandType.ShortInlineR // 얘 혼자 17번이고 4바이트인데, 15번부터 위로 전부 1바이트임
                ? codeSize + sizeof(float)
                : type >= OperandType.ShortInlineBrTarget
                ? codeSize + sizeof(byte)
                : codeSize + sizeof(uint); // 대부분의 피연산자는 4바이트이다
        }

        public void Dispose()
        {
            opCode = default;
            label = default;
            blockInfo = default;
            offset = 0;
            operand = null;

            GC.SuppressFinalize(this);
        }
        #endregion
        #endregion
    }
}
