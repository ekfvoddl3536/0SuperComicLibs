using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SuperComicLib.Reflection
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

            BrThis(requestMethod.MethodHandle, branchTargetMethod.MethodHandle);
        }

        public static void Force(T current, T branch)
        {
            MethodInfo requestMethod = current.Method;
            if (requestMethod.GetCustomAttribute<ILBranchableAttribute>() == null)
                throw new InvalidOperationException("Methods that cannot branch at runtime");

            MethodInfo branchTargetMethod = branch.Method;
            if (requestMethod.IsStatic ^ branchTargetMethod.IsStatic)
                throw new InvalidOperationException("Parameters do not match");

            BrThis(requestMethod.MethodHandle, branchTargetMethod.MethodHandle);
        }

        private static void BrThis(RuntimeMethodHandle curhnd, RuntimeMethodHandle tarhnd)
        {
            RuntimeHelpers.PrepareMethod(curhnd);
            RuntimeHelpers.PrepareMethod(tarhnd);

            Helper.WriteIL(
                curhnd.GetFunctionPointer(),
                tarhnd.GetFunctionPointer());
        }
    }
}
