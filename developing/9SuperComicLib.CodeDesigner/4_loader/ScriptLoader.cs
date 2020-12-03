using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SuperComicLib.CodeDesigner
{
    public abstract class ScriptLoader : IDisposable
    {
        protected internal Scanner m_scanner;
        protected internal PreProcessor m_preprocessor;
        protected internal LALRParser m_parser;
        protected internal string m_path;
        protected internal AssemblyBuilder m_asmbd;
        protected internal ModuleBuilder m_modbd;

        #region constructor
        private ScriptLoader()
        {
            m_asmbd =
                AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName("9SuperComicLib.CodeDesigner.LoadedScripts"),
                    AssemblyBuilderAccess.RunAndCollect);
            m_modbd = m_asmbd.DefineDynamicModule("LoadedScriptsModule");
        }

        public ScriptLoader(Grammar grammar) : 
            this(ScannerFactory.Default,
                null, 
                new LALRParser(grammar), 
                CurrentDirectory)
        {
        }

        public ScriptLoader(Grammar grammar, PreProcessor preprocessor) :
            this(ScannerFactory.Default,
                preprocessor,
                new LALRParser(grammar),
                CurrentDirectory)
        {
        }
    
        /// <exception cref="ArgumentNullException">rootDirectory is null or empty or whitespace</exception>
        public ScriptLoader(Scanner scanner, PreProcessor preprocessor, LALRParser parser, string rootDirectory) : this()
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
                throw new ArgumentNullException(nameof(rootDirectory));
            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);

            m_path = rootDirectory;
            m_scanner = scanner;
            m_preprocessor = preprocessor;
            m_parser = parser;
        }
        #endregion

        public abstract bool IsReference(HashedString source, HashedString target);

        public bool IsEndlessRef(HashedString v1, HashedString v2) =>
            IsReference(v1, v2) &&
            IsReference(v2, v1);

        public Type LoadOrGet(string relativePath) =>
            LoadOrGet(relativePath, m_scanner.typeMap);

        public abstract Type LoadOrGet(string relativePath, ITypeMap typeMap);
        // public Type LoadOrGet(string relativePath, IExceptionHandler handler, ITypeMap typeMap)
        // {
        //     HashedString hs = new HashedString(relativePath);
        //     if (cantLoad.Contains(hs))
        //         return null;
        //     
        //     if (items.TryGetValue(hs, out Type result))
        //         return result;
        // 
        //     string absolutePath = Path.Combine(path, relativePath);
        //     if (File.Exists(absolutePath) == false)
        //         throw new FileNotFoundException(absolutePath);
        // 
        //     ITokenEnumerator toks =
        //         m_preprocessor == null
        //         ? m_scanner.FromFile(absolutePath, Encoding.UTF8)
        //         : m_scanner.FromStream(m_preprocessor, File.OpenRead(absolutePath), Encoding.UTF8, false);
        // 
        //     if (!references.TryGetValue(hs, out CHashSet<HashedString> map))
        //     {
        //         map = new CHashSet<HashedString>();
        //         references.Add(hs, map);
        //     }
        // 
        //     TypeBuilder tb = modbd.DefineType(PathToName(relativePath));
        //     CodeGeneratorBase codegen = GetCodeGenerator(tb, map, hs, handler, typeMap);
        //     codegen.Generate(m_parser.Parse(toks, true));
        //     if (handler.FailCount > 0)
        //     {
        //         result = null;
        //         cantLoad.Add(hs);
        //     }
        //     else
        //     {
        //         result = tb.CreateType();
        //         items.Add(hs, result);
        //     }
        // 
        //     codegen.Dispose();
        //     
        //     return result;
        // }

        #region path help
        protected static string CurrentDirectory =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        protected static string PathToName(string relpath)
        {
            int count = relpath.Length;
            StringBuilder strb = new StringBuilder(count + 1);
            strb.Append("d"); // 이름오류 방지

            for (int x = 0; x < count; x++)
                if (char.IsLetterOrDigit(relpath[x]))
                    strb.Append(relpath[x]);
                else
                    strb.Append('_'); // 이름 exception 방지

            return strb.ToString();
        }
        #endregion

        #region dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_preprocessor != null)
            {
                m_preprocessor.Dispose();
                m_preprocessor = null;
            }

            if (m_path != null)
            {
                m_scanner.Dispose();
                m_parser.Dispose();

                m_scanner = null;
                m_parser = null;

                m_path = null;
                m_asmbd = null;
                m_modbd = null;
            }
        }
        #endregion
    }
}
