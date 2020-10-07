using System;
using System.Reflection.Emit;
using SuperComicLib.Collections;
using SuperComicLib.Core;

namespace SuperComicLib.CodeDesigner
{
    // last type
    // tempLocals
    public sealed class CILCodeHelper
    {
        #region static
        private static readonly Type[] ElementTypes = 
        { 
            CTypes.SBYTE_T,  // 90
            CTypes.BYTE_T,  // 91
            CTypes.SHORT_T,  // 92
            UCTypes.SHORT_T,  // 93
            CTypes.INT_T,// 94
            UCTypes.INT_T, // 95
            CTypes.LONG_T, // 96
            CTypes.INTPTR_T, // 97 (native int)
            CTypes.FLOAT_T, // 98
            CTypes.DOUBLE_T, // 99
        };

        public static void LdAddress(CILCode buffer)
        {
            int temp;
            if ((temp = buffer.code) == 0xFE_09) // Ldarg
                buffer.code = 0xFE_0A;
            else if (temp == 0x0E) // Ldarg_S
                buffer.code = 0x0F;
            else if (temp >= 0x02 && temp <= 0x05) // Ldarg_0 ~ Ldarg_3
            {
                buffer.code = 0x0F;
                buffer.index = temp - 2;
            }
            else if (temp == 0xFE_0C) // Ldloc
                buffer.code = 0xFE_0D;
            else if (temp == 0x11) // Ldloc_S
                buffer.code = 0x12;
            else if (temp >= 0x06 && temp <= 0x09) // Ldloc_0 ~ Ldloc_3
            {
                buffer.code = 0x12;
                buffer.index = temp - 6;
            }
            else if (temp == 0xA3) // Ldelem (struct)
                buffer.code = 0x8F;
            else if (temp >= 0x90 && temp <= 0x99)
            {
                buffer.code = 0x8F;
                buffer.operand = ElementTypes[temp - 0x90];
            }
        }

        public static void LdRef(ListStack<CILCode> list, ref int locals)
        {
            CILCode buffer = list.Peek();

            int temp;
            if ((temp = buffer.code) == 0x7B) // Ldfld
                buffer.code = 0x7C;
            else if (temp == 0x7E || temp == 0x28 || temp == 0x6F) // Ldsfld Call Callvirt
            {
                temp = CILCode.IDX_TEMPLOCAL | locals++;

                list.Push(new CILCode(temp, CILCode.CODE_LOCAL));
                list.Push(new CILCode(temp, OpCodes.Stloc));
                list.Push(new CILCode(temp, OpCodes.Ldloca));
                // buffer.code = 0x7F;
            }
        }
        #endregion
    }
}
