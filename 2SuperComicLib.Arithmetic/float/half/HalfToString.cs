//using System.Text;

//namespace SuperComicLib.Arithmetic
//{
//    internal static class HalfToString
//    {
//        public static void Integer(StringBuilder sb, int exp, int man)
//        {
//            int i = man;
//            if (exp > 0)
//                i <<= exp;
//            else if (exp < -10)
//            {
//                sb.Append('0');
//                return;
//            }
//            else if (exp < 0)
//                i >>= -exp;

//            char[] vs = new char[5]; // 65504
//            int k = 0;
//            while (i != 0)
//            {
//                vs[k++] = (char)(i % 10 + '0');
//                i /= 10;
//            }

//            while (--k >= 0)
//                sb.Append(vs[k]);
//        }

//        public static void Mantissa(StringBuilder sb, int exp, int man)
//        {
//            int shift = 28 + exp; // 32 + exp - 4 -> (32 - 4) + exp

//            if (shift >= 28 || shift < 0)
//                return; // no mantissa

//            sb.Append('.');

//            StringBuilder sb2 = new StringBuilder();
//            uint m = (uint)man << shift;

//            int v = 4;
//            while (m != 0 && v > 0)
//            {
//                m = (m & 0x0FFF_FFFF) * 10;

//                uint digit = m >> 28;
//                sb2.Append((char)(digit + '0'));

//                if (digit != 0 || v != 3)
//                    v--;
//            }

//            v = sb2.Length - 1;
//            while (v > 0 && sb2[v] == '0')
//                v--;

//            for (exp = 0; exp <= v;)
//                sb.Append(sb2[exp++]);
//        }
//    }
//}
