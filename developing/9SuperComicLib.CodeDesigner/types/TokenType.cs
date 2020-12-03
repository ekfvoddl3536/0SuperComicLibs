namespace SuperComicLib.CodeDesigner
{
    public enum TokenType
    {
        None,

        id,
        type,
        // nested_type,
        // type_pointer,
        type_array,

        assign,
        plus_assign,
        minus_assign,
        mult_assign,
        div_assign,
        mod_assign,
        or_assign,
        xor_assign,
        and_assign,
        lshift_assign,
        rshift_assign,

        // 후위 증감/가감 연산자
        plusplus_assign,
        minusminus_assign,

        lbracket,
        rbracket,
        lbracket_Sq,
        rbracket_Sq,
        lparen,
        rparen,

        comp_Equal,         // ==
        comp_NotEqual,      // !=
        comp_LessOrEqual,   // <=
        comp_GreatOrEqual,  // >=
        lbracket_angl,        // <
        rbracket_angl,       // >

        unary_bitnot,
        unary_bitwise,
        // unary_plusplus,
        // unary_minusminus,

        plus,
        minus,
        divide,
        modular,
        multiple,
        bit_or,
        bit_xor,
        bit_and,
        bit_lshift,
        bit_rshift,

        logic_AND,
        logic_OR,

        comma,
        colon,
        period,
        semi_colon,
        // atsign,

        _if,
        _else,
        _for,
        _foreach,
        // _loop,
        _while,
        _do,
        _break,
        _continue,
        _goto,
        _fixed,
        _new,
        _return,

        access_limiter, // public protected private ....

        literal_bool,

        literal_int_4,
        literal_int_8,

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

        const_kw,
        as_kw,
        is_kw,

        d__include_kw = 0x100,
        d__from_kw,

        c__class_kw,
        c__final_kw,

        EOL = short.MaxValue, // End Of Line
    }
}
