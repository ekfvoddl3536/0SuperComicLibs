using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.Reflection
{
    public class ILBuffer : IDisposable
    {
        #region statics
        public static readonly OpCode[] size_1_codes = new OpCode[0x100];
        public static readonly OpCode[] size_2_codes = new OpCode[0x100];

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

        #region instance + fields
        public OpCode opCode;
        public object operand;

        internal int offset;
        internal PublicLabel label;
        internal ExceptionBlockInfo blockInfo;

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

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                operand = null;
                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        ~ILBuffer()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(false);
        }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            GC.SuppressFinalize(this);
        }
        #endregion
        #endregion
    }
}
