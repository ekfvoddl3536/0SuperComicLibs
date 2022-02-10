using System;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    internal static class ILCodeGenHelper
    {
        private static string TypeToString(Type type) => type.FullName;

        public static HashedString CombineAuto(string name, Type[] parameters) =>
            HashedString.ConvertAll(name, parameters.FastConvertAll(TypeToString));

        public static void AddExprState(LookaheadStack<uint> states, uint start, uint end)
        {
            states.Push(end);

            states.Push(0);
            //states.Push(CodeGeneratorStates.STATE_METHODBODY);

            states.Push(start);
        }
    }
}
