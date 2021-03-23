using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    using static CodeGeneratorStates;
    public partial class ILCodeGenerator : CodeGeneratorBase
    {
        #region const
        protected internal const FieldAttributes fldAttrb = FieldAttributes.Public | FieldAttributes.Static;
        protected internal const MethodAttributes methAttrb = MethodAttributes.Public | MethodAttributes.Static;
        #endregion

        #region field
        protected TypeBuilder tb;
        protected ITypeMap typeMap;
        protected InternalTypeStack stackTypes;
        protected List<CILCode> codes;
        protected List<int> absmeths;
        protected List<int> absflds;
        /// <summary>
        /// key = function name + parameter type names
        /// </summary>
        protected Dictionary<ILMethodSignature, ILMethodBuildState> methods;
        /// <summary>
        /// key = field name
        /// </summary>
        protected Dictionary<HashedString, FieldBuilder> fields;
        /// <summary>
        /// key = local or parameter name
        /// </summary>
        protected Dictionary<HashedString, ScopeVarInfo> locOrParam;
        /// <summary>
        /// key = label name
        /// </summary>
        protected Dictionary<HashedString, int> locLabels;
        protected LookaheadStack<uint> states;
        protected LookaheadStack<INodeEnumerator> nodes;
        protected ILMethodBuildState currentMethod;
        protected LabelManager labelMgr;
        protected string tempString;
        #endregion

        #region constructor
        private ILCodeGenerator(IExceptionHandler handler, INode parsedNode) : base(handler, parsedNode)
        {
            stackTypes = new InternalTypeStack(64);

            codes = new List<CILCode>(32);

            absmeths = new List<int>();
            absflds = new List<int>();

            methods = new Dictionary<ILMethodSignature, ILMethodBuildState>();
            fields = new Dictionary<HashedString, FieldBuilder>();
            locOrParam = new Dictionary<HashedString, ScopeVarInfo>();

            states = new LookaheadStack<uint>(16);
            nodes = new LookaheadStack<INodeEnumerator>(16);

            labelMgr = new LabelManager();
        }

        public ILCodeGenerator(TypeBuilder tb, ITypeMap typeMap, IExceptionHandler handler, INode parsedNode) : this(handler, parsedNode)
        {
            this.tb = tb;
            this.typeMap = typeMap;
        }
        #endregion

        #region methods
        public override void Generate(ScriptLoader owner)
        {
            LookaheadStack<INodeEnumerator> nodes = this.nodes;
            LookaheadStack<uint> states = this.states;

            int proc_state = 0;
            object proc_arg = null;

            INodeEnumerator ne = m_enumerator;
            IExceptionHandler ehnd = m_handler;

            loop:
            uint state = states.PopOrDefault();

            while (ne.MoveNext())
            {
                INode node = ne.Current;
                if (node.ChildCount == 0)
                {
                    bool goNext;
                    do
                    {
                        goNext = OnGenerate(node.GetToken(), state, ref proc_state, ref proc_arg);

                        if (ehnd.FailCount > 0)
                        {
                            ne.Dispose();
                            while (nodes.Count > 0)
                                nodes.Pop().Dispose();

                            return;
                        }
                        else if (proc_state != 0)
                            proc_arg = OnRequested(ref state, ref proc_state, proc_arg);

                        // 다음으로 넘어가지 않는다
                        // 반복
                    } while (!goNext);
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

            // 끝내기 전에 마무리지어야 한다
        }

        /// <summary>
        /// 요청된 state에 대한 자료를 처리합니다
        /// </summary>
        /// <param name="processing_state">요청된 state</param>
        /// <param name="in_argument">함께 전달된 argument (default: null)</param>
        /// <returns>반환할 argument</returns>
        protected virtual object OnRequested(ref uint state, ref int processing_state, object in_argument)
        {
            if (processing_state <= PROC_SKIP)
                processing_state = 0;
            else if (processing_state == PROC_SYSTEM_NEXTSTATE)
            {
                state = states.PopOrDefault();
                processing_state = 0;
            }

            return null;
        }
        #endregion
    }
}
