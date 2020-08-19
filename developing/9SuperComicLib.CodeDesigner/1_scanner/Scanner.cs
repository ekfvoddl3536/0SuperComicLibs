#pragma warning disable IDE0046 // if 문을 삼항연산으로 바꾸는거
using SuperComicLib.Collections;
using SuperComicLib.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SuperComicLib.CodeDesigner
{
    public class Scanner : IDisposable
    {
        protected IExceptionHandler handler;
        protected RegexPatternPair patterns;
        protected StrKeywordTable ck_table;
        protected StrKeywordTable sym_table;

        public Scanner(IExceptionHandler handler)
        {
            this.handler = handler ?? ExceptionHandlerFactory.Default;
            // patterns = LoadDefaultRegexPatterns();
            ck_table = LoadDefaultCharKeywordTable();
            sym_table = LoadDefaultSymbolTable();
        }
        public Scanner() : this(null) { }

        #region debugging
        public IEnumerable<Token> FromFile(string path, Encoding enc)
        {
            if (enc == null)
                throw new ArgumentException(nameof(enc));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path))
                throw new FileNotFoundException("FILE NOT FOUND", path);
                
            return Internal_FromStream(File.OpenRead(path), enc, false);
        }

        public IEnumerable<Token> FromText(string text) => 
            string.IsNullOrWhiteSpace(text)
            ? throw new ArgumentException("invalid", nameof(text))
            : Internal_FromStream(new StringStream(text, Encoding.Default), Encoding.Default, false);

        public IEnumerable<Token> FromStream(Stream stream) => FromStream(stream, Encoding.Default, true);

        public IEnumerable<Token> FromStream(Stream stream, Encoding enc) => FromStream(stream, enc, true);

        public IEnumerable<Token> FromStream(Stream stream, Encoding enc, bool leaveOpen)
        {
            if (enc == null)
                throw new ArgumentNullException(nameof(enc));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead || stream.Position == stream.Length)
                throw new ArgumentException("invalid", nameof(stream));

            return Internal_FromStream(stream, enc, leaveOpen);
        }

        private IEnumerable<Token> Internal_FromStream(Stream stream, Encoding enc, bool leaveOpen) =>
            new ScanActionEnumerator(
                new StreamReader(stream, enc, true, 1024, leaveOpen),
                sym_table.Pair, 
                ck_table.Pair,
                handler);

        // private IEnumerable<Token> Internal_FromStream(Stream stream, Encoding enc, bool leaveOpen)
        // {
        //     List<Token> tokens = new List<Token>();
        //     StreamReader reader = new StreamReader(stream, enc, true, 1024, leaveOpen);
        // 
        //     int line = 0;
        // 
        // loop:
        //     string now = reader.MoveNext(ref line);
        //     if (now != null)
        //     {
        //         OnReadLine(now, line, tokens);
        // 
        //         // End Of Line
        //         tokens.Add(new Token(string.Empty, TokenType.EOL, line, now.Length));
        //         goto loop;
        //     }
        // 
        //     reader.Close();
        //     return tokens.ToArray();
        // }
        #endregion

        // protected virtual void OnReadLine(string now, int line, List<Token> tokens)
        // {
        //     
        // }

        protected virtual StrKeywordTable LoadDefaultCharKeywordTable() =>
            new StrKeywordTable(16)
            {
                { "ret", TokenType._return },
                { "new", TokenType._new },
                { "default", TokenType.default_kw },
                { "true", TokenType.literal_bool },
                { "false", TokenType.literal_bool },
                { "if", TokenType._if },
                { "else", TokenType._else },
                { "foreach", TokenType._foreach },
                { "for", TokenType._for },
                { "goto", TokenType._goto },
                { "fixed", TokenType._fixed },
                { "ref", TokenType.ref_kw },
                { "in", TokenType.in_kw }
            };

        // protected virtual RegexPatternPair LoadDefaultRegexPatterns() =>
        //     new RegexPatternPair(10)
        //     {
        //         { ScanRegexPatterns.refOrIn_pattern, DefaultScanAction.RefOrInKwToken },
        //         { ScanRegexPatterns.this_pattern, DefaultScanAction.ThisKwToken },
        //         { ScanRegexPatterns.typeidentity_pattern, DefaultScanAction.TypeIdentityToken },
        //         // { ScanRegexPatterns.type_pattern, DefaultScanAction.TypeToken },
        //         { ScanRegexPatterns.identity_pattern, DefaultScanAction.IdentityToken },
        //         { ScanRegexPatterns.floating_pattern, DefaultScanAction.RealPointToken },
        //         { ScanRegexPatterns.integer_pattern, DefaultScanAction.IntegerToken },
        //         { ScanRegexPatterns.string_pattern, DefaultScanAction.StringToken },
        //         { ScanRegexPatterns.casting_pattern, DefaultScanAction.CastingToken }
        //     };

        protected virtual StrKeywordTable LoadDefaultSymbolTable() =>
            new StrKeywordTable(40)
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
                { "|=", TokenType.logic_or_assign },
                { "^=", TokenType.logic_xor_assign },
                { "&=", TokenType.logic_and_assign },

                { "*", TokenType.multiple },
                { "/", TokenType.divide },
                { "%", TokenType.modular },
                { "+", TokenType.plus },
                { "-", TokenType.minus },
                { "&", TokenType.logic_and },
                { "^", TokenType.logic_xor },
                { "|", TokenType.logic_or },
                { "<<", TokenType.bit_lshift },
                { ">>", TokenType.bit_rshift },

                { "==", TokenType.comp_Equal },
                { "!=", TokenType.comp_NotEqual },
                { ">=", TokenType.comp_GreatOrEqual },
                { "<=", TokenType.comp_LessOrEqual },
                { "<", TokenType.comp_Lesser },
                { ">", TokenType.comp_Greater },

                { "=", TokenType.assign },

                { ".", TokenType.period },
                { ",", TokenType.comma },
                { ":", TokenType.colon },
                { ";", TokenType.semi_colon }
            };

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (handler != null)
            {
                handler = null;

                patterns.Dispose();
                patterns = null;

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
