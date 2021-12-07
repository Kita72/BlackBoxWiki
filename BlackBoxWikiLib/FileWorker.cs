using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace BlackBoxWikiLib
{
    public static class FileWorker
    {
        internal static void CreateFile(string file, string text)
        {
            DeleteFile(file);

            WriteAllFileText(file, text, false);
        }

        internal static bool CopyFile(string fileFrom, string fileTo)
        {
            if (File.Exists(fileFrom) && !File.Exists(fileTo))
            {
                LogEvent($"COPY -> [FILE]=> {fileFrom} : {fileTo}");

                File.Copy(fileFrom, fileTo);

                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool MoveFile(string fileFrom, string fileTo)
        {
            if (File.Exists(fileFrom) && !File.Exists(fileTo))
            {
                LogEvent($"MOVE -> [FILE]=> {fileFrom} : {fileTo}");

                File.Move(fileFrom, fileTo);

                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                LogEvent($"DELETE -> [FILE]=> {file}");

                File.Delete(file);
            }
        }

        internal static void WriteAllFileText(string file, string content, bool append)
        {
            if (content.Length > 0)
            {
                if (File.Exists(file) && append)
                {
                    LogEvent($"WRITE-TEXT -> [FILE]=> {file} : {content} : {append}");

                    File.AppendAllText(file, content);

                    return;
                }

                try
                {
                    if (File.Exists(file))
                        DeleteFile(file);

                    LogEvent($"WRITE-TEXT -> [FILE]=> {file} : {content} : {append}");

                    File.WriteAllText(file, content);
                }
                catch
                {
                    LogEvent($"WRITE-TEXT -> [ERROR]=> WriteAllText({file}, {content.Length}, {append})");
                }
            }
        }

        internal static void WriteAllFileLines(string file, string[] content, bool append)
        {
            if (content.Length > 0)
            {
                if (File.Exists(file) && append)
                {
                    LogEvent($"WRITE-LINE -> [FILE]=> {file} : {content} : {append}");

                    File.AppendAllLines(file, content);

                    return;
                }

                try
                {
                    if (File.Exists(file))
                        DeleteFile(file);

                    LogEvent($"WRITE-LINE -> [FILE]=> {file} : {content} : {append}");

                    File.WriteAllLines(file, content);
                }
                catch
                {
                    LogEvent($"WRITE-LINE -> [ERROR]=> WriteAllLines({file}, {content.Length})");
                }
            }
        }

        internal static string ReadAllFileText(string file)
        {
            if (File.Exists(file))
            {
                LogEvent($"READ-TEXT -> [FILE]=> {file}");

                return File.ReadAllText(file);
            }
            else
            {
                return null;
            }
        }

        internal static string[] ReadAllFileLines(string file)
        {
            if (File.Exists(file))
            {
                LogEvent($"READ-LINE -> [FILE]=> {file}");

                return File.ReadAllLines(file);
            }
            else
            {
                return null;
            }
        }

        internal static void ZipWiki(string startPath, string zipPath)
        {
            DeleteFile(zipPath);

            LogEvent($"[ZIP -> [WIKI]=> {startPath} : {zipPath}");

            ZipFile.CreateFromDirectory(startPath, zipPath);
        }

        static readonly List<string> LogList = new List<string>();

        public static void LogEvent(string log)
        {
            LogList.Add(log);
        }

        public static void SaveLog()
        {
            if (Directory.Exists(DirectoryStore.MyWikiFolder))
            {
                foreach (string log in LogList)
                {
                    if (log.StartsWith("SETUP -> [DIRECTORIES]"))
                    {
                        File.WriteAllText(FileStore.LogFile, log + "\n");
                    }
                    else
                    {
                        File.AppendAllText(FileStore.LogFile, log + "\n");
                    }
                }
            }
        }
    }
}
