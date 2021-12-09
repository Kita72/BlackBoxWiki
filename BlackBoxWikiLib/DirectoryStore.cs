using System;
using System.IO;

namespace BlackBoxWikiLib
{
    internal static class DirectoryStore
    {
        internal static string AppFolder = AppContext.BaseDirectory.TrimEnd('\\');

        internal static string MyDocFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd('\\');

        internal static string MyWikiFolder = $@"{MyDocFolder}\MyWiki";

        internal static string WikiFolder { get; set; }
        internal static string FileFolder { get; set; }
        internal static string BackUpFolder { get; set; }

        internal static bool CheckDirectory()
        {
            if (!Directory.Exists(MyDocFolder))
                return false;
            if (!Directory.Exists(AppFolder))
                return false;
            if (!Directory.Exists(MyWikiFolder))
            {
                Directory.CreateDirectory(MyWikiFolder);
            }

            FileWorker.LogEvent("setup", "mywiki", $@"=> {MyWikiFolder}");

            return true;
        }

        internal static void SetUpFileDirectory()
        {
            if (!Directory.Exists(WikiFolder))
                Directory.CreateDirectory(WikiFolder);

            if (!Directory.Exists(FileFolder))
                Directory.CreateDirectory(FileFolder);

            if (!Directory.Exists(BackUpFolder))
                Directory.CreateDirectory(BackUpFolder);
        }
    }
}
