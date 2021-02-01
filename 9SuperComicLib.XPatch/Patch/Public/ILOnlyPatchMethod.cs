﻿using SuperComicLib.Core;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperComicLib.XPatch
{
    public sealed class ILOnlyPatchMethod : ExMethodInfo
    {
        private delegate void P1(ILGenerator il, MethodBase original, int offset, bool hasReturn, bool isValuetype);
        private delegate bool P2(ILGenerator il, MethodBase original, int offset, bool hasReturn, bool isValuetype);

        private static readonly Type p1t = typeof(P1), p2t = typeof(P2);

        private P1 v1;
        private P2 v2;

        public ILOnlyPatchMethod(MethodInfo info) : base(info)
        {
            if (info.ReturnType == CTypes.VOID_T)
                v1 = (P1)info.CreateDelegate(p1t);
            else
                v2 = (P2)info.CreateDelegate(p2t);
        }

        public override bool GenerateCode(
            ILGenerator il,
            MethodBase original,
            ParameterInfo[] parameters,
            int offset,
            bool hasReturn,
            bool hasArgBuffer,
            bool isValuetype)
        {
            if (toTypes == null || toTypes.Length < 4)
                // 인수 길이가 충분하지 않습니다
                throw new InvalidOperationException("Parameter length is not long enough");
            if (toTypes[0] != typeof(ILGenerator) || original.GetType().IsAssignableFrom(toTypes[1]) || toTypes[2] != typeof(int) || toTypes[3] != CTypes.BOOL_T)
                // ILOnly의 패치함수는 반드시 (ILGenerator, <MethodInfo/ConstructorInfo>, int, bool, bool) 인수를 가져야합니다
                throw new InvalidOperationException("ILOnly patch method must have parameters (ILGenerator, <MethodBase/MethodInfo/ConstructorInfo>, int, bool)");
            if (patching.ReturnType != CTypes.VOID_T && patching.ReturnType != CTypes.BOOL_T)
                // 패치 메소드의 반환은 void 또는 bool이어야 합니다
                throw new InvalidOperationException("Return type of the patch method must be void or bool");

            if (v2 == null)
            {
                v1.Invoke(il, original, offset, hasReturn, isValuetype);
                return false;
            }
            else
                return v2.Invoke(il, original, offset, hasReturn, isValuetype);
        }

        protected override void Dispose(bool disposing)
        {
            v1 = null;
            v2 = null;
            base.Dispose(disposing);
        }
    }
}