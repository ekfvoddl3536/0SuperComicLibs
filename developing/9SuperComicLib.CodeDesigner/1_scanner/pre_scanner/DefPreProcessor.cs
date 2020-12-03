using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SuperComicLib.IO;
using SuperComicLib.Runtime;

namespace SuperComicLib.CodeDesigner
{
    public class DefPreProcessor : PreProcessor
    {
        protected const int
            STATE_IF_OPEN = 0x10,
            STATE_IF_CLOSED = 0x11,
            STATE_COMMENT = 0x8000;

        protected IBuildAttribute buildAttrb;
        protected int current_comment;
        protected int before_comment;
        protected Stack<int> states;

        public DefPreProcessor() : this(null) { }

        public DefPreProcessor(IBuildAttribute buildAttrb)
        {
            this.buildAttrb = buildAttrb ?? NopBuildAttrb.Instance;
            states = new Stack<int>();
        }

        protected override void OnConvert(CStreamReader input, StreamWriter output)
        {
            states.Push(0);
            while (input.EndOfStream == false)
            {
                string line = input.TrimmedReadLine();
                if (line == null)
                    return;

                if (!OnCompilerAction(line, input, output))
                    OnNormalAction(line, input, output);
            }
        }

        protected virtual bool OnCompilerAction(string current, CStreamReader input, StreamWriter output)
        {
            if (current.StartsWith("//"))
                return false;

            int state = states.Peek();
            if (current.StartsWith("/*"))
                if (state == STATE_COMMENT)
                    current_comment++;
                else
                {
                    state = STATE_COMMENT;
                    before_comment = current_comment++;
                }
            else if (current.StartsWith("*/"))
            {
                if (--current_comment == before_comment)
                    state = 0;
            }
            else if (state < STATE_COMMENT && current[0] == '#') // compiler 지시어
            {
                OnParseCompilerCommand(state, current.Substring(1), input, output);
                return true;
            }

            return state == 0;
        }

        #region 나중에 쓸 예정
        protected virtual void OnParseCompilerCommand(int state, string command, CStreamReader input, StreamWriter output)
        {
            int idx = command.IndexOf(' ');
            if (idx >= 0) // has Argument
                OnCmdWithArg(state, command.Substring(0, idx).Trim(), command.Substring(idx + 1).Trim(), input, output);
            // endif는 인수가 없다
            else if (command == "endif")
                states.Pop();
            else if (command == "else" && state == STATE_IF_CLOSED)
            {
                states.Pop();
                states.Push(0);
            }
        }

        protected virtual void OnCmdWithArg(int state, string command, string argument, CStreamReader input, StreamWriter output)
        {
            if (command == "apidoc")
                buildAttrb.AddImplement(argument);

            // if (command == "define")
            //     conditional.AddDefine(argument);
            // else if (command == "if")
            // {
            //     if (state <= STATE_IF_OPEN)
            //         states.Push(
            //             conditional.IsTrue(argument)
            //             ? STATE_IF_OPEN
            //             : STATE_IF_CLOSED);
            // }
            // else if (command == "elif" && state == STATE_IF_CLOSED)
            //     if (conditional.IsTrue(argument))
            //     {
            //         states.Pop();
            //         states.Push(STATE_IF_OPEN);
            //     }
        }
        #endregion

        protected virtual void OnNormalAction(string current, CStreamReader input, StreamWriter output)
        {
            int idx = current.IndexOf("//");
            if (idx >= 0)
                output.Write(current.Substring(0, idx).Trim() + Environment.NewLine); // 끝 주석때문에 newline 필요

            idx = current.IndexOf("/*");
            if (idx >= 0)
            {
                int cnt = 1;
                int max = current.Length - 1;

                StringBuilder builder = new StringBuilder(max - 3);
                builder.Append(current.Substring(0, idx).Trim());

                int begin = 0;
                for (; idx < max; idx += 2)
                {
                    char f1 = current[idx];
                    char f2 = current[idx + 1];
                    if (f1 == '/' && f2 == '*')
                    {
                        builder.Append(current.Substring(begin, idx).Trim());
                        cnt++;
                    }
                    else if (f1 == '*' && f2 == '/')
                    {
                        cnt--;
                        if (cnt == 0)
                            begin = idx + 2;
                    }
                }

                string temp = builder.ToString();
                if (FnCHAR.IsNewLine(temp[builder.Length - 1]))
                    output.Write(temp);
                else
                    output.Write(temp + Environment.NewLine);
            }
            else
                output.Write(current);
        }

        protected override void Dispose(bool disposing)
        {
            if (buildAttrb != null)
            {
                buildAttrb = null;

                states.Clear();
                states = null;
            }
        }
    }
}
