using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.Reflection
{
    using static Utils;
    // Refer to https://www.codeproject.com/Articles/14058/Parsing-the-IL-of-a-Method-Body
    public unsafe class MethodBodyEditor : IDisposable
    {
        #region 상수 및 미리정의
        private const ExceptionBlockType
            endBlock = ExceptionBlockType.End | ExceptionBlockType.BigBlock,
            beginBlock = ExceptionBlockType.Begin | ExceptionBlockType.BigBlock,
            beginFilter = ExceptionBlockType.Begin | ExceptionBlockType.FilterBlock,
            beginFinally = ExceptionBlockType.Begin | ExceptionBlockType.FinallyBlock,
            beginCatch = ExceptionBlockType.Begin | ExceptionBlockType.CatchBlock,
            beginFault = ExceptionBlockType.Begin | ExceptionBlockType.FaultBlock,
            flagValue = (ExceptionBlockType)0x3F; // end와 begin을 제외한 모든 것

        #endregion

        protected IReadOnlyList<ILBuffer> m_buffers;
        protected IList<ExceptionHandlingClause> exceptions;
        protected IList<LocalVariableInfo> localVars;

        internal MethodBodyEditor(IReadOnlyList<ILBuffer> buffers, IList<ExceptionHandlingClause> _blocks, IList<LocalVariableInfo> _locals)
        {
            m_buffers = buffers;
            exceptions = _blocks;
            localVars = _locals;
        }

        #region emit il prepare
        public virtual void WriteIL(ILGenerator il, int argFixOffset, bool hasReturn)
        {
#if DEBUG
            string before = IL2String(m_buffers);
#endif
            // step 1: declare local
            foreach (LocalVariableInfo loc in localVars)
                il.DeclareLocal(loc.LocalType, loc.IsPinned);

            // exception blocks
            foreach (ExceptionHandlingClause ex in exceptions)
            {
                FindILBuffer(m_buffers, ex.TryOffset)
                    .blockInfo = new ExceptionBlockInfo(beginBlock);
                FindILBuffer(m_buffers, ex.TryLength + ex.HandlerLength - 1)
                    .blockInfo = new ExceptionBlockInfo(endBlock);

                switch (ex.Flags)
                {
                    case ExceptionHandlingClauseOptions.Filter:
                        FindILBuffer(m_buffers, ex.FilterOffset)
                            .blockInfo = new ExceptionBlockInfo(beginFilter);
                        break;
                    case ExceptionHandlingClauseOptions.Finally:
                        FindILBuffer(m_buffers, ex.HandlerOffset)
                            .blockInfo = new ExceptionBlockInfo(beginFinally);
                        break;
                    case ExceptionHandlingClauseOptions.Clause:
                        FindILBuffer(m_buffers, ex.HandlerOffset)
                            .blockInfo = new ExceptionBlockInfo(beginCatch, ex.CatchType);
                        break;
                    case ExceptionHandlingClauseOptions.Fault:
                        FindILBuffer(m_buffers, ex.HandlerOffset)
                            .blockInfo = new ExceptionBlockInfo(beginFault);
                        break;
                    default:
                        break;
                }
            }

            List<ILBuffer> final_code = new List<ILBuffer>();

            Label retlabel = new Label();
            bool need_defineRetLabel = true;

            // step 2: create final IL code
            ForEach(il, buf => ModifyDefault(il, buf, final_code, ref retlabel, ref need_defineRetLabel, hasReturn, argFixOffset));

#if DEBUG
            string after = IL2String(final_code);

            Console.WriteLine(" === Before ===");
            Console.WriteLine(before);
            Console.WriteLine(Environment.NewLine);

            Console.WriteLine(" === After ===");
            Console.WriteLine(after);
            Console.WriteLine(Environment.NewLine);

            System.Diagnostics.Debug.WriteLine("Break!");
#endif

            // step 3: emit IL code
            EmitCode(il, final_code);
            if (need_defineRetLabel == false)
                il.MarkLabel(retlabel);
        }

        protected virtual void ModifyDefault(ILGenerator il, ILBuffer current, List<ILBuffer> final_code, ref Label retlabel, ref bool need_defineRetLabel, bool hasReturn, int argFixOffset)
        {
            // ret -> jump
            ref OpCode code = ref current.opCode;
            if (code == OpCodes.Ret)
            {
                if (hasReturn)
                {
                    code = OpCodes.Stloc_0;
                    final_code.Add(current);
                }

                // define label
                if (need_defineRetLabel)
                {
                    retlabel = il.DefineLabel();
                    need_defineRetLabel = false;
                }

                // use label
                final_code.Add(new ILBuffer
                {
                    opCode = OpCodes.Br,
                    operand = retlabel
                });
            }
            // add
            else
            {
                FixupILCode(current, code, argFixOffset, hasReturn);
                final_code.Add(current);
            }
        }

        protected virtual void FixupILCode(ILBuffer current, OpCode qcode, int argFixOffset, bool hasReturn)
        {
            // ldarg.s -> stloc.s
            // 0xE ~ 0x13
            // + FDFB = long version (remove 's')
            WORD v = qcode.Value;
            if (Range(v, 0xE, 0x13))
            {
                v += 0xFD_FB;
                // fix operand size
                current.operand = (short)(byte)current.operand;
            }

            // (only return buffer, instance)
            if (argFixOffset == 2)
            {
                // ldarg_3
                if (v == 5)
                {
                    v = 0xFE_09; // ldarg
                    current.operand = (short)4;
                }
                // ldarg_1 ldarg_2
                else if (v == 3 || v == 4)
                    v++;
                // ldarg ldarga starg
                else if (Range(v, 0xFE_09, 0xFE_0B))
                    current.operand = (short)((short)current.operand + 1);
            }
            if (hasReturn)
            {
                // ldloc_3
                if (v == 9)
                {
                    v = 0xFE_0C; // ldloc
                    current.operand = (short)4;
                }
                // stloc_3
                else if (v == 0xD)
                {
                    v = 0xFE_0E;
                    current.operand = (short)4;
                }
                // ldloc_0 ldloc_1 ldloc_2 stloc_0 stloc_1 stloc_2
                else if (Range(v, 6, 8) || Range(v, 0xA, 0xD))
                    v++;
                // ldloc ldloca stloc
                else if (Range(v, 0xFE_0C, 0xFE_0E))
                    current.operand = (short)((short)current.operand + 1);
            }

            // br.s -> blt.un.s
            if (Range(v, 0x2B, 0x37))
                v += 0xD;
            else if (v == 0xDE) // leave.s
                v = 0xDD;

            if (v != qcode.Value)
                current.opCode = ILBuffer.Find(v);
        }

        protected void ForEach(ILGenerator il, Action<ILBuffer> action)
        {
            for (int x = 0, max = m_buffers.Count - 1; x < max; x++)
            {
                ILBuffer buf = m_buffers[x];
                switch (buf.opCode.OperandType)
                {
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        FindILBuffer(m_buffers, (int)buf.operand).label = il.DefineLabel();
                        break;

                    case OperandType.InlineSwitch:
                        foreach (int i in (int[])buf.operand)
                            FindILBuffer(m_buffers, i).label = il.DefineLabel();
                        break;

                    default:
                        break;
                }

                action.Invoke(buf);
            }
        }
        #endregion

        #region emit il code
#pragma warning disable
        protected virtual void EmitCode(ILGenerator il, IReadOnlyList<ILBuffer> codes)
        {
            foreach (ILBuffer current in codes)
            {
                OpCode code = current.opCode;
                PublicLabel label = current.label;
                ExceptionBlockInfo eb = current.blockInfo;

                if (label.valid)
                    il.MarkLabel((Label)label);

                if (eb.IsValid)
                    MarkExceptionBlock(il, eb);

                switch (code.OperandType)
                {
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        il.Emit(code, (Label)FindILBuffer(codes, (int)current.operand).label);
                        break;

                    case OperandType.InlineField:
                        il.Emit(code, (FieldInfo)current.operand);
                        break;

                    case OperandType.InlineMethod:
                        {
                            if (current.operand is MethodInfo temp)
                                il.Emit(code, temp);
                            else
                                il.Emit(code, (ConstructorInfo)current.operand);
                        }
                        break;

                    case OperandType.InlineType:
                        il.Emit(code, (Type)current.operand);
                        break;

                    case OperandType.InlineI:
                        il.Emit(code, (int)current.operand);
                        break;

                    case OperandType.InlineI8:
                        il.Emit(code, (long)current.operand);
                        break;

                    case OperandType.InlineR:
                        il.Emit(code, (double)current.operand);
                        break;

                    case OperandType.InlineSig:
                        {
                            if (current.operand is SignatureHelper temp)
                                il.Emit(code, temp);
                            else
                                il.Emit(code, (int)current.operand);
                        }
                        break;

                    case OperandType.InlineString:
                        il.Emit(code, (string)current.operand);
                        break;

                    case OperandType.InlineSwitch:
                        {
                            foreach (int temp in (int[])current.operand)
                                il.Emit(code, (Label)FindILBuffer(codes, temp).label);
                        }
                        break;

                    case OperandType.InlineTok:
                        {
                            if (current.operand is Type temp)
                                il.Emit(code, temp);
                            else
                                il.Emit(code, (int)current.operand);
                        }
                        break;

                    case OperandType.InlineVar:
                        il.Emit(code, (short)current.operand);
                        break;

                    case OperandType.ShortInlineI:
                        il.Emit(code, (byte)current.operand);
                        break;

                    case OperandType.ShortInlineR:
                        il.Emit(code, (float)current.operand);
                        break;

                    case OperandType.ShortInlineVar:
                        il.Emit(code, (byte)current.operand);
                        break;

                    default: // no operand
                        il.Emit(code);
                        break;
                }
            }
        }
#pragma warning restore

        protected virtual void MarkExceptionBlock(ILGenerator il, ExceptionBlockInfo info)
        {
            ExceptionBlockType btype = info.blockType;
            if ((btype & ExceptionBlockType.End) != 0)
                il.EndExceptionBlock();
            else // begin
                switch (btype & flagValue)
                {
                    case ExceptionBlockType.BigBlock:
                        il.BeginExceptionBlock();
                        return;
                    case ExceptionBlockType.FilterBlock:
                        il.BeginExceptFilterBlock();
                        return;
                    case ExceptionBlockType.FinallyBlock:
                        il.BeginFinallyBlock();
                        return;
                    case ExceptionBlockType.CatchBlock:
                        il.BeginCatchBlock(info.catchType);
                        return;
                    case ExceptionBlockType.FaultBlock:
                        il.BeginFaultBlock();
                        return;
                    default:
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"[ERROR] {btype.ToString()}");
#endif
                        return;
                }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                m_buffers = null;

                exceptions = null;
                localVars = null;

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        ~MethodBodyEditor()
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
    }
}