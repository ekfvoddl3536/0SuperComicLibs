using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.Reflection.Patch
{
    internal sealed class PatchInfo : IDisposable
    {
        public MethodBase method;

        public List<ExMethodInfo> prefixes = new List<ExMethodInfo>();
        public List<ExMethodInfo> postfixes = new List<ExMethodInfo>();
        public ExMethodInfo replace;

        public event Action Finalizes;

        public PatchInfo(MethodBase originalMethod) => method = originalMethod;

        public unsafe DynamicMethod DoPatch()
        {
            #region 시작
            Type retType = Helper.ReturnType(method);

            ParameterInfo[] parameters = method.GetParameters();
            List<Type> argTypes = new List<Type>();

            bool hasReturn = retType != Helper.VOID_T;

            bool hasRetBuf;
            int offset;
            if (method.IsStatic == false)
            {
                Type temp = method.DeclaringType;
                hasRetBuf = NativeThisPointer.NeedsPointerFix(retType);
                if (hasRetBuf)
                {
                    if (temp.IsStruct())
                        argTypes.Add(temp.MakeByRefType());
                    else
                        argTypes.Add(temp);

                    argTypes.Add(typeof(IntPtr));

                    // [0] = instance
                    // [1] = buffer
                    offset = 2;
                }
                else
                {
                    if (temp.IsStruct())
                        argTypes.Add(temp.MakeByRefType());
                    else
                        argTypes.Add(temp);

                    // [0] = instance
                    offset = 1;
                }
            }
            else // static, 정적 메서드는 return buffer가 없다, 참고: NativeThisPointer.cs
            {
                offset = 0;
                hasRetBuf = false;
            }

            argTypes.AddRange(Array.ConvertAll(parameters, t => t.ParameterType));
            #endregion

            // dynamicmethod는 by ref 반환을 지원하지 않는다
            // 하지만, by ref 가 주소를 반환하는 것이므로 (실제 IL 표현이 T& 이다)
            // 주소를 저장하여 내보내주면 된다
            if (hasReturn && retType.IsNative())
                retType = typeof(IntPtr);

            string methodname = method.GetMethodName(prefixes.Count + postfixes.Count, replace != null);
            DynamicMethod dm = new DynamicMethod(
                methodname,
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                hasRetBuf ? Helper.VOID_T : retType,
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
            foreach (ExMethodInfo prefix in prefixes)
                if (prefix.GenerateCode(il, method, parameters, offset, hasReturn, hasRetBuf))
                {
                    if (callOriginal == null)
                        callOriginal = il.DeclareLocal(Helper.BOOL_T);
                    il.Emit(OpCodes.Stloc, callOriginal);
                }
            #endregion

            #region 본 함수, ======= if (callOriginal)
            // jump 가능성이 있는 경우
            // if 코드 조각 생성
            if (callOriginal != null)
            {
                Label skipOrigianlMethod = il.DefineLabel();
                il.Emit(OpCodes.Ldloc, callOriginal);
                il.Emit(OpCodes.Brfalse, skipOrigianlMethod);

                // 함수 원본 실행
                if (replace == null)
                    Helper.OriginalMethodBodyEmit(il, method, offset, hasReturn);
                else
                    // 만약 replace가 null이 아니면 replace를 실행
                    replace.GenerateCode(il, method, parameters, offset, hasReturn, hasRetBuf);

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
                    replace.GenerateCode(il, method, parameters, offset, hasReturn, hasRetBuf);

                if (hasReturn)
                    il.Emit(OpCodes.Stloc_0);
            }
            #endregion

            #region postfix, ======= else
            // Postfix 메서드들 코드 생성
            foreach (ExMethodInfo postfix in postfixes)
                postfix.GenerateCode(il, method, parameters, offset, hasReturn, hasRetBuf);
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
            ClsArray.DisposeAll(ref prefixes);
            ClsArray.DisposeAll(ref postfixes);
            if (replace != null)
                replace.Dispose();
            method = null;

            Finalizes?.Invoke();
            Finalizes = null;
        }
    }
}
