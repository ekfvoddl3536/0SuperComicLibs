using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.XPatch
{
    public unsafe class MethodBodyReader : IDisposable
    {
        protected List<ILBuffer> m_buffers = new List<ILBuffer>();
        protected MethodBase method;
        protected Module module;

        public MethodBodyReader(MethodBase method) : this(method, (Module)null, false) { }

        public MethodBodyReader(MethodBase method, Type owner, bool isDynamicMethod) : this(method, owner.Module, isDynamicMethod) { }

        public MethodBodyReader(MethodBase method, Module module, bool isDynamicMethod)
        {
            if (isDynamicMethod)
                this.method = new RTDynamicMethodInfo(method.GetType().GetField("m_owner", Helper.flag0).GetValue(method) as DynamicMethod);
            else
                this.method = method;
            this.module = module;
            ParseIL();
        }

        public MethodBodyReader() { }

        protected static T Read<T>(byte* ptr, ref int pos) where T : unmanaged
        {
            T result = *(T*)(ptr + pos);
            pos += sizeof(T);
            return result;
        }

        protected virtual void ParseIL()
        {
            Module mod = module ?? method.Module;

            byte[] il = method.GetMethodBody().GetILAsByteArray();
            fixed (byte* ptr = il)
                for (int pos = 0, max = il.Length; pos < max;)
                {
                    ILBuffer buf = new ILBuffer
                    {
                        offset = pos
                    };

                    OpCode code;
                    byte temp = Read<byte>(ptr, ref pos);
                    if (temp == 0xFE)
                        code = ILBuffer.size_2_codes[Read<byte>(ptr, ref pos)];
                    else
                        code = ILBuffer.size_1_codes[temp];

                    buf.opCode = code;

                    switch (code.OperandType)
                    {
                        case OperandType.InlineBrTarget:
                            buf.operand = Read<int>(ptr, ref pos) + pos;
                            break;

                        case OperandType.InlineField:
                            buf.operand = mod.ResolveField(Read<int>(ptr, ref pos));
                            break;

                        case OperandType.InlineMethod:
                            {
                                int token = Read<int>(ptr, ref pos);
                                try
                                {
                                    buf.operand = mod.ResolveMethod(token);
                                }
#pragma warning disable CA1031 // Do not catch general exception types
                                catch
                                {
                                    buf.operand = mod.ResolveMember(token);
                                }
#pragma warning restore CA1031
                            }
                            break;

                        case OperandType.InlineSig:
                            // 사용하지 않음
                            // buf.operand = mod.ResolveSignature(Read<int>(ptr, ref pos));
                            buf.operand = Read<int>(ptr, ref pos);
                            break;

                        case OperandType.InlineTok:
                            {
                                int token = Read<int>(ptr, ref pos);
                                try
                                {
                                    buf.operand = mod.ResolveType(token);
                                }
#pragma warning disable CA1031 // Do not catch general exception types
                                catch
                                {
                                    buf.operand = token;
                                }
#pragma warning restore CA1031
                            }
                            break;

                        case OperandType.InlineType:
                            buf.operand = mod.ResolveType(Read<int>(ptr, ref pos), method.DeclaringType?.GetGenericArguments(), method.GetGenericArguments());
                            break;

                        case OperandType.InlineI:
                            buf.operand = Read<int>(ptr, ref pos);
                            break;

                        case OperandType.InlineI8:
                            buf.operand = Read<long>(ptr, ref pos);
                            break;

                        case OperandType.InlineR:
                            buf.operand = Read<double>(ptr, ref pos);
                            break;

                        case OperandType.InlineString:
                            buf.operand = mod.ResolveString(Read<int>(ptr, ref pos));
                            break;

                        case OperandType.InlineSwitch:
                            {
                                int count = Read<int>(ptr, ref pos);
                                int base_offset = pos + 4 * count;
                                int[] cases = new int[count];
                                for (int x = 0; x < count; x++)
                                    cases[x] = Read<int>(ptr, ref pos) + base_offset;
                                buf.operand = cases;
                            }
                            break;

                        case OperandType.InlineVar:
                            buf.operand = Read<short>(ptr, ref pos);
                            break;

                        case OperandType.ShortInlineBrTarget:
                            buf.operand = Read<byte>(ptr, ref pos) + pos;
                            break;

                        case OperandType.ShortInlineI:
                            buf.operand = Read<byte>(ptr, ref pos);
                            break;

                        case OperandType.ShortInlineR:
                            buf.operand = Read<float>(ptr, ref pos);
                            break;

                        case OperandType.ShortInlineVar:
                            buf.operand = Read<byte>(ptr, ref pos);
                            break;

                        case OperandType.InlineNone:
                            break;

                        default:
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("Unknown operand type: " + code.OperandType);
#endif
                            break;
                    }

                    m_buffers.Add(buf);
                }
        }

        public virtual MethodBodyEditor GetEditor()
        {
            MethodBody body = method.GetMethodBody();
            return new MethodBodyEditor(m_buffers, body.ExceptionHandlingClauses, body.LocalVariables);
        }

        public virtual void ParseIL(MethodBase meth)
        {
            m_buffers.Clear();
            method = meth;
            ParseIL();
        }

        // debugging
        public override string ToString() => Utils.IL2String(m_buffers);

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                foreach (ILBuffer buf in m_buffers)
                    buf.Dispose();

                m_buffers.Clear();
                m_buffers = null;

                method = null;

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        ~MethodBodyReader()
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
