#if DEBUG
using System;
using System.Collections.Generic;
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

    public sealed class DebugCalc : CodeGeneratorBase
    {
        public DebugCalc(IExceptionHandler handler, INode parsedNode) : base(handler, parsedNode)
        {
        }

        public override void Generate(ScriptLoader owner)
        {
            INodeEnumerator ne = m_enumerator;

            Stack<INodeEnumerator> nodes = new Stack<INodeEnumerator>();
            
            Queue<long> vs = new Queue<long>(2);

            while (ne.MoveNext())
            {
                INode node = ne.Current;
                if (node.ChildCount == 0)
                {
                    Token t = node.GetToken();
                    TokenType tt = t.type;
                    if (tt == TokenType.literal_int_4)
                        vs.Enqueue((int)t.state);
                    else
                    {
                        long a1 = vs.Dequeue(),
                            a2 = vs.Dequeue();
                        if (tt == TokenType.plus)
                            vs.Enqueue(a1 + a2);
                        else if (tt == TokenType.minus)
                            vs.Enqueue(a1 - a2);
                        else if (tt == TokenType.multiple)
                            vs.Enqueue(a1 * a2);
                        else
                            vs.Enqueue(a1 / a2);
                    }
                }
                else
                {
                    nodes.Push(ne);
                    ne = node.GetEnumerator();
                }
            }

            Console.WriteLine(vs.Dequeue());
        }
    }
}
#endif