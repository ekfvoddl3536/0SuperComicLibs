using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SuperComicLib.Collections;
using SuperComicLib.Reflection;

namespace SuperComicLib.CodeDesigner
{
    using static CodeGeneratorStates;
    partial class ILCodeGenerator
    {
        #region helper
        #region type 관련
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        protected void MakeArrayAuto() => stackTypes.Push(stackTypes.Pop().MakeArrayType());

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.PreserveSig)]
        protected void PushTypeAuto(Token v) =>
            stackTypes.Push(typeMap.Get(v.text).NormalType);

        protected void MakeGenericTypeAuto()
        {
            var stackTypes = this.stackTypes;

            Type[] vs = stackTypes.PopAuto();
            Type last = stackTypes.Pop();

            stackTypes.Push(last.MakeGenericType(vs));
        }
        #endregion


        #endregion

        /// <summary>
        /// 코드를 분석하고 의미를 생성합니다
        /// </summary>
        /// <param name="token">현재 토큰</param>
        /// <param name="current_state">현재 상태 (default: 0)</param>
        /// <param name="processing_state">주고받을 데이터 상태 (default: 0)</param>
        /// <param name="argument">주고받을 수 있는 데이터</param>
        /// <returns>다음 토큰으로 넘어가려면 true를 반환합니다</returns>
        protected virtual bool OnGenerate(Token token, uint current_state, ref int processing_state, ref object argument)
        {
            if ((processing_state & PROC_SKIP) > PROC_SKIP)
            {
                processing_state--;
                return true;
            }

            if (current_state != 0)
                return OnGenerateWithState(token, current_state, ref processing_state, ref argument);

            TokenType tt = token.type;
            if (tt == TokenType.semi_colon)
                return true;

            // type | val | using
            var states = this.states;
            if (tt == TokenType.c__val_kw) // field
                states.Push(STATE_FIELD_TYPE);
            else if (tt == TokenType.type) // method
            {
                states.Push(STATE_METHOD_NAME);

                states.Push(E0_DEFAULT);
                return OnGenerateWithState(token, STATE_TYPE, ref processing_state, ref argument);
            }
            else // using (현재 무시)
            { // using id string ; (3개 더 무시)
                processing_state = PROC_SKIP | 3;
                m_handler.Warn(CIL_WMSG.I); // 현재 사용되지 않는 문법입니다
            }

            return true;
        }

        protected virtual bool OnGenerateWithState(Token token, uint current_state, ref int processing_state, ref object argument)
        {
            var states = this.states;

            TokenType tt = token.type;

            #region part 1 - field
            if (current_state == STATE_FIELD_TYPE)
            {
                PushTypeAuto(token);
                states.Push(STATE_FIELD_NAME);
            }
            else if (current_state == STATE_FIELD_NAME)
                OnDeclareField(stackTypes.Pop(), token.text);
            #endregion
            #region type
            else if (current_state == STATE_TYPE)
                return OnMakeType(token, tt, states.Pop());
            #endregion
            #region part 2 - method
            else if (current_state == STATE_METHOD_NAME)
            {
                tempString = token.text;
                states.Push(STATE_METHOD_PARAMs);
            }
            else if (current_state == STATE_METHOD_PARAMs)
                if (tt == TokenType.lparen || tt == TokenType.comma)
                    states.Push(STATE_METHOD_PARAMs); // get next token
                else if (tt == TokenType.rparen)
                    OnAddMethod();
                else
                {
                    states.Push(STATE_PARAM_NAME);

                    states.InnerState(STATE_TYPE, E0_DEFAULT);
                }
            else if (current_state == STATE_PARAM_NAME)
                OnAddParameter(new HashedString(token.text));
            #endregion
            #region part 3 - method body
            else if (current_state == STATE_METHODBODY)
                return OnMethodBody(token, tt, states.Pop(), ref processing_state, ref argument);
            #endregion

            return true;
        }

        protected virtual bool OnMethodBody(Token token, TokenType tokType, uint inner_state, ref int processing_state, ref object argument)
        {
            var states = this.states;

            if (inner_state == Z1_DEFAULT)
                switch (tokType) // 모든 시작가능한 state들의 집합
                {
                    case TokenType.type: // local declare
                    {
                        states.InnerState(STATE_METHODBODY, Z1_LOCAL_OPEN);
                        states.InnerState(STATE_TYPE, E0_DEFAULT);

                        processing_state = PROC_SYSTEM_NEXTSTATE;

                        return false;
                    }

                    case TokenType._if:
                    {
                        states.InnerState(STATE_METHODBODY, Z1_IF_EXPR);
                        states.InnerState(STATE_EXPRESSION, Q_DEFAULT);

                        break;
                    }
                }
            else if (inner_state == Z1_LOCAL_OPEN)
            {

            }
            else if (inner_state == Z1_IF_EXPR)
            {

            }

            return true;
        }

        #region method declare
        protected virtual void OnAddMethod()
        {
            HashedString methodname = new HashedString(tempString);
            Type[] types = stackTypes.PopAuto();

            ILMethodSignature signature = new ILMethodSignature(methodname, types);
            var methods = this.methods;

            if (methods.ContainsKey(signature))
            {
                m_handler.Fail(CIL_FMSG.F2);
                return;
            }

            MethodBuilder methodBuilder = tb.DefineMethod(tempString, methAttrb, stackTypes.Pop(), types);

            OnCreatedMethodBuilder(methodBuilder);

            methods.Add(signature, new ILMethodBuildState(methodBuilder));

            states.InnerState(STATE_METHODBODY, Z1_DEFAULT);
        }

        protected virtual void OnCreatedMethodBuilder(MethodBuilder methodBuilder)
        {
        }

        protected virtual void OnAddParameter(HashedString param_name)
        {
            var locOrParam = this.locOrParam;
            if (locOrParam.ContainsKey(param_name))
            {
                m_handler.Fail(CIL_FMSG.F1);
                return;
            }

            locOrParam.Add(param_name, ScopeVarInfo.Parameter(stackTypes.Pop(), locOrParam.Count));

            states.Push(STATE_METHOD_PARAMs);
        }
        #endregion

        #region type maker
        protected virtual bool OnMakeType(Token token, TokenType tokType, uint inner_state)
        {
            var states = this.states;
            switch (inner_state)
            {
                case E0_DEFAULT:
                    PushTypeAuto(token);
                    states.InnerState(STATE_TYPE, E0_GENERIC_OPEN);
                    break;

                case E0_GENERIC_OPEN:
                    if (tokType == TokenType.lbracket_angl) // <
                    {
                        stackTypes.MarkPoint();
                        states.InnerState(STATE_TYPE, E0_GENERIC_CLOSE);
                        states.InnerState(STATE_TYPE, E0_DEFAULT);
                        break;
                    }
                    else if (tokType == TokenType.comma)
                    {
                        states.InnerState(STATE_TYPE, E0_DEFAULT);
                        break;
                    }
                    else
                        goto case E0_ARRAY;

                case E0_GENERIC_CLOSE:
#if DEBUG
                    System.Diagnostics.Debug.Assert(tokType == TokenType.rbracket_angl);
#endif
                    MakeGenericTypeAuto();
                    break;

                case E0_ARRAY:
                    if (tokType == TokenType.lbracket_Sq)
                    {
                        MakeArrayAuto(); // end
                        break;
                    }

                    return false;
            }

            return true;
        }
        #endregion

        #region field declare & load
        protected virtual void OnDeclareField(Type field_type, string field_name)
        {
            HashedString key = new HashedString(field_name);
            if (fields.ContainsKey(key))
            {
                m_handler.Fail(CIL_FMSG.F1);
                return;
            }

            fields.Add(key, tb.DefineField(field_name, field_type, fldAttrb));
        }

        protected virtual void TryLoadField(string field_name)
        {
            HashedString key = new HashedString(field_name);
            if (fields.TryGetValue(key, out FieldBuilder result))
                codes.Add(new CILCode(OpCodes.Ldsfld, result));
            else
            {
                absflds.Add(codes.Count);
                codes.Add(new CILCode(CILCode.CCODE_LOAD_FIELD, field_name));
            }
        }
        #endregion
    }
}
