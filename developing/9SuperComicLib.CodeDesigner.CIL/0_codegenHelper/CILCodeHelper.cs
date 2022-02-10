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
            // Ldarg Ldarg_S Ldloc Ldloc_S Ldfld Ldsfld
            if ((temp = buffer.code) == 0xFE_09 ||
                temp == 0x0E ||
                temp == 0xFE_0C ||
                temp == 0x11 ||
                temp == 0x7B ||
                temp == 0x7E)
            {
                buffer.code++;
            }
            else if (temp >= 0x02 && temp <= 0x05) // Ldarg_0 ~ Ldarg_3
            {
                buffer.code = 0x0F;
                buffer.state = temp - 2;
            }
            else if (temp >= 0x06 && temp <= 0x09) // Ldloc_0 ~ Ldloc_3
            {
                buffer.code = 0x12;
                buffer.state = temp - 6;
            }
            else if (temp == 0xA3) // Ldelem (struct)
            {
                buffer.code = 0x8F;
            }
            else if (temp >= 0x90 && temp <= 0x99)
            {
                buffer.code = 0x8F;
                buffer.operand = ElementTypes[temp - 0x90];
            }
        }

        public static void LdRef(ListStack<CILCode> list, ref int locals)
        {
            int temp;
            if ((temp = list.Peek().code) == 0x28 || temp == 0x6F) // Call Callvirt
            {
                temp = CILCode.TEMP_INDEX | locals++;

                list.Push(new CILCode(CILCode.CCODE_LOCAL, temp));
                list.Push(new CILCode(OpCodes.Stloc, temp));
                list.Push(new CILCode(OpCodes.Ldloca, temp));
                // buffer.code = 0x7F;
            }
        }
        #endregion
    }
}
