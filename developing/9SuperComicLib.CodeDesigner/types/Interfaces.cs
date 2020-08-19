using System;

namespace SuperComicLib.CodeDesigner
{
    public interface IExceptionHandler
    {
        int FailCount { get; }

        int WarnCount { get; }

        int MsgCount { get; }

        void Fail(string value);

        void Warn(string value);

        void Msg(string value);

        void Reset();
    }

    public interface IExpressMap : IDisposable
    {
        TokenType Terminal(string text);
        bool Terminal(string text, out TokenType result);
        int Nonterminal(string text);
    }

    public interface INode : IDisposable
    {
        INode Parent { get; set; }

        INode this[int index] { get; }

        INode C0 { get; }

        INode C1 { get; }

        INode C2 { get; }

        INode C3 { get; }

        int ChildCount { get; }

        void Add(INode node);

        Token GetToken();
    }
}
