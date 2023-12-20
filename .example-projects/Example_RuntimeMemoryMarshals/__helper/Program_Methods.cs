using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Example_RuntimeMemoryMarshals
{
    partial class Program
    {
        #region sources
        private static string GetTitleName((string, string, Action)[] menu, string text) =>
            int.TryParse(text, out int index) && (uint)(--index) < (uint)menu.Length
            ? menu[index].Item1
            : text;

        private static int TryInvoke((string, string, Action)[] menu, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Invalid name.");
                Console.WriteLine();
                return -1;
            }
            else if (IsExitOrQuit(input))
                return 0;

            var target = GetEntryPoint(menu, input);
            if (target == null)
            {
                Console.WriteLine($"'{input}' is not the name of an runnable example.");
                Console.WriteLine();
                return -1;
            }
            else
            {
                Console.Clear();

                target.Invoke();

                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(Environment.NewLine);

                return 1;
            }
        }

        private static void PrintMenu((string, string, Action)[] menu, int maxTL, int maxSL)
        {
            // [2][4][2]{NAME}[2]
            const int PAD = 2 + 4 + 2 + 2;

            int width = maxTL + maxSL + 4 + 2 + 4;

            string v1 = "[ MENU ]".PadLeft((width >> 1) + 4);
            string v2 = "Name".PadLeft((maxTL >> 1) + PAD).PadRight(maxTL + PAD);
            string v3 = "Summary".PadRight((maxSL >> 1) + PAD).PadLeft(maxSL + PAD);

            string horizontal = new string('-', width);
            for (; ; )
            {
                Console.WriteLine(v1);
                Console.Write(v2);
                Console.WriteLine(v3);

                string theme = string.Empty;
                for (int i = 0; i < menu.Length; ++i)
                {
                    var item = menu[i];

                    var title = item.Item1;
                    var summary = item.Item2;

                    if (theme.Length == 0)
                        theme = GetTheme(title);
                    else if (!title.StartsWith(theme))
                    {
                        Console.Write("  ");
                        Console.WriteLine(horizontal);
                        theme = GetTheme(title);
                    }

                    // Console.Write(item.)
                    Console.Write("  ");
                    Console.Write((i + 1).ToString().PadRight(4));
                    Console.Write("  ");
                    Console.Write(title.PadRight(maxTL));
                    Console.Write("    ");
                    Console.Write(summary);
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine("Enter the name (or, number) of the example to run. To quit, type 'exit' or 'quit'.");

                for (; ; )
                {
                    Console.Write("$ ");

                    var input = Console.ReadLine();

                    input = GetTitleName(menu, input);

                    var code = TryInvoke(menu, input);
                    if (code < 0)
                        continue;

                    if (code == 0)
                    {
                        Console.WriteLine();
                        return;
                    }

                    Console.WriteLine("Press any key to return to the menu...");
                    Console.ReadKey(true);

                    Console.Clear();
                    break;
                }
            }
        }

        private static string GetTheme(string title)
        {
            var idx = title.IndexOf('/');
            return
                idx >= 0
                ? title.Substring(0, idx)
                : title;

        }

        private static (string, string, Action)[] LoadExamples(out int maxTitleLength, out int maxSummaryLength)
        {
            var list = new List<(string, string, Action)>(16);

            maxTitleLength = -1;
            maxSummaryLength = -1;

            foreach (var type in typeof(Program).Assembly.GetTypes())
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    if (method.ReturnType != typeof(void) || method.GetParameters().Length != 0)
                        continue;

                    var attrb = method.GetCustomAttribute<ExampleAttribute>();
                    if (attrb == null)
                        continue;

                    if (IsInvalidTitleName(attrb.Title))
                    {
                        Console.WriteLine($"[WARN] Invalid example title name! --> '{attrb.Title}'");
                        continue;
                    }

                    var lowerTitle = attrb.Title.ToLower();
                    if (ContainsKey(list, lowerTitle))
                    {
                        Console.WriteLine($"[WARN] Duplicate title name. --> '{attrb.Title}'");
                        continue;
                    }

                    if (lowerTitle.Length > maxTitleLength)
                        maxTitleLength = lowerTitle.Length;

                    if (attrb.Summary.Length > maxSummaryLength)
                        maxSummaryLength = attrb.Summary.Length;

                    var target = (Action)method.CreateDelegate(typeof(Action));
                    list.Add((lowerTitle, attrb.Summary, target));
                }

            list.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            return list.ToArray();
        }

        private static Action GetEntryPoint((string, string, Action)[] list, string key) =>
            list.FirstOrDefault(pair => pair.Item1.Equals(key, StringComparison.OrdinalIgnoreCase)).Item3;

        private static bool ContainsKey(List<(string, string, Action)> list, string key) =>
            list.Any(pair => pair.Item1 == key);

        private static bool IsInvalidTitleName(string v)
        {
            if (string.IsNullOrEmpty(v))
                return true;

            if (IsExitOrQuit(v))
                return true;

            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
                if (char.IsWhiteSpace(c))
                    return true;
            }

            return false;
        }

        private static bool IsExitOrQuit(string v) =>
            v.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
            v.Equals("quit", StringComparison.OrdinalIgnoreCase);
        #endregion
    }
}
