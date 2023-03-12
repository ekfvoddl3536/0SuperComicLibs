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
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using static SuperComicLib.XPatch.Helper;

namespace SuperComicLib.XPatch
{
    public static class DynamicMethodExtension
    {
        private static readonly MethodInfo m_compileMeth = typeof(RuntimeHelpers).GetMethod("_CompileMethod", mflag1);

        public static IntPtr PrepareMethod(this DynamicMethod meth)
        {
            Type t = meth.GetType();

            // mono only
            MethodInfo mi = t.GetMethod("CreateDynMethod", mflag0);
            if (mi != null)
            {
                mi.Invoke(meth, null);
                return ((RuntimeMethodHandle)t.GetField("mhandle", mflag0).GetValue(meth)).GetFunctionPointer();
            }

            RuntimeMethodHandle handle;
            FieldInfo fi;

            mi = t.GetMethod("GetMethodDescriptor", mflag0);
            if (mi != null)
                handle = (RuntimeMethodHandle)mi.Invoke(meth, null);
            else
            {
                fi = t.GetField("m_method", mflag0);
                handle =
                    fi != null
                    ? (RuntimeMethodHandle)fi.GetValue(meth)
                    : default;
            }

            t = handle.GetType();

            object result;
            fi = t.GetField("m_value", mflag0);
            if (fi != null)
                result = fi.GetValue(handle);
            else
            {
                fi = t.GetField("Value", mflag0);
                if (fi != null)
                    result = fi.GetValue(handle);
                else
                {
                    mi = t.GetMethod("GetMethodInfo", mflag0);
                    if (mi != null)
                        result = mi.Invoke(handle, null);
                    else
                        result = null;
                }
            }

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
            if (p.ParameterType.IsAssignableFrom(typeof(IntPtr)))
            {
                m_compileMeth.Invoke(null, new object[1] { handle.Value });
                return handle.GetFunctionPointer();
            }

            // _CompileMethod(RuntimeMethodHandle)
            if (p.ParameterType.IsAssignableFrom(t))
                m_compileMeth.Invoke(null, new object[1] { handle });

            return handle.GetFunctionPointer();
        }
    }
}
