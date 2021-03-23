namespace SuperComicLib.CodeDesigner
{
    public static class CodeGeneratorStates
    {
        public const uint
            STATE_FIELD_TYPE = 1,
            STATE_FIELD_NAME = 2,
            STATE_TYPE = 3,
            STATE_METHOD_NAME = 0x10,
            STATE_METHOD_PARAMs = 0x11,
            STATE_PARAM_NAME = 0x12,
            STATE_METHODBODY = 0x100,
            STATE_EXPRESSION = 0x200;

        public const uint
            E0_DEFAULT = 0,
            E0_GENERIC_OPEN = 1,
            E0_GENERIC_CLOSE = 2,
            E0_ARRAY = 3,
            E0_POINTER = 4;

        public const uint
            Z1_DEFAULT = 0,
            Z1_LOCAL_OPEN = 1,
            Z1_IF_EXPR = 2;

        public const uint
            Q_DEFAULT = 0;

        public const int
            PROC_SKIP = 0x1000; // ~ 0xFFF

        public const int
            PROC_SYSTEM_NEXTSTATE = 0x2000;
    }
}
