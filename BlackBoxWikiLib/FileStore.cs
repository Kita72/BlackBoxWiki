using System;
using System.IO;

namespace BlackBoxWikiLib
{
    internal static class FileStore
    {
        internal static string SerialFile = $@"{DirectoryStore.AppFolder}\Serial.mywiki";

        internal static string LogFile = $@"{DirectoryStore.MyWikiFolder}\Log.mywiki";

        internal static string InfoFile { get; private set; }
        internal static string TopicFile { get; private set; }

        internal static bool CheckFiles(string directory)
        {
            string SysInfo = $"{Environment.MachineName}-{DateTime.Now.DayOfYear + 1234}";

            FileWorker.LogEvent($@"SETUP -> [FILES]=> {SysInfo} : {directory}");

            if (Directory.Exists(directory))
            {
                InfoFile = $@"{directory}\Info.mywiki";
                TopicFile = $@"{directory}\Data.mywiki";

                if (!File.Exists($@"{directory}\Info.mywiki"))
                {
                    FileWorker.CreateFile(InfoFile, SysInfo);
                }

                if (!File.Exists($@"{directory}\Data.mywiki"))
                {
                    FileWorker.CreateFile(TopicFile, "VOID");
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
