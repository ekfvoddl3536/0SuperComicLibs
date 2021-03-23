using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public object Load(string relativePath) =>
            Load(relativePath, m_scanner.typeMap);

        public abstract object Load(string relativePath, ITypeMap typeMap);

        #region path help
        protected static string CurrentDirectory =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        protected static string PathToName(string path)
        {
            path = Path.GetFileName(path);
            int count = path.Length;
            StringBuilder strb = new StringBuilder(count + 1);
            strb.Append("d"); // 이름오류 방지

            for (int x = 0; x < count; x++)
                if (char.IsLetterOrDigit(path[x]))
                    strb.Append(path[x]);
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
