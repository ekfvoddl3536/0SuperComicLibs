// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
// Copyright (c) 2017 Andreas Pardeike
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SuperComicLib.Runtime;

namespace SuperComicLib.XPatch
{
    public unsafe class MethodBodyReader : IDisposable
    {
        protected List<ILBuffer> m_buffers;
        protected MethodBase method;
        protected Module module;

        public MethodBodyReader(MethodBase method) : this(method, (Module)null, false) { }

        public MethodBodyReader(MethodBase method, bool isDynamicMethod) : this(method, (Module)null, isDynamicMethod) { }

        public MethodBodyReader(MethodBase method, Type owner, bool isDynamicMethod) : this(method, owner.Module, isDynamicMethod) { }

        public MethodBodyReader(MethodBase method, Module module, bool isDynamicMethod)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            m_buffers = new List<ILBuffer>();

            this.method = 
                isDynamicMethod
                ? new RTDynamicMethodInfo(method as DynamicMethod)
                : method;

            this.module = module ?? method.Module;
        }

        protected static T Read<T>(byte* ptr, ref int pos) where T : unmanaged
        {
            T result = *(T*)(ptr + pos);
            pos += sizeof(T);
            return result;
        }

        public void Parse() => ParseIL();

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
                        code = OpCodeConverter.GetCode_sz2(Read<byte>(ptr, ref pos));
                    else
                        code = OpCodeConverter.GetCode_sz1(temp);

                    buf.code = code;

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
        protected virtual void Dispose(bool disposing)
        {
            if (m_buffers != null)
            {
                foreach (ILBuffer buf in m_buffers)
                    buf.Dispose();

                m_buffers.Clear();

                if (disposing)
                {
                    m_buffers = null;
                    method = null;
                }
            }
        }

        ~MethodBodyReader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
