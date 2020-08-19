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
