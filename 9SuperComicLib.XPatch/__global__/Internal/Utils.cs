using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using SuperComicLib.Runtime;

namespace SuperComicLib.XPatch
{
    internal static unsafe class Utils
    {
        internal static bool Range(ushort value, int min, int max) =>
            value >= min && value <= max;

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
            IEnumerator<ILBuffer> iter = buffers.GetEnumerator();
            while (iter.MoveNext())
            {
                ILBuffer buf = iter.Current;
                if (buf.offset == offset)
                {
                    Console.WriteLine("[WARN] 'm_buffers' is not sorted!");
                    System.Diagnostics.Debug.WriteLine("Not Sorted!!");

                    iter.Dispose();
                    return buf;
                }
            }

            iter.Dispose();
            System.Diagnostics.Debug.Fail("Missing ILBuffer");
#endif
            return null;
        }

        public static string IL2String(IReadOnlyList<ILBuffer> buffers)
        {
            StringBuilder strb = new StringBuilder(1024);

            IEnumerator<ILBuffer> iter = buffers.GetEnumerator();
            while (iter.MoveNext())
            {
                ILBuffer il = iter.Current;

                OpCode code = il.code;
                strb.Append($"IL_{il.offset:X4}: {code}");
                switch (code.OperandType)
                {
                    case OperandType.InlineBrTarget:
                    case OperandType.ShortInlineBrTarget:
                        strb.Append($" IL_{FindILBuffer(buffers, (int)il.operand).offset:X4}");
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
                            strb.Append($" {temp}");
                        else
                            strb.Append($" meta_{(int)il.operand:X4}");
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
                            strb.Append($" 0x{(int)il.operand:X4}");
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
                        strb.Append($" {il.operand}");
                        break;

                    default:
                        break;
                }
                strb.AppendLine();
            }
            iter.Dispose();

            return strb.ToString();
        }
    }
}
