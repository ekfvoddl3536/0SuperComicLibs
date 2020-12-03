namespace SuperComicLib.CodeDesigner
{
    public static class CodeGeneratorStates
    {
        public const uint
            STATE_TYPE_BUILD = 0x10,
            STATE_CHECK_ID = 0x11,
            STATE_RETURN_EXPR = 0x12,
            STATE_END_RETURN = 0x13,
            STATE_DECL_METHOD = 0x30,
            STATE_METHODBODY = 0x36,
            // STATE_OPEN_BRANCH = 0x40,
            STATE_OPEN_IF = 0x41,
            STATE_OPEN_ELSE = 0x42,
            STATE_OPEN_ELIF = 0x43,
            STATE_OPEN_FOR = 0x44,
            STATE_OPEN_WHILE = 0x45,
            STATE_CLOSE_BRANCH = 0x50,
            STATE_CLOSE_ELIF = 0x51,
            STATE_EXPRESSION = 0x60,
            STATE_SKIP_PUSHTYPE = 0x61,
            STATE_IF_EXPR = 0x62,
            STATE_OPEN_ID = 0x400,
            STATE_OPEN_CALL = 0x410,
            STATE_OPEN_IDXR = 0x420,
            STATE_THIS_KW = 0x440,
            STATE_THIS_KW_ID = 0x441,
            // STATE_SKIP = 0x7EFF_FFFF,
            STATE_ARGUMENT = 0x8000_0000;
    }
}
