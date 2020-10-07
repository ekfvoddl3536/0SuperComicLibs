using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner.IO
{
    public static class SaveLoader
    {
        #region basic
        private static byte[] S(Type v) => v.GUID.ToByteArray();

        private static T D<T>(BinaryReader r) where T : class =>
            FormatterServices.GetUninitializedObject(Type.GetTypeFromCLSID(new Guid(r.ReadBytes(16)))) as T;
        #endregion

        #region grammar
        #region s
        public static void WriteGrammar(BinaryWriter writer, Grammar grammar)
        {
            WriteGItems(grammar.m_items, writer);
            WriteNonts(grammar.m_nonterminals, writer);
            writer.Write(grammar.startIdx);
            // shortranges (buffer)
            // st 먼저
            // d1 길이
            // gitem은 2바이트 produce (압축됨 index) 랑 2바이트 index만 기록 => 6 바이트 크기임
            //  d1 길이 * 4 길이만큼 buffer 먼저
            // d2 길이
            // 8바이트 (압축 4바이트)
            //  d2 길이 * 4 길이만큼 buffer 먼저
        }

        private static void WriteGItems(GItem[] vs, BinaryWriter wr)
        {
            // produce = 압축 2바이트
            // expressint = 길이 1바이트 + 4 * N 바이트 (압축 불가능)
            int max = vs.Length;
            wr.Write((ushort)max);
            for (int x = 0; x < max; x++)
            {
                GItem n = vs[x];
                int emax;
                wr.Write(n.produce);
                wr.Write((byte)(emax = n.express.Length));

                for (int z = 0; z < emax; z++)
                    wr.Write(n.express[z]);
            }
        }

        private static void WriteNonts(Range[] vs, BinaryWriter wr)
        {
            int max = vs.Length;
            wr.Write((ushort)max);
            for (int x = 0; x < max; x++)
            {
                Range n = vs[x];
                wr.Write((ushort)n.start);
                wr.Write((ushort)n.end);
            }
        }
        #endregion

        #region d
        public static Grammar ReadGrammar(BinaryReader reader)
        {
            Grammar v = new Grammar();

            v.m_items = ReadGItems(reader);
            v.m_nonterminals = ReadNonts(reader);
            v.startIdx = reader.ReadInt32();

            return v;
        }

        private static GItem[] ReadGItems(BinaryReader r)
        {
            int length = r.ReadUInt16();
            GItem[] vs = new GItem[length];

            for (int x = 0; x < length; x++)
            {
                int p = r.ReadInt32();

                int len2 = r.ReadByte();
                int[] ex = new int[len2];
                for (int z = 0; z < len2; z++)
                    ex[z] = r.ReadInt32();

                vs[x] = new GItem(p.ToNonterminal(), new ExpressInt(ex));
            }

            return vs;
        }

        private static Range[] ReadNonts(BinaryReader r)
        {
            int length = r.ReadUInt16();
            Range[] vs = new Range[length];

            for (int x = 0; x < length; x++)
                vs[x] = new Range(r.ReadUInt16(), r.ReadUInt16());

            return vs;
        }
        #endregion
        #endregion

        #region lalrparser
        #region s
        public static void Serialize(BinaryWriter writer, LALRParser parser, bool includeGrammar)
        {
            if (!parser.readyToParse)
                throw new InvalidOperationException();

            writer.Write(S(parser.GetType()));
            writer.Write((byte)includeGrammar.ToInt());
            if (includeGrammar)
                WriteGrammar(writer, parser.m_grammar);

            writer.Write((byte)parser.leaveOpen.ToInt());
            writer.Write(parser.threadLimit);

            WriteGTB(parser.gtb, writer);
            WriteATB(parser.atb, writer);

            parser.OnSerialize(writer);
        }

        internal static void WriteGTB(Map<int>[] gtb, BinaryWriter wr)
        {
            int max = gtb.Length;
            wr.Write(max);

            for (int x = 0; x < max; x++)
            {
                Map<int> n = gtb[x];

                int max2 = n.Count;
                wr.Write((ushort)max2); // 6만개 넘을 확률이 거의 없음

                IEnumerator<KeyValuePair<int, int>> iter = n.GetEnumerator();
                while (iter.MoveNext())
                {
                    KeyValuePair<int, int> kv = iter.Current;
                    wr.Write(kv.Key);
                    wr.Write(kv.Value);
                }

                iter.Dispose();
            }
        }

        internal static void WriteATB(Map<TableItem>[] atb, BinaryWriter wr)
        {
            int max = atb.Length;
            wr.Write(max);

            for (int x = 0; x < max; x++)
            {
                Map<TableItem> n = atb[x];

                int max2 = n.Count;
                wr.Write((ushort)max2);

                IEnumerator<KeyValuePair<int, TableItem>> iter = n.GetEnumerator();
                while (iter.MoveNext())
                {
                    KeyValuePair<int, TableItem> kv = iter.Current;
                    wr.Write(kv.Key);

                    TableItem temp = kv.Value;
                    wr.Write(temp.actType);
                    wr.Write(temp.nextstate);
                }

                iter.Dispose();
            }
        }
        #endregion

        #region d
        public static LALRParser DeserializeLALR(BinaryReader reader, Grammar grammar, IExceptionHandler handler)
        {
            if (!(D<LALRParser>(reader) is LALRParser result))
                return null;

            if (grammar == null && reader.ReadBoolean())
                result.m_grammar = ReadGrammar(reader);
            else
                result.m_grammar = grammar;

            result.m_handler = handler ?? ExceptionHandlerFactory.Default;

            result.leaveOpen = reader.ReadBoolean();
            result.threadLimit = reader.ReadInt32();

            result.gtb = ReadGTB(reader);
            result.atb = ReadATB(reader);

            result.readyToParse = true;

            result.OnDeserialize(reader);

            return result;
        }

        internal static Map<int>[] ReadGTB(BinaryReader r)
        {
            int max = r.ReadInt32();
            Map<int>[] vs = new Map<int>[max];
            for (int x = 0; x < max; x++)
            {
                int cnt = r.ReadUInt16();
                Map<int> n = new Map<int>(cnt);

                while (--cnt >= 0)
                    n.Add(r.ReadInt32(), r.ReadInt32());

                vs[x] = n;
            }

            return vs;
        }

        internal static Map<TableItem>[] ReadATB(BinaryReader r)
        {
            int max = r.ReadInt32();
            Map<TableItem>[] vs = new Map<TableItem>[max];
            for (int x = 0; x < max; x++)
            {
                int cnt = r.ReadUInt16();
                Map<TableItem> n = new Map<TableItem>(cnt);

                while (--cnt >= 0)
                    n.Add(r.ReadInt32(), new TableItem(r.ReadInt32(), r.ReadInt32()));

                vs[x] = n;
            }

            return vs;
        }
        #endregion
        #endregion

        #region scanner
        #region s
        public static void Serialize(BinaryWriter writer, Scanner scanner)
        {
            writer.Write(S(scanner.GetType()));

            WriteSKT(scanner.ck_table, writer);
            WriteSKT(scanner.sym_table, writer);

            scanner.OnSerialize(writer);
        }

        private static void WriteSKT(StrKeywordTable t, BinaryWriter wr)
        {
            int cnt = t.Count;
            wr.Write((byte)cnt);

            WriteShortStr(t.texts, cnt, wr);
            WriteTT(t.tokenTypes, cnt, wr);
        }

        private static void WriteShortStr(string[] vs, int cnt, BinaryWriter wr)
        {
            Encoding enc = Encoding.UTF8;
            while (--cnt >= 0)
            {
                byte[] buffer = enc.GetBytes(vs[cnt]);
                wr.Write((byte)buffer.Length);
                wr.Write(buffer);
            }
        }

        private static void WriteTT(TokenType[] vs, int cnt, BinaryWriter wr)
        {
            while (--cnt >= 0)
                wr.Write((ushort)vs[cnt]);
        }
        #endregion

        #region d
        public static Scanner Deserialize(BinaryReader reader, IExceptionHandler handler, ITypeMap typeMap)
        {
            if (!(D<Scanner>(reader) is Scanner result))
                return null;

            result.handler = handler;
            result.typeMap = typeMap;
            result.ck_table = ReadSKT(reader);
            result.sym_table = ReadSKT(reader);

            result.OnDeserialize(reader);

            return result;
        }

        private static StrKeywordTable ReadSKT(BinaryReader r)
        {
            StrKeywordTable result = new StrKeywordTable();

            int cnt = r.ReadByte();
            result.texts = ReadShortStr(r, cnt);
            result.tokenTypes = ReadTT(r, cnt);
            result.Count = cnt;

            return result;
        }

        private static string[] ReadShortStr(BinaryReader r, int cnt)
        {
            Encoding enc = Encoding.UTF8;
            string[] vs = new string[cnt];
            while (--cnt >= 0)
                vs[cnt] = enc.GetString(r.ReadBytes(r.ReadByte()));

            return vs;
        }

        private static unsafe TokenType[] ReadTT(BinaryReader r, int cnt)
        {
            TokenType[] vs = new TokenType[cnt];
            while (--cnt >= 0)
            {
                ushort v = r.ReadUInt16();
                vs[cnt] = *(TokenType*)&v;
            }

            return vs;
        }
        #endregion
        #endregion

        #region preprocessor
        public static void Serialize(BinaryWriter writer, PreProcessor preprocessor)
        {
            writer.Write(S(preprocessor.GetType()));
            preprocessor.OnSerialize(writer);
        }

        public static PreProcessor DeserializePP(BinaryReader reader)
        {
            if (!(D<PreProcessor>(reader) is PreProcessor result))
                return null;

            result.OnDeserialize(reader);

            return result;
        }
        #endregion

        #region script loader
        public static void SerializeLoader(BinaryWriter writer, ScriptLoader loader)
        {
            writer.Write(S(loader.GetType()));
            Serialize(writer, loader.m_scanner);
            if (loader.m_preprocessor != null)
            {
                writer.Write(true);
                Serialize(writer, loader.m_preprocessor);
            }
            else
                writer.Write(false);

            Serialize(writer, loader.m_parser, true);

            byte[] buffer = Encoding.UTF8.GetBytes(loader.path);
            writer.Write(buffer.Length);
            writer.Write(buffer);
        }

        public static ScriptLoader DeserializeLoader(BinaryReader reader, IExceptionHandler handler, ITypeMap typeMap)
        {
            if (!(D<ScriptLoader>(reader) is ScriptLoader result))
                return null;

            result.m_scanner = Deserialize(reader, handler, typeMap);
            if (reader.ReadBoolean())
                result.m_preprocessor = DeserializePP(reader);

            result.m_parser = DeserializeLALR(reader, null, handler);
            result.path = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

            return result;
        }
        #endregion
    }
}
