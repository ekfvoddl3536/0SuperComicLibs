using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.Runtime
{
    public static class OpCodeConverter
    {
        #region statics
        private static readonly Dictionary<int, OpCode> opCodes = new Dictionary<int, OpCode>();

        static OpCodeConverter()
        {
            FieldInfo[] vs = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
            int len = vs.Length;

            while (--len >= 0)
            {
                OpCode code = (OpCode)vs[len].GetValue(null);
                opCodes[(ushort)code.Value] = code;
            }
        }

        public static OpCode GetCode_sz1(int value) => opCodes[value & 0xFF];

        public static OpCode GetCode_sz2(int value) => opCodes[0xFE_00 | value];

        public static OpCode GetCode(int value) => opCodes[value];
        #endregion
    }
}
