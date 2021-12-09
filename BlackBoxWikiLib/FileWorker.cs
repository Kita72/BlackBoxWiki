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

            LogEvent("file", "create", $"=> {file} : {text}");

            WriteAllFileText(file, text, false);
        }

        internal static bool CopyFile(string fileFrom, string fileTo)
        {
            if (File.Exists(fileFrom) && !File.Exists(fileTo))
            {
                LogEvent("file", "success : copy", $"=> {fileFrom} : {fileTo}");

                File.Copy(fileFrom, fileTo);

                return true;
            }
            else
            {
                LogEvent("file", "failed : copy", $"=> {fileFrom} : {fileTo}");

                return false;
            }
        }

        internal static bool MoveFile(string fileFrom, string fileTo)
        {
            if (File.Exists(fileFrom) && !File.Exists(fileTo))
            {
                LogEvent("file", "success : move", $"=> {fileFrom} : {fileTo}");

                File.Move(fileFrom, fileTo);

                return true;
            }
            else
            {
                LogEvent("file", "failed : move", $"=> {fileFrom} : {fileTo}");

                return false;
            }
        }

        internal static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                LogEvent("file", "success : delete", $"=> {file}");

                File.Delete(file);
            }
            else
            {
                LogEvent("file", "failed : delete", $"=> {file}");
            }
        }

        internal static void WriteAllFileText(string file, string content, bool append)
        {
            if (content.Length > 0)
            {
                if (File.Exists(file) && append)
                {
                    LogEvent("file", "success : append text", $"=> {file} : {content.Length} : {append}");

                    File.AppendAllText(file, content);

                    return;
                }

                try
                {
                    if (File.Exists(file))
                        DeleteFile(file);

                    LogEvent("file", "success : write text", $"=> {file} : {content.Length} : {append}");

                    File.WriteAllText(file, content);
                }
                catch
                {
                    LogEvent("file", "failed : write text", $"=> {file} : {content.Length} : {append}");
                }
            }
            else
            {
                LogEvent("file", "failed : write text", $"=> {file} : {content.Length} : {append}");
            }
        }

        internal static void WriteAllFileLines(string file, string[] content, bool append)
        {
            if (content.Length > 0)
            {
                if (File.Exists(file) && append)
                {
                    LogEvent("file", "success : append line", $"=> {file} : {content.Length} : {append}");

                    File.AppendAllLines(file, content);

                    return;
                }

                try
                {
                    if (File.Exists(file))
                        DeleteFile(file);

                    LogEvent("file", "success : write line", $"=> {file} : {content.Length} : {append}");

                    File.WriteAllLines(file, content);
                }
                catch
                {
                    LogEvent("file", "failed : write line", $"=> {file} : {content.Length} : {append}");
                }
            }
            else
            {
                LogEvent("file", "failed : write line", $"=> {file} : {content.Length} : {append}");
            }
        }

        internal static string ReadAllFileText(string file)
        {
            if (File.Exists(file))
            {
                LogEvent("file", "success : read text", $"=> {file}");

                return File.ReadAllText(file);
            }
            else
            {
                LogEvent("file", "success : read text", $"=> {file}");

                return null;
            }
        }

        internal static string[] ReadAllFileLines(string file)
        {
            if (File.Exists(file))
            {
                LogEvent("file", "success : read line", $"=> {file}");

                return File.ReadAllLines(file);
            }
            else
            {
                LogEvent("file", "failed : read line", $"=> {file}");

                return null;
            }
        }

        internal static void ZipWiki(string startPath, string zipPath)
        {
            DeleteFile(zipPath);

            LogEvent("zip", "export", $"=> {startPath} : {zipPath}");

            ZipFile.CreateFromDirectory(startPath, zipPath);
        }

        static readonly List<string> LogList = new List<string>();

        public static void LogEvent(string log, string method, string values)
        {
            LogList.Add($"{log.ToUpper()} -> [{method}] {values}");
        }

        public static void SaveLog()
        {
            if (Directory.Exists(DirectoryStore.MyWikiFolder))
            {
                foreach (string log in LogList)
                {
                    if (log.StartsWith("SETUP -> [MYWIKI]"))
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
