//using SuperComicLib.Collections;
//using System;

//namespace SuperComicLib.CodeDesigner
//{
//    public static class DefaultScanAction
//    {
//        public static void IdentityToken(ref string text, int line, int row, IAddOnlyList<Token> list)
//        {
//            int x = 1;
//            for (; x < text.Length; x++)
//            {
//                char letter = text[x];
//                if (letter != '_' && !char.IsLetterOrDigit(letter))
//                    break;
//            }

//            string temp = text.Substring(0, x);
//            text = text.Substring(x);

//            list.Add(new Token(temp, TokenType.id, line, row));
//        }

//        private static void TypeToken(ref string text, int line, int row, IAddOnlyList<Token> list)
//        {
//            int x = 1;
//            for (; x < text.Length; x++)
//            {
//                char letter = text[x];
//                if (letter != '_' && !char.IsLetterOrDigit(letter))
//                    break;
//            }

//            if (x < text.Length)
//            {
//                char letter = text[x];
//                if (letter == '*')
//                {
//                    for (x++; x < text.Length; x++)
//                        if (text[x] != '*')
//                            break;
//                }
//                else if (letter == '[')
//                    for (int s = x % 2; x < text.Length; x++)
//                    {
//                        letter = text[x];
//                        if (s == x % 2)
//                        {
//                            if (letter == ']' || letter == '*')
//                                return;

//                            if (letter != '[')
//                                break;
//                        }
//                        else if (letter == '*' || letter == '[' || letter != ']')
//                            return;
//                    }
//            }

//            string temp = text.Substring(0, x);
//            text = text.Substring(x);

//            list.Add(new Token(temp, TokenType.type, line, row));
//        }

//        public static void TypeIdentityToken(ref string text, int line, int row, IAddOnlyList<Token> list)
//        {
//            int before = text.Length;
//            TypeToken(ref text, line, row, list);

//            text = text.Trim();
//            IdentityToken(ref text, line, row + before - text.Length, list);
//        }

//        public static void IntegerToken(ref string text, int line, int row, IAddOnlyList<Token> list)
//        {
//            int x = 1;
//            for (; x < text.Length; x++)
//            {
//                char c = char.ToLower(text[x]);
//                if (!char.IsDigit(text[x])
//                    && c != 'u'
//                    && c != 'l')
//                    break;
//            }

//            string temp = text.Substring(0, x);
//            text = text.Substring(x);

//            list.Add(
//                temp.EndsWith("ul")
//                ? new Token(temp.RemoveBack(2), TokenType.literal_int_u8, line, row)
//                : temp.EndsWith("l")
//                ? new Token(temp.RemoveBack(1), TokenType.literal_int_8, line, row)
//                : temp.EndsWith("u")
//                ? new Token(temp.RemoveBack(1), TokenType.literal_int_u4, line, row)
//                : new Token(temp, TokenType.literal_int_4, line, row));
//        }

//        public static void RealPointToken(ref string text, int line, int row, IAddOnlyList<Token> list)
//        {
//            int x = 1;
//            for (; x < text.Length; x++)
//            {
//                char c = char.ToLower(text[x]);
//                if (!char.IsDigit(c)
//                    && c != '.' && c != '+' && c != '-'
//                    && c != 'e' && c != 'd')
//                    break;
//            }

//            string temp = text.Substring(0, x);
//            text = text.Substring(x);

//            list.Add(
//                temp.EndsWith("d")
//                ? new Token(temp.RemoveBack(1), TokenType.literal_real_8, line, row)
//                : new Token(temp, TokenType.literal_real_4, line, row));
//        }

//        public static void ThisKwToken(ref string text, int line, int row, IAddOnlyList<Token> list)
//        {
//            text = text.Substring(5); // this.
//            list.Add(new Token("this", TokenType.this_kw, line, row));
//        }

//        public static void RefOrInKwToken(ref string text, int line, int row, IAddOnlyList<Token> list)
//        {
//            char c = text[0];
//            text = text.Substring(3);
        
//            list.Add(
//                c == 'r'
//                ? new Token("ref", TokenType.ref_kw, line, row)
//                : new Token("in", TokenType.in_kw, line, row));
//        }

//        public static void StringToken(ref string text, int line, int row, IAddOnlyList<Token> list)
//        {
//            int x = 1;
//            while (x < text.Length)
//            {
//                char c = text[x];
//                if (c == '"')
//                    break;
//                else if (c == '\\')
//                    x += 2;
//                else
//                    x++;
//            }

//            string temp = text.Substring(1, x - 1);
//            text = text.Substring(x + 1);

//            list.Add(new Token(temp, TokenType.literal_str, line, row));
//        }

//        public static void CastingToken(ref string text, int line, int row, IAddOnlyList<Token> list)
//        {
//            int idx = text.IndexOf(')');
//            string temp = text.Substring(1, idx - 1);

//            text = text.Substring(idx + 1);
//            list.Add(new Token(temp, TokenType.casting, line, row));
//        }
//    }
//}
