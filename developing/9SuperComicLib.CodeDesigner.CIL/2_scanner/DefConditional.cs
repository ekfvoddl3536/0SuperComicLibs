//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SuperComicLib.Collections;
//using SuperComicLib.Numerics;
//using SuperComicLib.Text;

//namespace SuperComicLib.CodeDesigner
//{
//    public class DefConditional : Conditional
//    {
//        protected Scanner m_scanner;
//        protected LookaheadStack<ConditionalValue> m_values;
//        protected LookaheadStack<TokenType> m_opers;
//        protected Dictionary<HashedString, ConditionalValue> m_keys;
//        protected int parens;

//        public DefConditional(IExceptionHandler handler) : base(handler)
//        {
//            m_scanner = new Scanner(handler, NopTypeMap.Default);
//            m_values = new LookaheadStack<ConditionalValue>(32);
//            m_opers = new LookaheadStack<TokenType>(16);
//        }

//        public override bool IsTrue(string argument)
//        {
//            if (string.IsNullOrWhiteSpace(argument))
//            {
//                m_handler.Fail(CIL_FMSG.F1);
//                return false;
//            }

//            ITokenEnumerator tokens = m_scanner.FromStream(new StringStream(argument, Encoding.UTF8), Encoding.UTF8, false);

//            int p = 0;
//            Token v;
//            while (tokens.MoveNext() && p != -1)
//                p = ParsingWithArg(v = tokens.Current, v.type, p);

//            OnEndParsing(tokens.IsEnd, p);
//            return m_handler.FailCount == 0;
//        }

//        protected virtual void OnEndParsing(bool isEnd, int lastArg)
//        {
//            if (parens != 0)
//            {
//                m_handler.Fail(CIL_FMSG.F2);
//                return;
//            }
//            else if (isEnd == false)
//                return;
//        }

//        /// <summary>
//        /// default -> return <a cref="int">0</a><para/>
//        /// break -> return <a cref="int">-1</a><para/>
//        /// throw exception -> refer to <a cref="Conditional.m_handler">m_handler</a> field and write fail message using <see cref="IExceptionHandler.Fail(string)"/> method
//        /// </summary>
//        protected virtual int ParsingWithArg(Token token, TokenType type, int argument)
//        {
//            ConditionalValue result;
//            if (type == TokenType.lparen)
//            {
//                m_opers.Push(TokenType.lparen);
//                parens++;
//            }
//            else if (type == TokenType.rparen)
//            {
//                m_opers.Push(TokenType.rparen);
//                parens--;
//            }
//            else if (type >= TokenType.literal_bool && type <= TokenType.literal_real_8)
//                if (ConditionalValue.TryParse(token.text, out result))
//                    m_values.Push(result);
//                else
//                {
//                    m_handler.Fail(CIL_FMSG.F1);
//                    return -1;
//                }
//            else if (type == TokenType.id)
//                if (m_keys.TryGetValue(new HashedString(token.text), out result))
//                    m_values.Push(result);
//                else
//                    m_values.Push(ConditionalValue.Empty);
//            else if (type >= TokenType.unary_bitnot && type <= TokenType.logic_OR)
//                m_opers.Push(type);
//            else
//            {
//                m_handler.Fail(CIL_FMSG.F2);
//                return -1;
//            }

//            return 0;
//        }

//        protected override bool Contains(HashedString name) => 
//            m_keys.ContainsKey(name);

//        protected override bool OnAddDefine(HashedString name, string[] vs)
//        {
//            if (m_keys.ContainsKey(name))
//            {
//                m_handler.Fail(CIL_FMSG.F2);
//                return false;
//            }

//            if (vs.Length == 2)
//                if (ConditionalValue.TryParse(vs[1], out ConditionalValue result))
//                    m_keys.Add(name, result);
//                else
//                {
//                    m_handler.Fail(CIL_FMSG.F1);
//                    return false;
//                }
//            else
//                m_keys.Add(name, ConditionalValue.TRUE);

//            return true;
//        }

//        protected override bool TryGetValue(HashedString name, out ConditionalValue result) =>
//            m_keys.TryGetValue(name, out result);

//        #region disposable
//        protected override void Dispose(bool disposing)
//        {
//            // if (parser != null)
//            // {
//            //     parser.Dispose();
//            //     parser = null;
//            // }
//            base.Dispose(disposing);
//        }
//        #endregion
//    }
//}
