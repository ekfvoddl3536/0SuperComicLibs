namespace SuperComicLib.CodeDesigner
{
    public enum TokenType
    {
        None,

        id,
        type,

        comp_Equal,         // ==
        comp_NotEqual,      // !=
        comp_Lesser,        // <
        comp_LessOrEqual,   // <=
        comp_Greater,       // >
        comp_GreatOrEqual,  // >=

        assign,
        plus_assign,
        minus_assign,
        mult_assign,
        div_assign,
        mod_assign,
        logic_or_assign,
        logic_xor_assign,
        logic_and_assign,
        lshift_assign,
        rshift_assign,

        // 후위 증감/가감 연산자
        plusplus_assign,
        minusminus_assign,

        unary_logic_not,
        unary_bitwise,
        // unary_plusplus,
        // unary_minusminus,

        lbracket,
        rbracket,
        lbracket_Sq,
        rbracket_Sq,
        lparen,
        rparen,

        plus,
        minus,
        divide,
        modular,
        multiple,
        logic_or,
        logic_xor,
        logic_and,
        bit_lshift,
        bit_rshift,

        comma,
        colon,
        period,
        semi_colon,

        _if,
        _else,
        _for,
        _foreach,
        // _while,
        // _do,
        _break,
        _continue,
        _goto,
        _fixed,
        _new,
        _return,

        access_limiter, // public protected private ....

        // -1 ~ 8               => ldc.i4.<x>
        // -128 ~ -2, 9 ~ 127   => ldc.i4.s <x>
        // other                => ldc.i4 <x>
        literal_bool,

        // literal_int_i,
        // literal_int_i2,
        literal_int_4,
        literal_int_8,

        // literal_int_u,
        // literal_int_u2,
        // literal_int_u4, // parse uint -> cast to int
        // literal_int_u8, // parse ulong -> cast to long

        literal_real_4,
        literal_real_8,

        literal_str,

        ref_kw,
        in_kw,
        this_kw,
        base_kw,
        default_kw,

        casting,

        EOL = short.MaxValue, // End Of Line
    }
}
