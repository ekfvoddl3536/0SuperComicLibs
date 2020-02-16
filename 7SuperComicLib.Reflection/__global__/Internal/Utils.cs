using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SuperComicLib.Reflection
{
    internal static class Utils
    {
        internal static bool Range(ushort value, int min, int max) => value >= min && value <= max;

        internal static Action ToDelegate(MethodInfo meth) => (Action)Delegate.CreateDelegate(typeof(Action), meth);

        internal static ILBuffer FindILBuffer(IReadOnlyList<ILBuffer> buffers, int offset)
        {
            int lastIdx = buffers.Count - 1;
#if DEBUG
            if (offset < 0 || offset > buffers[lastIdx].offset)
                throw new ArgumentOutOfRangeException(nameof(offset));
#endif
            int min = 0, max = lastIdx;
            while (min <= max)
            {
                int mid = min + (max - min) / 2;
                ILBuffer current = buffers[mid];

                if (current.offset == offset)
                    return current;

                if (offset < current.offset)
                    max = mid - 1;
                else
                    min = mid + 1;
            }
#if DEBUG
            foreach (ILBuffer buf in buffers)
                if (buf.offset == offset)
                {
                    Console.WriteLine("[WARN] 'm_buffers' is not sorted!");
                    System.Diagnostics.Debug.WriteLine("Not Sorted!!");
                    return buf;
                }
            throw new Exception("Missing ILBuffer!");
#else
            return null;
#endif
        }

        public static string IL2String(IReadOnlyList<ILBuffer> buffers)
        {
            StringBuilder strb = new StringBuilder(1024);
            foreach (ILBuffer il in buffers)
            {
                OpCode code = il.opCode;
                strb.Append($"IL_{il.offset.ToString("X4")}: {code.ToString()}");
                switch (code.OperandType)
                {
                    case OperandType.InlineBrTarget:
                    case OperandType.ShortInlineBrTarget:
                        strb.Append($" IL_{FindILBuffer(buffers, (int)il.operand).offset.ToString("X4")}");
                        break;

                    case OperandType.InlineField:
                        {
                            FieldInfo info = (FieldInfo)il.operand;
                            strb.Append($" {info.FieldType.Name.ToLower()} {info.DeclaringType.Name}::{info.Name}");
                        }
                        break;

                    case OperandType.InlineMethod:
                        {
                            if (il.operand is MethodInfo temp)
                            {
                                if (temp.IsStatic == false)
                                    strb.Append($" instance");
                                strb.Append($" {temp.ReturnType.Name.ToLower()} {temp.DeclaringType.Name}::{temp.Name}()");
                            }
                            else
                            {
                                ConstructorInfo info = (ConstructorInfo)il.operand;
                                if (info.IsStatic == false)
                                    strb.Append($" instance");
                                strb.Append($" void {info.DeclaringType.Name}::.ctor()");
                            }
                        }
                        break;

                    case OperandType.InlineSig:
                        {
                            if (il.operand is SignatureHelper temp)
                                strb.Append($" {temp.ToString()}");
                            else
                                strb.Append($" meta_{((int)il.operand).ToString("X4")}");
                        }
                        break;

                    case OperandType.InlineString:
                        strb.Append($" \"{(string)il.operand}\"");
                        break;

                    case OperandType.InlineSwitch:
                        strb.Append($" <inlineSwitch>");
                        break;

                    case OperandType.InlineTok:
                        {
                            if (il.operand is Type temp)
                                strb.Append($" {temp.FullName}");
                            else
                                strb.Append($" 0x{((int)il.operand).ToString("X4")}");
                        }
                        break;

                    case OperandType.InlineType:
                        strb.Append($" {((Type)il.operand).FullName}");
                        break;

                    case OperandType.InlineI:
                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                    case OperandType.InlineVar:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineR:
                    case OperandType.ShortInlineVar:
                        strb.Append($" {il.operand.ToString()}");
                        break;

                    default:
                        break;
                }
                strb.AppendLine();
            }
            return strb.ToString();
        }
    }
}
