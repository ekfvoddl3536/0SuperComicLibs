#if DEBUG
using System;
using System.Reflection;
using System.Reflection.Emit;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public static class DebugTest
    {
        public static ITokenEnumerator Scan(string text) => new Scanner().FromText(text);

        public static INode Parse(Grammar g, ITokenEnumerator tokE) => new LALRParser(g).Parse(tokE);

        public static Type CodeGen(INode node, string class_name)
        {
            var tb = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DebugTestAsm"), AssemblyBuilderAccess.RunAndCollect)
                    .DefineDynamicModule("DebugModule_0")
                    .DefineType(class_name, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            // var temp = new ILCodeGenerator(
            //     null,
            //     tb,
            //     new CHashSet<HashedString>(),
            //     new HashedString("debugClass"),
            //     ExceptionHandlerFactory.TDebug,
            //     TypeTable.Current);
            // 
            // temp.Generate(node);
            // temp.Dispose();

            return tb.CreateType();
        }
    }
}
#endif