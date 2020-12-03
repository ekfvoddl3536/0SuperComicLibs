using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using SuperComicLib.Collections;
using SuperComicLib.Core;
using SuperComicLib.Reflection;

namespace SuperComicLib.CodeDesigner
{
    using static CodeGeneratorStates;
    public sealed class ILMethodBuildState
    {
        public string methodName;
        public Type returnType;
        public CILCode[] methodBody;
        // 아직 발견하지 못한 메소드를 호출한 경우, 그 cilcode 위치를 저장
        // 나중에 다시와서 확인하는겸 코드정리할 예정
        public int[] chkMethods;
        // 마찬가지로 이 메소드를 만드는도중 아직 발견하지 못한 필드가 있다면(상수포함)
        // 그 cilcode의 위치를 저장
        public int[] chkFields;
        // 2020-12-03 사용안해도 될 듯
        // private int signautre; // cache할 용도 (선택)

        public ILMethodBuildState(string name, Type retType)
        {
            methodName = name;
            returnType = retType;
        }

//         public bool SignatureEquals(ILMethodBuildState other) =>
//             returnType == other.returnType && Equals(other.parameterTypes);
// 
//         public bool Equals(Type[] parameterTypes)
//         {
// #if DEBUG
//             System.Diagnostics.Contracts.Contract.Requires(this.parameterTypes != null);
//             System.Diagnostics.Contracts.Contract.Requires(parameterTypes != null);
// #endif
//             int arrayLen = parameterTypes.Length;
//             Type[] me = this.parameterTypes;
//             if (arrayLen != me.Length)
//                 return false;
// 
//             for (int x = 0; x < arrayLen; x++)
//                 if (me[x] != parameterTypes[x])
//                     return false;
// 
//             return true;
//         }
// 
//         public int MakeHashedSignature()
//         {
//             if (signautre == 0)
//             {
//                 int retvalue = 7;
//                 IntHash.Combine(ref retvalue, methodName.GetHashCode());
// 
//                 Type[] ps = parameterTypes;
//                 int arrayLen = ps.Length;
//                 while (--arrayLen >= 0)
//                     IntHash.CombineMOD(ref retvalue, ps[arrayLen].FullName.GetFixedHashcode());
// 
//                 signautre = retvalue;
//                 return retvalue;
//             }
// 
//             return signautre;
//         }
    }

    public class ILCodeGenerator : CodeGeneratorBase
    {
        #region const
        protected const FieldAttributes defFieldAttrb = FieldAttributes.Public | FieldAttributes.Static;
        #endregion

        private TypeBuilder tb;
        private ITypeMap typeMap;
        private BarrieredStack<Type> stackTypes;
        private List<CILCode> codes;
        private List<int> absmeths;
        private List<int> absflds;
        /// <summary>
        /// key = function name
        /// </summary>
        private Map<ILMethodBuildState> methods;
        /// <summary>
        /// key = field name
        /// </summary>
        private Dictionary<HashedString, GlobalVarInfo> gidOrFlds;
        /// <summary>
        /// key = local or parameter name
        /// </summary>
        private Dictionary<HashedString, ScopeVarInfo> locOrArgs;
        private LookaheadStack<uint> states;
        private LookaheadStack<INodeEnumerator> nodes;
        private StringBuilder tempStringBuilder;
        private ILMethodBuildState tempBuildState;

        private ILCodeGenerator(IExceptionHandler handler, INode parsedNode) : base(handler, parsedNode)
        {
            stackTypes = new BarrieredStack<Type>();

            codes = new List<CILCode>(16);

            absmeths = new List<int>();
            absflds = new List<int>();

            methods = new Map<ILMethodBuildState>();
            gidOrFlds = new Dictionary<HashedString, GlobalVarInfo>();
            locOrArgs = new Dictionary<HashedString, ScopeVarInfo>();
            
            states = new LookaheadStack<uint>(16);
            nodes = new LookaheadStack<INodeEnumerator>(16);

            tempStringBuilder = new StringBuilder(64);
        }

        public ILCodeGenerator(
            TypeBuilder tb,
            ITypeMap typeMap,
            IExceptionHandler handler,
            INode parsedNode) : this(handler, parsedNode)
        {
            this.tb = tb;
            this.typeMap = typeMap;
        }

        public override bool Generate(int previous_state, out int state, ref object argument)
        {
            // 필요 없음
            // if (m_handler.FailCount > 0)
            // {
            //     state = -1; // error code
            //     return false; // end
            // }

            LookaheadStack<INodeEnumerator> nodes = this.nodes;
            // LookaheadStack<uint> local_states = states;

            state = 0;

            INodeEnumerator ne = m_enumerator;
            IExceptionHandler ehnd = m_handler;

            loop:
            while (ne.MoveNext())
            {
                INode node = ne.Current;
                if (node.ChildCount == 0)
                {
                    OnTokenProcess(ref state, ref argument, node.GetToken(), ne);

                    if (ehnd.FailCount > 0)
                    { // throwed exception
                        // disposed
                        ne.Dispose();
                        while (nodes.Count > 0)
                            nodes.Pop().Dispose();

                        state = -1; // error
                        return false; // break (end)
                    }
                    else if (state != 0)
                    {
                        m_enumerator = ne;
                        return true;
                    }
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
            
            return false; // end
        }

        protected virtual void OnTokenProcess(ref int global_state, ref object argument, Token tok, INodeEnumerator node)
        {
            uint current_state = states.PopOrDefault();
            // 기본 상태
            if (current_state == 0)
            {
                OnDefaultState(current_state, ref global_state, ref argument, tok, node);
                return;
            }

            if (current_state == STATE_TYPE_BUILD)
                OnTokenToType(tok, node);
            else if (current_state == STATE_CHECK_ID)
                OnCheckID(tok, node);
            else if (current_state == STATE_DECL_METHOD)
                OnMethodDeclare(tok, node);
        }

        #region detect type
        protected virtual void OnTokenToType(Token tok, INodeEnumerator node)
        {
            #region note
            // Type tempType = MakeType(tok);
            // if (tt == TokenType.lbracket_angl) // <
            // {
            //     // generic
            //     states.Push(STATE_TYPE_GENERIC);
            //     states.Push(STATE_TYPE_BUILD);
            // }
            // if (tt == TokenType.multiple) // *
            // {
            //     // check type can make pointer
            //     // 만약, node에 *이 연속적이면 여기서 while 돌리자
            //     states.Push(STATE_TYPE_POINTER);
            // }
            // if (tt == TokenType.lbracket_Sq)
            // {
            //     // 현재는 크기가 1인 배열밖에 없지만, 향후 2이상의 rank를 갖는 배열이나 중첩배열이 나올수도 있음
            //     // 다소 공격적이면서 전략적인 설계가 필요해보임
            //     tempType = tempType.MakeArrayType();
            // }
            // stackTypes.Push(tempType);
            #endregion

            // 임시 최적화
            if (tok.type == TokenType.lbracket_Sq)
            {
                node.MoveNext(); // 즉시 끝내기
                stackTypes.Push(MakeType(tok).MakeArrayType());
            }
            else
                stackTypes.Push(MakeType(tok));
        }
        #endregion

        #region detect method or field
        protected virtual void OnCheckID(Token tok, INodeEnumerator node)
        {
            // type 하나를 소비한다
            // 여기서 field와 method가 분기된다

            node.MoveNext();

            if (node.Current.GetToken().type == TokenType.semi_colon)
            { // FIELD
                HashedString tempID = new HashedString(tok.text);
                if (gidOrFlds.ContainsKey(tempID))
                    m_handler.Fail(CIL_FMSG.F1);
                else
                    gidOrFlds.Add(tempID, new GlobalVarInfo(tb.DefineField(tok.text, stackTypes.Pop(), defFieldAttrb)));
            }
            else
            { // METHOD
              // 할 게 많다
                tempBuildState = new ILMethodBuildState(tok.text, stackTypes.Pop());

                stackTypes.MarkPoint();

                states.Push(0);
                states.Push(STATE_DECL_METHOD);

                node.MoveNext(); // skip '('
            }
        }

        protected virtual void OnMethodDeclare(Token tok, INodeEnumerator node)
        {
            LookaheadStack<uint> states = this.states;

            #region localState list
            const uint loc_argDecl = 1;
            #endregion

            uint localState = states.Pop();
            if (localState == 0)
            {
                if (tok.type == TokenType.comma)
                    node.MoveNext();
                else if (tok.type == TokenType.rparen) // end
                {
                    OnEndMethodDeclare();
                    return;
                }

                states.Push(loc_argDecl);
                states.Push(STATE_DECL_METHOD);

                states.Push(STATE_TYPE_BUILD);
            }
            else if (localState == loc_argDecl)
            {
                HashedString tempID = new HashedString(tok.text);

                Dictionary<HashedString, ScopeVarInfo> locOrArgs = this.locOrArgs;
                if (locOrArgs.ContainsKey(tempID))
                {
                    m_handler.Fail(CIL_FMSG.F1);
                    return;
                }

                locOrArgs.Add(tempID, new ScopeVarInfo(stackTypes.Pop(), locOrArgs.Count));

                states.Push(0);
                states.Push(STATE_DECL_METHOD);
            }
        }

        protected virtual void OnEndMethodDeclare()
        {
            Type[] parameterTypes = stackTypes.ToArray();
            ILMethodBuildState currentMethod = tempBuildState;

            StringBuilder sb = tempStringBuilder;
            
            sb.Append(tempBuildState.methodName);

            
            sb.Clear();
        }
        #endregion

        #region token process impl
        protected virtual void OnDefaultState(uint local_state, ref int global_state, ref object argument, Token tok, INodeEnumerator node)
        {
            TokenType tt = tok.type;
            // 기본 상태의 EOL은 처리되지 않은 토큰이거나 처리할 필요가 없는 무의미한 토큰
            if (tt == TokenType.EOL)
                return;

            // type 먼저 제거
            if (tt == TokenType.type)
            {
                // 2.0 중첩 상태는 node와 nodes를 적절히 섞어서 처리
                // 이 다음 토큰은 [ (배열)또는 id 이다

                // ** 역순으로 넣는다 **
                states.Push(STATE_CHECK_ID);
                states.Push(STATE_TYPE_BUILD);
            }
        }
        #endregion

        #region helper
        protected Type MakeType(Token v) => null;

        protected Type MakeType(Token v, INodeEnumerator nodes)
        {
            Token k = nodes.Peek().GetToken();
            TokenType kt = k.type;
            if (kt == TokenType.lbracket_angl) // <
            {
                nodes.MoveNext();
                var iterTemp = nodes.Current.GetEnumerator();

                iterTemp.Dispose();
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Open {nameof(INodeEnumerator)}.{nameof(INode.Parent)}");
#endif
            
            return null;
        }
        #endregion

        #region disposable
        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }
        #endregion
    }
}
