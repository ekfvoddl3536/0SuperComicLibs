using System;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    #region public
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

    public interface ISymbolMap : IDisposable
    {
        TokenType Terminal(string text);
        bool Terminal(string text, out TokenType result);
        int NonTerminal(string text);

        bool ContainsNT(string text);

        void BeginParseGrammar(List<Range> list);
        void EndParseGrammar();
    }

    public interface ITypeMap
    {
        void Add<T>(string name);
        void Add(string name, Type value);

        bool TryGet(string name, out Type result);
        Type Get(string name);

        IEnumerable<Type> ToArray();

        bool Contains(string name);

        bool IsSynchronized { get; }
        int Count { get; }
    }

    public interface INodeEnumerator : IEnumerator<INode>
    {
        int TokenCount { get; }
        int Count { get; }
        int Index { get; }

        INode Peek();

        INode Peek(int idx);

        int DeepCount(int limit);
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

        Token GetToken();

        INodeEnumerator GetEnumerator();

        void Add(INode node);

        int DeepCount(int limit, int find);
    }

    public interface ITokenEnumerator : IEnumerator<Token>
    {
        bool IsEnd { get; }
    }
    #endregion

    #region internal
    internal interface IRuntimeTypeMapEX : ITypeMap
    {
        void Dispose();
    }
    #endregion
}
