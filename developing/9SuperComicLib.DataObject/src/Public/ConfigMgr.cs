using System;
using System.IO;
using System.Reflection;

namespace SuperComicLib.DataObject
{
    /* Design
     * [Config("vehicles/car_0")]
     * internal static class CarOptions
     * {
     *      [MarkAsName("MaxObjects")]
     *      public static int Name = 10;
     *      public static string Path = "/abcd/poi.config";
     * }
     */

    public static class ConfigMgr
    {
        private static bool IsValid(Type t, out string filename)
        {
            if (t.IsAbstract && t.IsSealed)
            {
                filename = (t.GetCustomAttribute<ConfigAttribute>()?.rel_name ?? t.Name) + ".datobj";
                return true;
            }
            else
            {
                filename = null;
                return false;
            }
        }

        public static void RefreshAll()
        {
            Assembly[] asmbs = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asmb in asmbs)
            {
                string root_dir = Path.GetDirectoryName(asmb.Location);
                foreach (Type t in asmb.GetTypes())
                    OnRefresh(root_dir, t);
            }
        }

        public static void Refresh(Type type, string rootdir)
        {
            if (type != null &&
                string.IsNullOrWhiteSpace(rootdir) == false &&
                Directory.Exists(rootdir))
                OnRefresh(rootdir, type);
        }

        public static void RefreshAuto(Type type)
        {
            if (type != null)
                OnRefresh(Path.GetDirectoryName(type.Assembly.Location), type);
        }

        public static void RefreshAuto<T>() where T : class
        {
            Type type = typeof(T);
            OnRefresh(Path.GetDirectoryName(type.Assembly.Location), type);
        }

        public static void Save(Type type)
        {
            if (type == null)
                return;
            if (IsValid(type, out string name))
                DO_Helper.SaveAll(Path.Combine(Path.GetDirectoryName(type.Assembly.Location), name), type);
        }

        public static void Save(Type type, string rootdir)
        {
            if (type == null || string.IsNullOrWhiteSpace(rootdir))
                return;
            if (IsValid(type, out string name))
                DO_Helper.SaveAll(Path.Combine(rootdir, name), type);
        }

        public static void Save(string fieldName, Type type)
        {
            if (fieldName == null || type == null)
                return;
            if (IsValid(type, out string name))
                DO_Helper.Save(Path.Combine(Path.GetDirectoryName(type.Assembly.Location), name), fieldName, type);
        }

        public static void Save(string fieldName, Type type, string rootdir)
        {
            if (fieldName == null || type == null || string.IsNullOrWhiteSpace(rootdir))
                return;
            if (IsValid(type, out string name))
                DO_Helper.Save(Path.Combine(rootdir, name), fieldName, type);
        }

#if DEBUG
        public static void Debug(Type type, string option_text, bool forcedSetMode)
        {
            System.Diagnostics.Debug.Assert(type != null);
            System.Diagnostics.Debug.Assert(string.IsNullOrWhiteSpace(option_text) == false);

            if (forcedSetMode == false || IsValid(type, out string name))
            {
                System.Diagnostics.Debug.WriteLine($"SCL::DataObject, ok -> {nameof(ConfigMgr)}");
                DO_Helper.Debug(type, option_text);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"SCL::DataObject, warn -> {nameof(forcedSetMode)} is FALSE");
                System.Diagnostics.Debug.WriteLine($"SCL::DataObject, warn -> {nameof(type)} is INVALID.");
                System.Diagnostics.Debug.Fail($"SCL::DataObject, fail -> {nameof(ConfigMgr)}");
            }
        }
#endif

        #region 폐기
        // public static void Save(int[] indexes, Type type)
        // {
        //     if (indexes == null || type == null)
        //         return;
        //     if (IsValid(type, out string name))
        //         DO_Helper.Save(Path.Combine(Path.GetDirectoryName(type.Assembly.Location), name), indexes, type);
        // }

        // public static void Save(int[] indexes, Type type, string rootdir)
        // {
        //     if (indexes == null || type == null || string.IsNullOrWhiteSpace(rootdir))
        //         return;
        //     if (IsValid(type, out string name))
        //         DO_Helper.Save(Path.Combine(rootdir, name), indexes, type);
        // }
        #endregion

        #region private
        private static void OnRefresh(string root_dir, Type t)
        {
            if (IsValid(t, out string name))
            {
                string path = Path.Combine(root_dir, name);
                if (File.Exists(path))
                    DO_Helper.Parse(path, t);
            }
        }
        #endregion
    }
}
