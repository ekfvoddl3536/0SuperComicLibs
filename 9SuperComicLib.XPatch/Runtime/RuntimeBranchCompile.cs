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

namespace SuperComicLib.XPatch
{
    public static class RuntimeBranchCompile<T> where T : Delegate
    {
        public static void Do(T current, T true_branch, T false_branch, bool condition)
        {
            MethodInfo requestMethod = current.Method;
            if (requestMethod.GetCustomAttribute<ILBranchableAttribute>() == null)
                throw new InvalidOperationException("Methods that cannot branch at runtime");

            MethodInfo branchTargetMethod = (condition ? true_branch : false_branch).Method;
            if (requestMethod.IsStatic ^ branchTargetMethod.IsStatic)
                throw new InvalidOperationException("Parameters do not match");

            RuntimeBranchCompile.Br(requestMethod.MethodHandle, branchTargetMethod.MethodHandle);
        }

        public static void Force(T current, T branch)
        {
            MethodInfo requestMethod = current.Method;
            if (requestMethod.GetCustomAttribute<ILBranchableAttribute>() == null)
                throw new InvalidOperationException("Methods that cannot branch at runtime");

            RuntimeBranchCompile.Br(requestMethod.MethodHandle, branch.Method.MethodHandle);
        }

        public static void Force(T current, MethodInfo info)
        {
            MethodInfo requestMethod = current.Method;
            if (requestMethod.GetCustomAttribute<ILBranchableAttribute>() == null)
                throw new InvalidOperationException("Methods that cannot branch at runtime");

            if (info is DynamicMethod dm)
                RuntimeBranchCompile.Br(requestMethod.MethodHandle, dm);
            else
                RuntimeBranchCompile.Br(requestMethod.MethodHandle, info.MethodHandle);
        }
    }

    public static class RuntimeBranchCompile
    {
        public static void Force(MethodInfo curinfo, MethodInfo tarinfo)
        {
            if (curinfo.GetCustomAttribute<ILBranchableAttribute>() == null)
                throw new InvalidOperationException("Methods that cannot branch at runtime");

            if (tarinfo is DynamicMethod dm)
                Br(curinfo.MethodHandle, dm);
            else
                Br(curinfo.MethodHandle, tarinfo.MethodHandle);
        }

        public static void Br(MethodInfo curinfo, MethodInfo tarinfo)
        {
            if (curinfo.GetCustomAttribute<ILBranchableAttribute>() == null)
                throw new InvalidOperationException("Methods that cannot branch at runtime");

            var x = curinfo.MethodHandle.GetFunctionPointer();
            if (tarinfo is DynamicMethod dm)
                Helper.WriteIL(x, dm.PrepareMethod());
            else
                Helper.WriteIL(
                    curinfo.MethodHandle.GetFunctionPointer(),
                    tarinfo.MethodHandle.GetFunctionPointer());
        }

        internal static void Br(RuntimeMethodHandle hnd, DynamicMethod target)
        {
            RuntimeHelpers.PrepareMethod(hnd);

            Helper.WriteIL(hnd.GetFunctionPointer(), target.PrepareMethod());
        }

        internal static void Br(RuntimeMethodHandle curhnd, RuntimeMethodHandle tarhnd)
        {
            RuntimeHelpers.PrepareMethod(curhnd);
            RuntimeHelpers.PrepareMethod(tarhnd);

            Helper.WriteIL(
                curhnd.GetFunctionPointer(),
                tarhnd.GetFunctionPointer());
        }
    }
}
