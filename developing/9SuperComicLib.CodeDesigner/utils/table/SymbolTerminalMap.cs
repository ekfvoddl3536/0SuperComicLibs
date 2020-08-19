using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    using static TokenType;
    public sealed class SymbolTerminalMap : ExpressMap
    {
        private static readonly IExpressMap inst = new SymbolTerminalMap();

        public static IExpressMap Default => inst;

        private SymbolTerminalMap() { }

        protected override Dictionary<string, TokenType> Initalize() =>
            new Dictionary<string, TokenType>
            {
                { "id", id },
                { "type", type },
                
                { "==", comp_Equal },
                { "!=", comp_NotEqual },
                { "<",  comp_Lesser },
                { "<=", comp_LessOrEqual },
                { ">",  comp_Greater },
                { ">=", comp_GreatOrEqual },
                { "=",  assign },
                { "+=", plus_assign },
                { "-=", minus_assign },
                { "*=", mult_assign },
                { "/=", div_assign },
                { "%=", mod_assign },
                { "|=", logic_or_assign },
                { "^=", logic_xor_assign },
                { "&=", logic_and_assign },
                { "<<=", lshift_assign },
                { ">>=", rshift_assign },

                { "++", plusplus_assign },
                { "--", minusminus_assign },

                { "!", unary_logic_not },
                { "~", unary_bitwise },

                { "{", lbracket },
                { "}", rbracket },
                { "[", lbracket_Sq },
                { "]", rbracket_Sq },
                { "(", lparen },
                { ")", rparen },

                { "+", plus },
                { "-", minus },
                { "/", divide },
                { "%", modular },
                { "*", multiple },
                { "'|'", logic_or },
                { "^", logic_xor },
                { "&", logic_and },
                { "<<", bit_lshift },
                { ">>", bit_rshift },

                { ",", comma },
                { ":", colon },
                { ".", period },
                { ";", semi_colon },

                { "if", _if },
                { "else", _else },
                { "for", _for },
                { "foreach", _foreach },
                { "break", _break },
                { "continue", _continue },
                { "goto", _goto },
                { "fixed", _fixed },
                { "new", _new },
                { "ret", _return },

                { "global", access_limiter },
                { "public", access_limiter },
                { "protected", access_limiter },
                { "private", access_limiter },

                // { "value", literal_bool },
                { "bool", literal_bool },
                { "int", literal_int_4 },
                { "long", literal_int_8 },
                { "float", literal_real_4 },
                { "double", literal_real_8 },
                { "string", literal_str },

                { "ref", ref_kw },
                { "in", in_kw },
                { "this", this_kw },
                { "base", base_kw },
                { "def", default_kw },

                { "cast", casting },

                { "eol", EOL }
            };
    }
}
