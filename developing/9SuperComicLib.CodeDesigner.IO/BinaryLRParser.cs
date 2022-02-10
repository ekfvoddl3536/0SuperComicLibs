using System.Collections.Generic;
using System.IO;
using System.Text;
using SuperComicLib.CodeDesigner.IO;
using SuperComicLib.Collections;

namespace SuperComicLib.CodeDesigner
{
    public sealed class BinaryLRParser : ParserBase
    {
        private Map<int>[] gtb;
        private Map<TableItem>[] atb;

        private BinaryLRParser(bool leaveOpen, Grammar grammar, Map<int>[] gtb, Map<TableItem>[] atb, IExceptionHandler handler) : base(grammar, handler, leaveOpen)
        {
            this.gtb = gtb;
            this.atb = atb;
        }

        #region method
        protected override INode OnParse(IEnumerator<Token> iterator, Stack<int> tempstack, Stack<INode> tempnodes)
        {
            tempstack.Push(0);

            Map<int>[] gtb = this.gtb;
            Map<TableItem>[] atb = this.atb;

            Grammar g = m_grammar;

            do
            {
                int current = (int)iterator.Current.type;
                if (!atb[tempstack.Peek()].TryGet(current, out TableItem ctb))
                {
                    m_handler.Fail(FMSG.I);
                    return null;
                }

                int act = ctb.actType;
                if (act == LALRParser.act_SHIFT)
                {
                    tempnodes.Push(new TokNode(iterator.Current));
                    tempstack.Push(ctb.nextstate);

                    iterator.MoveNext();
                }
                else if (act == LALRParser.act_REDUCE)
                {
                    GItem item = g.m_items[ctb.nextstate];
                    int len = item.express.Length;

                    if (len > 1)
                    {
                        ExNode node = new ExNode(len);
                        while (--len >= 0)
                        {
                            tempstack.Pop();
                            node.Add(tempnodes.Pop());
                        }
                        tempnodes.Push(node);
                    }
                    else
                        while (--len >= 0)
                            tempstack.Pop();

                    tempstack.Push(gtb[tempstack.Peek()].Get(item.produce));
                }
                else if (act == LALRParser.act_ACCEPT)
                    return tempnodes.Pop();
            } while (true);
        }

        protected override void Dispose(bool disposing)
        {
            int x;
            if (gtb != null)
            {
                x = gtb.Length;
                while (--x >= 0)
                {
                    gtb[x].Dispose();
                    gtb[x] = null;
                }

                gtb = null;
            }

            if (atb != null)
            {
                x = atb.Length;
                while (--x >= 0)
                {
                    atb[x].Dispose();
                    atb[x] = null;
                }

                atb = null;
            }

            base.Dispose(disposing);
        }
        #endregion

        #region static
#if DEBUG
        private static int GetCapacity(LALRParser target)
        {
            Grammar g = target.m_grammar;
            GItem[] gts = g.m_items;

            int x = 0, max = gts.Length;

            int size = 17 + max * 5 + g.m_nonterminals.Length * 4; // bool + ushort * 4 + int * 2 + (sizeof(ushort) + sizeof(byte)) * N
            for (; x < max; x++)
                size += gts[x].express.Length * 4;

            Map<int>[] gtb = target.gtb;
            max = gtb.Length;

            size += max * 2;
            for (x = 0; x < max; x++)
                size += gtb[x].Count * 8;

            Map<TableItem>[] atb = target.atb;
            max = atb.Length;

            size += max * 2;
            for (x = 0; x < max; x++)
                size += atb[x].Count * 12;

            return size;
        }

        public static byte[] ToByteArray(LALRParser target)
        {
            MemoryStream ms = new MemoryStream(GetCapacity(target));
            BinaryWriter wr = new BinaryWriter(ms, Encoding.ASCII, false);

            Serialize(wr, target);

            byte[] result = ms.ToArray();
            wr.Dispose();

            return result;
        }

        public static void Serialize(BinaryWriter writer, LALRParser target)
        {
            writer.Write(target.leaveOpen);
            SaveLoader.WriteGrammar(writer, target.m_grammar);
            SaveLoader.WriteGTB(target.gtb, writer);
            SaveLoader.WriteATB(target.atb, writer);
        }
#endif

        public static ParserBase Deserialize(BinaryReader reader, IExceptionHandler handler) =>
            new BinaryLRParser(
                reader.ReadBoolean(),
                SaveLoader.ReadGrammar(reader),
                SaveLoader.ReadGTB(reader),
                SaveLoader.ReadATB(reader),
                handler);

        public static ParserBase FromByteArray(byte[] datas, IExceptionHandler handler)
        {
            BinaryReader rd =
                new BinaryReader(
                    new MemoryStream(datas, false),
                    Encoding.ASCII,
                    false);

            ParserBase result = Deserialize(rd, handler);
            rd.Dispose();

            return result;
        }

        public static ParserBase FromFile(string filePath, IExceptionHandler handler)
        {
            if (File.Exists(filePath) == false)
                throw new FileNotFoundException(filePath);

            BinaryReader rd = 
                new BinaryReader(
                    File.OpenRead(filePath),
                    Encoding.ASCII,
                    false);

            ParserBase result = Deserialize(rd, handler);
            rd.Dispose();

            return result;
        }
        #endregion
    }
}
