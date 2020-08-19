using SuperComicLib.Core;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SuperComicLib.Reflection;

namespace SuperComicLib.XPatch
{
    public class ExMethodInfo : IDisposable
    {
        public MethodInfo patching;
        public string[] toNames;
        public Type[] toTypes;

        public ExMethodInfo(MethodInfo info)
        {
            patching = info;

            ParameterInfo[] temp = info.GetParameters();
            int x = temp.Length;

            toNames = new string[x];
            toTypes = new Type[x];

            for (x--; x >= 0; x--)
            {
                ref ParameterInfo i = ref temp[x];
                toNames[x] = i.Name;
                toTypes[x] = i.ParameterType;
                i = null;
            }
        }

        public virtual bool GenerateCode(ILGenerator il, MethodBase original, ParameterInfo[] parameters, int offset, bool hasReturn, bool hasArgBuffer)
        {
            int idx = 0;
            int len = toNames.Length;
            if (idx < len)
            {
                if (toNames[0] == "out" && hasReturn)
                {
                    il.Emit(OpCodes.Ldloca_S, (byte)0);
                    idx++;
                }

                if (idx < len && toNames[idx] == "this")
                {
                    if (original.IsStatic)
                        // static 메소드의 첫번째 인수는 @this 일 수 없습니다
                        throw new InvalidOperationException("The first parameter of the static method cannot be @this");

                    if (toTypes[idx].IsByRef)
                        il.Emit(OpCodes.Ldarga_S, (byte)0);
                    else
                        il.Emit(OpCodes.Ldarg_0);
                    idx++;
                }

                for (; idx < len; idx++)
                {
                    ParameterInfo param = parameters.FirstOrDefault(t =>
                    {
                        string now = toNames[idx];
                        return 
                            now.StartsWith("param_") 
                            ? now.Remove(0, 5) == t.Name 
                            : now == t.Name;
                    });
                    if (param == null)
                        continue;
                    if (toTypes[idx].IsByRef)
                        il.Emit_Ldarga(param.Position + offset);
                    else
                        il.Emit_Ldarg(param.Position + offset);
                }
            }
            il.Emit(OpCodes.Call, patching);

            if (patching.ReturnType != CTypes.VOID_T && patching.ReturnType != CTypes.BOOL_T)
                // 패치 메소드의 반환은 void 또는 bool이어야 합니다
                throw new InvalidOperationException("Return type of the patch method must be void or bool");

            return patching.ReturnType == CTypes.BOOL_T;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                ClsArray.DeleteAll(ref toNames);
                patching = null;

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        ~ExMethodInfo()
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
