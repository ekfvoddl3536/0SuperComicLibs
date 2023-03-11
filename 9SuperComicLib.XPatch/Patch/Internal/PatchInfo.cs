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

using SuperComicLib.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SuperComicLib.Runtime;

namespace SuperComicLib.XPatch
{
    internal sealed class PatchInfo : IDisposable
    {
        public MethodBase method;

        public List<ExMethodInfo> prefixes = new List<ExMethodInfo>();
        public List<ExMethodInfo> postfixes = new List<ExMethodInfo>();
        public ExMethodInfo replace;

        public PatchInfo(MethodBase originalMethod) => method = originalMethod;

        public unsafe DynamicMethod DoPatch()
        {
            #region 시작
            Type retType = Helper.ReturnType(method);

            ParameterInfo[] parameters = method.GetParameters();
            List<Type> argTypes = new List<Type>();

            bool hasReturn = retType != typeof(void);

            bool hasRetBuf;
            bool isvalue;
            int offset;
            if (method.IsStatic == false)
            {
                Type temp = method.DeclaringType;
                if (isvalue = temp.IsStruct())
                    argTypes.Add(temp.MakeByRefType());
                else
                    argTypes.Add(temp);

                hasRetBuf = NativeThisPointer.NeedsPointerFix(retType);
                if (hasRetBuf)
                {
                    argTypes.Add(typeof(IntPtr));

                    // [0] = instance
                    // [1] = buffer
                    offset = 2;
                }
                else
                {
                    // [0] = instance
                    offset = 1;
                }
            }
            else // static, 정적 메서드는 return buffer가 없다, 참고: NativeThisPointer.cs
            {
                offset = 0;
                isvalue = hasRetBuf = false;
            }

            argTypes.AddRange(Array.ConvertAll(parameters, t => t.ParameterType));
            #endregion

            // dynamicmethod는 by ref 반환을 지원하지 않는다
            // 하지만, by ref 가 주소를 반환하는 것이므로 (실제 IL 표현이 T& 이다)
            // 주소를 저장하여 내보내주면 된다
            //
            // 만약 이게 안되면, by ref를 지원하지 말자
            // A: 그리고 됨
            if (hasReturn && retType.IsNative())
                retType = typeof(IntPtr);

            string methodname = method.GetMethodName(prefixes.Count + postfixes.Count, replace != null);
            DynamicMethod dm = new DynamicMethod(
                methodname,
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                hasRetBuf ? typeof(void) : retType,
                argTypes.ToArray(),
                method.DeclaringType,
                true);

            #region 매개변수 
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo p = parameters[i];
                dm.DefineParameter(i + offset, p.Attributes, p.Name);
            }
            #endregion

            #region 선행 작업
            ILGenerator il = dm.GetILGenerator();
            LocalBuilder retlocal = hasReturn ? il.DeclareLocal(retType) : null;

            LocalBuilder callOriginal = null;

            Helper.TryNoInlining(method);
            #endregion

            #region prefix
            // Prefix 메서드들 코드 생성
            IEnumerator<ExMethodInfo> iter = prefixes.GetEnumerator();
            while (iter.MoveNext())
                if (iter.Current.GenerateCode(il, method, parameters, offset, hasReturn, hasRetBuf, isvalue))
                {
                    if (callOriginal == null)
                        callOriginal = il.DeclareLocal(typeof(bool));

                    il.Emit_Stloc(callOriginal.LocalIndex);
                }
            iter.Dispose();
            #endregion

            #region 본 함수, ======= if (callOriginal)
            // jump 가능성이 있는 경우
            // if 코드 조각 생성
            if (callOriginal != null)
            {
                Label skipOrigianlMethod = il.DefineLabel();
                il.Emit_Ldloc(callOriginal.LocalIndex);
                il.Emit(OpCodes.Brfalse, skipOrigianlMethod);

                // 함수 원본 실행
                if (replace == null)
                    Helper.OriginalMethodBodyEmit(il, method, offset, hasReturn);
                else
                    // 만약 replace가 null이 아니면 replace를 실행
                    replace.GenerateCode(il, method, parameters, offset, hasReturn, hasRetBuf, isvalue);

                // 리턴이 있는경우, 리턴값을 저장
                if (hasReturn)
                    il.Emit(OpCodes.Stloc_0);

                // (jump 가능성이 있는 경우)
                // if에 의해 넘어옴
                il.MarkLabel(skipOrigianlMethod);
            }
            else
            {
                if (replace == null)
                    Helper.OriginalMethodBodyEmit(il, method, offset, hasReturn);
                else
                    replace.GenerateCode(il, method, parameters, offset, hasReturn, hasRetBuf, isvalue);

                if (hasReturn)
                    il.Emit(OpCodes.Stloc_0);
            }
            #endregion

            #region postfix, ======= else
            // Postfix 메서드들 코드 생성
            iter = postfixes.GetEnumerator();
            while (iter.MoveNext())
                iter.Current.GenerateCode(il, method, parameters, offset, hasReturn, hasRetBuf, isvalue);
            iter.Dispose();
            #endregion

            #region 끝
            // ret
            if (hasReturn)
                if (hasRetBuf)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Stobj, retType);
                }
                else
                    il.Emit(OpCodes.Ldloc_0);

            il.Emit(OpCodes.Ret);

            Helper.ReplaceMethod(method, dm.PrepareMethod());
            #endregion

            return dm;
        }

        public void Dispose()
        {
            if (method != null)
            {
                prefixes.Clear();
                postfixes.Clear();

                method = null;
                replace = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
