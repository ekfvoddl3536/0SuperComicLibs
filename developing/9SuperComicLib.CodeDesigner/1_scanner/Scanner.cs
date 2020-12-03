#pragma warning disable IDE0046 // if 문을 삼항연산으로 바꾸는거
using SuperComicLib.IO;
using SuperComicLib.Text;
using System;
using System.IO;
using System.Text;

namespace SuperComicLib.CodeDesigner
{
    public class Scanner : IDisposable
    {
        protected internal IExceptionHandler handler;
        protected internal ITypeMap typeMap;
        protected internal StrKeywordTable ck_table;
        protected internal StrKeywordTable sym_table;

        public Scanner(IExceptionHandler handler, ITypeMap typeMap)
        {
            this.handler = handler ?? ExceptionHandlerFactory.Default;
            this.typeMap = typeMap ?? TypeTable.Instance;

            ck_table = LoadDefaultCharKeywordTable();
            sym_table = LoadDefaultSymbolTable();
        }

        public Scanner() : this(null, null) { }

        #region method
        public ITokenEnumerator FromFile(string path, Encoding enc)
        {
            if (enc == null)
                throw new ArgumentException(nameof(enc));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path))
                throw new FileNotFoundException("FILE NOT FOUND", path);
                
            return Internal_FromStream(File.OpenRead(path), enc, false);
        }

        public ITokenEnumerator FromText(string text) => 
            string.IsNullOrWhiteSpace(text)
            ? throw new ArgumentException("invalid", nameof(text))
            : Internal_FromStream(new StringStream(text, Encoding.Default), Encoding.Default, false);

        public ITokenEnumerator FromStream(Stream stream) => FromStream(stream, Encoding.Default, true);

        public ITokenEnumerator FromStream(Stream stream, Encoding enc) => FromStream(stream, enc, true);

        public ITokenEnumerator FromStream(Stream stream, Encoding enc, bool leaveOpen)
        {
            if (enc == null)
                throw new ArgumentNullException(nameof(enc));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead || stream.Position == stream.Length)
                throw new ArgumentException("invalid", nameof(stream));

            return Internal_FromStream(stream, enc, leaveOpen);
        }

        public ITokenEnumerator FromStream(PreProcessor pre_processor, Stream stream, Encoding enc, bool leaveOpen)
        {
            if (pre_processor == null)
                throw new ArgumentNullException(nameof(pre_processor));
            if (enc == null)
                throw new ArgumentNullException(nameof(enc));

            return new ScanActionEnumerator(
                pre_processor.Convert(stream, enc, leaveOpen),
                sym_table.Pair,
                ck_table.Pair,
                handler,
                typeMap);
        }

        private ITokenEnumerator Internal_FromStream(Stream stream, Encoding enc, bool leaveOpen) =>
            new ScanActionEnumerator(
                new CStreamReader(stream, enc, true, 1024, leaveOpen),
                sym_table.Pair, 
                ck_table.Pair,
                handler,
                typeMap);

        #endregion

        #region keywords & symbols
        protected virtual StrKeywordTable LoadDefaultCharKeywordTable() =>
            new StrKeywordTable(24)
            {
                { "this", TokenType.this_kw },
                // { "base", TokenType.base_kw },

                { "const", TokenType.const_kw },

                { "true", TokenType.literal_bool },
                { "false", TokenType.literal_bool },

                { "ret", TokenType._return },
                { "new", TokenType._new },
                { "default", TokenType.default_kw },

                { "if", TokenType._if },
                { "else", TokenType._else },
                { "for", TokenType._for },
                { "foreach", TokenType._foreach },
                { "while", TokenType._while },
                { "do", TokenType._do },

                { "goto", TokenType._goto },
                { "fixed", TokenType._fixed },
                { "continue", TokenType._continue },
                { "break", TokenType._break },

                { "ref", TokenType.ref_kw },
                { "in", TokenType.in_kw },

                // { "public", TokenType.access_limiter },
                // { "protected", TokenType.access_limiter },
                // { "private", TokenType.access_limiter },

                { "as", TokenType.as_kw },
                { "is", TokenType.is_kw },

                { "using", TokenType.d__include_kw },
                { "from", TokenType.d__from_kw },
            };

        protected virtual StrKeywordTable LoadDefaultSymbolTable() =>
            new StrKeywordTable(48)
            {
                { "[", TokenType.lbracket_Sq },
                { "]", TokenType.rbracket_Sq },
                { "{", TokenType.lbracket },
                { "}", TokenType.rbracket },
                { "(", TokenType.lparen },
                { ")", TokenType.rparen },

                { "<<=", TokenType.lshift_assign },
                { ">>=", TokenType.rshift_assign },

                { "*=", TokenType.mult_assign },
                { "/=", TokenType.div_assign },
                { "%=", TokenType.mod_assign },
                { "+=", TokenType.plus_assign },
                { "-=", TokenType.minus_assign },
                { "|=", TokenType.or_assign },
                { "^=", TokenType.xor_assign },
                { "&=", TokenType.and_assign },

                { "*", TokenType.multiple },
                { "/", TokenType.divide },
                { "%", TokenType.modular },
                { "+", TokenType.plus },
                { "-", TokenType.minus },
                { "&", TokenType.bit_and },
                { "^", TokenType.bit_xor },
                { "|", TokenType.bit_or },
                { "<<", TokenType.bit_lshift },
                { ">>", TokenType.bit_rshift },

                { "&&", TokenType.logic_AND },
                { "||", TokenType.logic_OR },

                { "~", TokenType.unary_bitwise },
                { "!", TokenType.unary_bitnot },

                { "++", TokenType.plusplus_assign },
                { "--", TokenType.minusminus_assign },

                { "==", TokenType.comp_Equal },
                { "!=", TokenType.comp_NotEqual },
                { ">=", TokenType.comp_GreatOrEqual },
                { "<=", TokenType.comp_LessOrEqual },
                { "<", TokenType.lbracket_angl },
                { ">", TokenType.rbracket_angl },

                { "=", TokenType.assign },

                { ".", TokenType.period },
                { ",", TokenType.comma },
                { ":", TokenType.colon },
                { ";", TokenType.semi_colon }
            };
        #endregion

        #region serialize & deserialize
        protected internal virtual void OnSerialize(BinaryWriter writer) { }
        protected internal virtual void OnDeserialize(BinaryReader reader) { }
        #endregion
        
        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (handler != null)
            {
                handler = null;
                typeMap = null;

                sym_table.Dispose();
                sym_table = null;

                ck_table.Dispose();
                ck_table = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
