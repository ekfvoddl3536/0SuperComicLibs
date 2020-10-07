using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using SuperComicLib.Collections;
using SuperComicLib.Reflection;

namespace SuperComicLib.CodeDesigner
{
    public class ILCodeGenerator : CodeGeneratorBase
    {
        private ILScriptLoader owner;
        private TypeBuilder tb;
        private CHashSet<HashedString> refers;
        private HashedString name;
        private ITypeMap typeMap;
        private Stack<Label> labels;
        private Stack<Label> escLabels;
        private ListStack<CILCode> codes;
        private Dictionary<HashedString, MethodBuilder> methods;
        private Dictionary<HashedString, FieldBuilder> fields;
        private Dictionary<HashedString, Type> gids;
        private Dictionary<HashedString, VarInfo> locOrArgs;
        private MethodBuilder currentMeth;
        private ILGenerator ilgen;

        private ILCodeGenerator(IExceptionHandler handler) : base(handler)
        {
        }

        public ILCodeGenerator(
            ILScriptLoader owner,
            TypeBuilder tb,
            CHashSet<HashedString> refers,
            HashedString name,
            IExceptionHandler handler,
            ITypeMap typeMap) : this(handler)
        {
            this.owner = owner;
            this.tb = tb;
            this.refers = refers;
            this.name = name;
            this.typeMap = typeMap;
        }

        protected override void OnGenerate(INodeEnumerator ne)
        {
            uint current_state = 0;
            uint current_argument = 0;

            Stack<INodeEnumerator> nodes;
            if (CILCodegenOpt.InitialSize == 0)
                nodes = new Stack<INodeEnumerator>();
            else
                nodes = new Stack<INodeEnumerator>(ne.DeepCount(CILCodegenOpt.InitialSize).Max(4));


            loop:
            while (ne.MoveNext())
            {
                INode node = ne.Current;
                if (node.ChildCount == 0)
                {
                    OnTokenProcess(ref current_state, ref current_argument, node.GetToken());
                    if (current_argument != 0 && OnArgument(current_state, current_argument))
                        current_argument = 0;
                }
                else
                {
                    nodes.Push(ne);
                    ne = node.GetEnumerator();
                }
            }

            ne.Dispose();
            if (nodes.Count > 0)
            {
                ne = nodes.Pop();
                goto loop;
            }
        }

        protected virtual void OnTokenProcess(ref uint state, ref uint argument, Token tok)
        {
            
        }

        protected virtual bool OnArgument(uint state, uint prev_argument)
        {
            return true;
        }
    }
}
