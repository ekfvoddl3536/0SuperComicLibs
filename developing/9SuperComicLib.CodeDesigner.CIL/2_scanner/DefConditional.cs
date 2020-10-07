using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperComicLib.Text;

namespace SuperComicLib.CodeDesigner
{
    public class DefConditional : Conditional
    {
        private Scanner m_scanner;

        public DefConditional(IExceptionHandler handler) : base(handler) =>
            m_scanner = new Scanner(handler, NopTypeMap.Default);

        public override bool IsTrue(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                m_handler.Fail(CIL_FMSG.F1);
                return false;
            }

            ITokenEnumerator tokens = m_scanner.FromStream(new StringStream(argument, Encoding.UTF8), Encoding.UTF8, false);
            bool result = SimpleParsingTokens(tokens);
            tokens.Dispose();

            return result;
        }

        protected virtual bool SimpleParsingTokens(ITokenEnumerator tokens)
        {
            throw new NotImplementedException();
        }

        protected override bool Contains(HashedString name)
        {
            throw new NotImplementedException();
        }

        protected override bool OnAddDefine(HashedString name, string[] vs)
        {
            throw new NotImplementedException();
        }

        protected override bool TryGetValue(HashedString name, out int result)
        {
            throw new NotImplementedException();
        }

        #region disposable
        protected override void Dispose(bool disposing)
        {
            // if (parser != null)
            // {
            //     parser.Dispose();
            //     parser = null;
            // }
            base.Dispose(disposing);
        }
        #endregion
    }
}
