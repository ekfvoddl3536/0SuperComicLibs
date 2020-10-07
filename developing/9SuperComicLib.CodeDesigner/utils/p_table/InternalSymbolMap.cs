using System.Collections.Generic;

namespace SuperComicLib.CodeDesigner
{
    using static TokenType;
    internal class InternalSymbolMap : SymbolMapBase
    {
        internal InternalSymbolMap() { }

        protected override Dictionary<string, TokenType> Initalize() =>
            new Dictionary<string, TokenType>(80)
            {
                { "id", id },
                { "type", type },
                { "type_arr", type_array },
                // { "type_ptr", type_pointer },
                
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
                { "|=", or_assign },
                { "^=", xor_assign },
                { "&=", and_assign },
                { "<<=", lshift_assign },
                { ">>=", rshift_assign },

                { "++", plusplus_assign },
                { "--", minusminus_assign },

                { "!", unary_bitnot },
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
                { "|", bit_or },
                { "^", bit_xor },
                { "&", bit_and },
                { "<<", bit_lshift },
                { ">>", bit_rshift },

                { "&&", logic_AND },
                { "||", logic_OR },

                { ",", comma },
                { ":", colon },
                { ".", period },
                { ";", semi_colon },

                { "if", _if },
                { "else", _else },
                { "for", _for },
                { "foreach", _foreach },
                { "while", _while },
                { "do", _do },
                { "break", _break },
                { "continue", _continue },
                { "goto", _goto },
                { "fixed", _fixed },
                { "new", _new },
                { "ret", _return },

                { "access", access_limiter },
                // { "public", access_limiter },
                // { "protected", access_limiter },
                // { "private", access_limiter },

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

                { "as", as_kw },
                { "const", const_kw },

                { "using", d__include_kw },
                { "from", d__from_kw },

                { "eol", EOL }
            };
    }
}
