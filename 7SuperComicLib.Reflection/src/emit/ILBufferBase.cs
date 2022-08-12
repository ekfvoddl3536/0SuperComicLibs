using System;
using System.Reflection.Emit;

namespace SuperComicLib.Runtime
{
    public class ILBufferBase : IDisposable
    {
        public PublicLabel label;
        public OpCode code;
        public object operand;

        public ILBufferBase() { }

        public ILBufferBase(OpCode code) => this.code = code;

        public ILBufferBase(OpCode code, object operand) : this(code) =>
            this.operand = operand;

        public int ILSize()
        {
            int codeSize = code.Size;
            OperandType type = code.OperandType;
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            label = default;
            code = default;
            operand = null;
        }
    }
}
