// MIT License
//
// Copyright (c) 2019-2022 SuperComic (ekfvoddl3535@naver.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SuperComicLib.Text;

namespace SuperComicLib.DataObject
{
    using static DataObject_Helper;
    internal static class DO_Helper
    {
        #region 상수
        private const string lstr = "System.String";
        private const string lbool = "System.Boolean";
        private const string lr8 = "System.Double";
        private const string lr4 = "System.Single";
        private const string l64 = "System.Int64";

        private const string aligned = "#aligned";
        private const string unaligned = "#unaligned";
        #endregion

        public static void Parse(string filepath, Type t)
        {
            StreamReader sr = new StreamReader(File.OpenRead(filepath), NoBOMEncoding.UTF8);

            string header = sr.ReadLine();
            if (header == aligned)
                Aligned_OnParse(sr, t.GetFields(stbflag));
            else
                Unaligned_OnParse(sr, t.GetFields(stbflag));

            sr.Close();
        }

        public static void SaveAll(string filepath, Type t)
        {
            StreamWriter wr = new StreamWriter(File.Create(filepath), NoBOMEncoding.UTF8);

            wr.WriteLine(aligned);
            foreach (FieldInfo fd in t.GetFields(stbflag))
                if (fd.FieldType.FullName == lstr)
                    SaveString(wr, "string : ", (string)fd.GetValue(null));
                else
                    wr.WriteLine($"{L2S(fd.FieldType.FullName)} : {fd.GetValue(null)}");

            wr.Close();
        }

        public static void Save(string filepath, string fieldName, Type t)
        {
            StreamWriter wr = new StreamWriter(File.OpenWrite(filepath), NoBOMEncoding.UTF8);
            wr.BaseStream.Seek(0, SeekOrigin.End);
            wr.WriteLine(unaligned);

            Write(wr, t.GetField(fieldName, stbflag));

            wr.Close();
        }

#if DEBUG
        public static void Debug(Type type, string option_text)
        {
            var encoding = System.Text.Encoding.UTF8;

            MemoryStream ms = new MemoryStream(encoding.GetBytes(option_text));
            System.Diagnostics.Debug.WriteLine($"SCL::DataObject, MemoryStream -> {nameof(option_text)} ({nameof(System.Text.Encoding.UTF8)})");

            StreamReader sr = new StreamReader(ms, encoding);
            System.Diagnostics.Debug.WriteLine($"SCL::DataObject, StreamReader -> {nameof(ms)} (local [0])");

            string header = sr.ReadLine();
            System.Diagnostics.Debug.WriteLine($"SCL::DataObject, {nameof(sr.ReadLine)}() result -> {header}");

            if (header == aligned)
            {
                System.Diagnostics.Debug.WriteLine($"SCL::DataObject, if result -> Aligned");
                Aligned_OnParse(sr, type.GetFields(stbflag));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"SCL::DataObject, if result -> Unaligned");
                Unaligned_OnParse(sr, type.GetFields(stbflag));
            }

            sr.Close();
            System.Diagnostics.Debug.WriteLine($"SCL::DataObject, called -> StreamReader.Close()");
        }
#endif

        #region non-public
        private static void Aligned_OnParse(StreamReader sr, FieldInfo[] fields)
        {
            // int : 123141425
            // string : {"123"}
            // <type> : <value>
            foreach (FieldInfo fd in fields)
            {
                string str = sr.MoveNext();
                if (str != null && str[0] != '!')
                {
                    string[] iv = str.Split(split_char_2, 2);
                    if (iv.Length == 2)
                        fd.SetValue(null, ParseValue(sr, iv[0].Trim(), iv[1].Trim()));
                }
            }
        }

        private static void Unaligned_OnParse(StreamReader sr, FieldInfo[] fields)
        {
            // 배치 순서 상관 x
            // int <name> = <value>
            // <type> <name> = <value>
            Dictionary<HashedString, object> values = new Dictionary<HashedString, object>();

            while (sr.EndOfStream == false)
            {
                string str = sr.MoveNext();
                if (str != null && str[0] != '!')
                    SetValue(sr, str, values);
            }

            int x = fields.Length;
            while (--x >= 0)
            {
                FieldInfo fd = fields[x];
                if (values.TryGetValue(fd.GetCustomAttribute<MarkAsNameAttribute>()?.opt_name ?? fd.Name, out object v))
                    fd.SetValue(null, v);
            }

            values.Clear();
        }

        private static void SetValue(StreamReader sr, string str, Dictionary<HashedString, object> values)
        {
            string[] sp1 = str.Split(split_char_0, 2);
            if (sp1.Length == 2)
            {
                string[] type_and_name = sp1[0].Split(split_char_1, opt);

                if (type_and_name.Length == 2)
                    values[type_and_name[1].Trim()] = ParseValue(sr, type_and_name[0].Trim(), sp1[1].Trim());
#if DEBUG
                else
                    System.Diagnostics.Debug.Fail("invalid -> " + str);
#endif
            }
        }

        private static object ParseValue(StreamReader sr, string type, string value) =>
            type == s32 && int.TryParse(value, out int r0)
            ? r0
            : type == s64 && long.TryParse(value, out long r1)
            ? r1
            : type == sr4 && float.TryParse(value, out float r2)
            ? r2
            : type == sr8 && double.TryParse(value, out double r3)
            ? r3
            : type == sbool && bool.TryParse(value, out bool r4)
            ? r4
            : value.StartsWith("{\"")
            ? RawLongString(sr, value)
            : value.StartsWith("\"")
            ? value.Substring(1, value.Length - 2)
            : (object)value;

        private static string RawLongString(StreamReader sr, string value)
        {
            if (value.EndsWith("\"}"))
                return value.Substring(2, value.Length - 4);

            string temp = value.Substring(2);
            int count = 0;

        loop:
            value = sr.MoveNext(ref count);
            if (value == null)
                return temp;

            for (; count > 0; count--)
                temp += Environment.NewLine;

            if (value.EndsWith("\"}"))
                return temp + value.RemoveBack(2);

            temp += value;
            goto loop;
        }

        private static void SaveString(StreamWriter wr, string pre, string value)
        {
            if (value.Contains(Environment.NewLine))
            {
                string[] vs = value.Split(split_nl, 0);
                wr.Write(pre + "{\"");

                int x = 0, max = vs.Length - 1;
                for (; x < max; x++)
                    wr.WriteLine(vs[x]);

                wr.WriteLine(vs[x] + "\"}");
            }
            else
                wr.WriteLine(pre + value);
        }

        // public static void Save(string filepath, int[] indexes, Type t)
        // {
        //     using (StreamWriter wr = new StreamWriter(File.OpenWrite(filepath), BOMEncoding.UTF8))
        //     {
        //         wr.BaseStream.Seek(0, SeekOrigin.End);
        //         wr.WriteLine(unaligned);
        // 
        //         FieldInfo[] fds = t.GetFields(stbflag);
        //         foreach (int i in indexes)
        //             Write(wr, fds[i]);
        //     }
        // }

        private static void Write(StreamWriter wr, FieldInfo fd)
        {
            string name =
                fd.GetCustomAttribute<MarkAsNameAttribute>() is MarkAsNameAttribute attrb
                ? attrb.opt_name
                : fd.Name;

            if (fd.GetCustomAttribute<SetDescAttribute>() is SetDescAttribute desc)
            {
                string[] list = desc.opt_desc;
                int len;
                if (list != null && (len = list.Length) > 0)
                {
                    string c;
                    for (int x = 0; x < len; x++)
                        if (string.IsNullOrWhiteSpace(c = list[x]))
                            wr.WriteLine('!');
                        else
                        {
                            wr.Write("! ");
                            wr.WriteLine(c);
                        }
                }
                else
                {
                    wr.WriteLine("! ========== exception ==========");
                    wr.WriteLine("! can't write description");
                }
            }

            if (fd.FieldType.FullName == lstr)
                SaveString(wr, $"{L2S(fd.FieldType.FullName)} {name} = ", (string)fd.GetValue(null));
            else
                wr.WriteLine($"{L2S(fd.FieldType.FullName)} {name} = {fd.GetValue(null)}");
        }

        private static string L2S(string lname) =>
            lname == lbool
            ? sbool
            : lname == l64
            ? s64
            : lname == lr4
            ? sr4
            : lname == lr8
            ? sr8
            : s32;
        #endregion
    }
}
