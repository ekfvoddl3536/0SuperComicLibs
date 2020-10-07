using System;
using SuperComicLib.IO;

namespace SuperComicLib.CodeDesigner
{
    public abstract class Conditional : IDisposable
    {
        protected IExceptionHandler m_handler;

        protected Conditional(IExceptionHandler handler) => m_handler = handler;

        public bool AddDefine(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
                return false;

            string[] vs = argument.Split(Constants.sp, 2);
            string name;
            return
                IsValidID(name = vs[0]) &&
                name != "true" &&
                name != "false" && 
                OnAddDefine(new HashedString(name), vs);
        }

        protected abstract bool OnAddDefine(HashedString name, string[] value);

        public abstract bool IsTrue(string argument);

        protected virtual bool TryPushOperator(CStreamReader reader, int read)
        {
            if (read != '(' &&
                read != ')' &&
                read != '+' &&
                read != '-' &&
                read != '*' &&
                read != '/' &&
                read != '%')
            {
                int peek = reader.Peek();
                if (read == '|')
                {
                    if (peek != '|')
                        return false;
                }
                else if (read == '&')
                {
                    if (peek != '&')
                        return false;
                }
                else if (read == '=')
                {
                    if (peek != '=')
                        return false;
                }
                else if ((read == '!' || read == '<' || read == '>') && peek == '=')
                    reader.Move();
            }

            return true;
        }

        protected abstract bool TryGetValue(HashedString name, out int result);

        protected abstract bool Contains(HashedString name);

        #region dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => m_handler = null;
        #endregion

        #region static methods
        protected static bool IsValidID(string v) => IsValidID(v, v.Length);

        protected static bool IsValidID(string v, int len)
        {
            if (IsInvalidStartCHAR(v[0]))
                return false;

            for (int x = 1; x < len; x++)
                if (IsInvalidCHAR(v[x]))
                    return false;

            return true;
        }

        protected static bool IsInvalidStartCHAR(char c) => !char.IsLetter(c) && c != '_';

        protected static bool IsInvalidCHAR(char c) => !char.IsLetterOrDigit(c) && c != '_';
        #endregion
    }
}
