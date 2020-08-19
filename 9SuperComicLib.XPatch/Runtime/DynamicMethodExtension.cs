using SuperComicLib.Core;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SuperComicLib.XPatch
{
    using static Helper;
    public static class DynamicMethodExtension
    {
        private static readonly MethodInfo m_compileMeth = typeof(RuntimeHelpers).GetMethod("_CompileMethod", mflag1);

        public static IntPtr PrepareMethod(this DynamicMethod meth)
        {
            Type t = meth.GetType();
            RuntimeMethodHandle handle =
                    t.GetMethod("GetMethodDescriptor", mflag0) is MethodInfo getmd
                    ? (RuntimeMethodHandle)getmd.Invoke(meth, null)
                    : t.GetField("m_method", mflag0) is FieldInfo mm
                    ? (RuntimeMethodHandle)mm.GetValue(meth)
                    : (RuntimeMethodHandle)t.GetField("mhandle", mflag0).GetValue(meth);

            Type ht = handle.GetType();

            object result =
                ht.GetField("m_value", mflag0) is FieldInfo fd
                ? fd.GetValue(handle)
                : ht.GetMethod("GetMethodInfo", mflag0) is MethodInfo mi
                ? mi.Invoke(handle, null)
                : null;

            if (result != null)
                try
                {
                    // 2020 01 30
                    // dynamicmethod 코드에 전혀 문제가 없는 경우에
                    // 여기서 오류가 발생하지 않는다
                    m_compileMeth.Invoke(null, new object[1] { result });
                    return handle.GetFunctionPointer();
                }
#pragma warning disable
                catch
#pragma warning restore
                {
                }

            ParameterInfo p = m_compileMeth.GetParameters()[0];
            // _CompileMethod(IntPtr)
            if (p.ParameterType.IsAssignableFrom(CTypes.INTPTR_T))
            {
                m_compileMeth.Invoke(null, new object[1] { handle.Value });
                return handle.GetFunctionPointer();
            }

            // _CompileMethod(RuntimeMethodHandle)
            if (p.ParameterType.IsAssignableFrom(ht))
                m_compileMeth.Invoke(null, new object[1] { handle });

            return handle.GetFunctionPointer();
        }
    }
}
