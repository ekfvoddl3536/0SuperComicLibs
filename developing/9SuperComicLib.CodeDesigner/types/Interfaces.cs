using System;
using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    #region public
    public interface IBuildAttribute
    {
        void AddImplement(string uri);
    }

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

    public interface ITypeDesc
    {
        Type NormalType { get; }

        Type GenericType(params Type[] types);
    }

    public interface ITypeMap
    {
        void Add<T>(string name);
        void Add(string name, Type value);

        bool TryGet(string name, out ITypeDesc result);
        ITypeDesc Get(string name);

        IEnumerable<ITypeDesc> ToArray();

        bool Contains(string name);

        bool IsSynchronized { get; }
        int Count { get; }
    }

    public interface INodeEnumerator : IEnumerator<INode>
    {
        int TokenCount { get; }
        int Count { get; }
        int Index { get; }

        INode Parent { get; }

        INode Peek();

        INode Peek(int idx);
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
