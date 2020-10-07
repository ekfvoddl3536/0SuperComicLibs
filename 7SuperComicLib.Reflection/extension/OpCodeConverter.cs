using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.Reflection
{
    public static class OpCodeConverter
    {
        #region statics
        private static readonly Dictionary<int, OpCode> size_1_codes = new Dictionary<int, OpCode>();
        private static readonly Dictionary<int, OpCode> size_2_codes = new Dictionary<int, OpCode>();

        static OpCodeConverter()
        {
            FieldInfo[] vs = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
            int len = vs.Length;

            while (--len >= 0)
            {
                OpCode code = (OpCode)vs[len].GetValue(null);
                int first = code.Value & 0xFF_00;
                if (first == 0xFE_00)
                    size_2_codes[code.Value & 0xFF] = code;
                else
                    size_1_codes[code.Value] = code;
            }
        }

        public static OpCode GetCode_sz1(int value) => size_1_codes[value];

        public static OpCode GetCode_sz2(int value) => size_2_codes[value];

        public static OpCode GetCode(int value)
        {
            int first = value & 0xFF_00;
            return
                first == 0xFE_00
                ? size_2_codes[value & 0xFF]
                : size_1_codes[value];
        }
        #endregion
    }
}
