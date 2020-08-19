namespace SuperComicLib.CodeDesigner
{
    public static class ScanRegexPatterns
    {
        public const string m_idpt = @"(_|[^\p{P}\p{S}\p{C}\p{N}])+(_|[^\p{P}\p{S}\p{C}\p{No}])*";
        public const string m_type = m_idpt + @"(\[\]|\*)*";

        public const string identity_pattern = "^" + m_idpt;
        public const string type_pattern = "^" + m_type;
        public const string typeidentity_pattern = "^" + m_type + @"\s+" + m_idpt;
        public const string integer_pattern = @"^[+-]?\d+";
        public const string floating_pattern = @"^[+-]?\d*[\.]\d+[eE]?[-+]?\d+";
        public const string string_pattern = @"^"".*""";

        public const string this_pattern = @"^this\.";

        public const string refOrIn_pattern = @"^(ref|in)\s+";

        public const string casting_pattern = @"^\(\s*" + m_type + @"\s*\)[^\.]";
    }
}
