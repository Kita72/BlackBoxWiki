using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlackBoxWikiLib
{
    public class WikiDirectory
    {
        public string AppFolder => DirectoryStore.AppFolder;

        public string MyDocFolder => DirectoryStore.MyDocFolder;

        public string MyWikiFolder => DirectoryStore.MyWikiFolder;

        public WikiFile WikiHandle { get; set; }

        public bool MoveFile { get; set; } = false;

        public bool ScrapeFile { get; set; } = false;

        public int ScrapeLetter { get; set; } = 0;

        public int ScrapeFrequency { get; set; } = 0;

        void SetWikiFile(string name)
        {
            WikiHandle.Name = name;

            WikiHandle.InfoFile = FileStore.InfoFile;

            WikiHandle.TopicFile = FileStore.TopicFile;
        }

        public string WikiFolder
        {
            get => DirectoryStore.WikiFolder;

            set => DirectoryStore.WikiFolder = value;
        }

        public string FileFolder
        {
            get => DirectoryStore.FileFolder;

            private set => DirectoryStore.FileFolder = $@"{WikiFolder}\Files";
        }

        public string BackUpFolder
        {
            get => DirectoryStore.BackUpFolder;

            private set => DirectoryStore.BackUpFolder = $@"{WikiFolder}\BackUp";
        }

        public WikiDirectory()
        {
        }

        public void StartWiki() => FileWorker.CreateFile(FileStore.SerialFile, "Running");

        public void StopWiki() => FileWorker.DeleteFile(FileStore.SerialFile);

        public bool CheckRunning()
        {
            string GetSerial = FileWorker.ReadAllFileText(FileStore.SerialFile);

            if (GetSerial == null)
            {
                return false;
            }
            else
            {
                FileWorker.LogEvent($@"FAILED -> [START]=> {DirectoryStore.AppFolder}\Serial.mywiki)");

                return true;
            }
        }

        public bool IsValidDirectorySetup()
        {
            return DirectoryStore.CheckDirectory();
        }

        public string StartNewWiki(string wikiName, out string message)
        {
            wikiName = WikiUtility.ConvertFromSys(wikiName);

            bool fileList = Directory.GetDirectories(MyWikiFolder, wikiName).Length == 0;

            bool notCancelled = wikiName != "Cancel";

            WikiUtility.SaveAll();

            WikiUtility.ResetAll();

            if (wikiName.Length > 2 && fileList && notCancelled)
            {
                SetUpNewWiki(wikiName);

                message = $"[{wikiName}]-> Added";
            }
            else
            {
                message = "Failed : No Name or a Duplicate";

                wikiName = string.Empty;
            }

            return wikiName;
        }

        void SetUpNewWiki(string wikiName)
        {
            WikiHandle = new WikiFile(this);

            WikiHandle.CreateWikiFolder($@"{DirectoryStore.MyWikiFolder}\{wikiName}");

            WikiHandle.SetUpWikiFiles($@"{DirectoryStore.MyWikiFolder}\{wikiName}");

            SetWikiFile(wikiName);
        }

        public string ImportWiki(out string message)
        {
            string wikiName = string.Empty;

            string failedMessage = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = MyWikiFolder;
                openFileDialog.Filter = "Wiki File (Info.mywiki)|Info.mywiki";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    WikiUtility.SaveAll();

                    WikiUtility.ResetAll();

                    string filePath = openFileDialog.FileName;

                    if (filePath.Contains("Info.mywiki"))
                    {
                        string fileName = filePath.Split('\\').Last();

                        ImportWikiFile(filePath.Replace(fileName, "").TrimEnd('\\'));

                        wikiName = WikiFolder.Split('\\').Last();
                    }
                    else
                    {
                        failedMessage = "Corrupt [Info.mywiki] File";
                    }
                }
            }

            if (wikiName.Length > 0)
            {
                TopicStore.LoadTopicData();

                message = $"[{wikiName}]-> Imported";
            }
            else
            {
                if (failedMessage == string.Empty)
                    message = "Failed to Import";
                else
                    message = failedMessage;
            }

            return wikiName;
        }
        void ImportWikiFile(string wikiName)
        {
            WikiFolder = wikiName;

            WikiHandle = new WikiFile(this);

            WikiHandle.CreateWikiFolder(wikiName);

            WikiHandle.SetUpWikiFiles(wikiName);

            SetWikiFile(wikiName.Split('\\').Last());
        }

        public void MergeWiki(out string message)
        {
            string finalMessage = "";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = MyWikiFolder;
                openFileDialog.Filter = "Wiki File (Info.mywiki)|Info.mywiki";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    if (filePath.Contains("Info.mywiki"))
                    {
                        string path = filePath.Replace(filePath.Split('\\').Last(), "").TrimEnd('\\');

                        TopicStore.MergeTopicData($@"{path}\Data.mywiki", $@"{path}\Files");

                        finalMessage = "Success : Merge Completed!";
                    }
                    else
                    {
                        finalMessage = "Failed : Corrupt Info!";
                    }
                }
                else
                {
                    finalMessage = "Exiting : Cancelled Merge!";
                }
            }

            message = finalMessage;
        }

        public void ExportWiki(bool Export, out string message)
        {
            if (WikiFolder != null)
            {
                if (Export)
                {
                    WikiUtility.SaveAll();

                    string startPath = WikiFolder;

                    string zipPath = $@"{MyWikiFolder}\{WikiHandle.Name}.zip";

                    Task.Factory.StartNew(() => FileWorker.ZipWiki(startPath, zipPath));

                    message = $"[{WikiHandle.Name}]-> Exported : Loading Hosting Site";
                }
                else
                {
                    message = "Failed to Export";
                }
            }
            else
            {
                message = string.Empty;
            }
        }

        public void DeleteWiki(string currentWiki, out string message)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = MyWikiFolder;
                openFileDialog.Filter = "Wiki (Info.mywiki)|Info.mywiki";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    if (filePath.Contains("Info.mywiki"))
                    {
                        string fileName = filePath.Split('\\').Last();

                        string directoryPath = filePath.Replace(fileName, "").Trim('\\');

                        if (directoryPath.Split('\\').Last().ToUpper() != currentWiki.ToUpper())
                        {
                            Directory.Delete(directoryPath, true);

                            message = $"{directoryPath.Split('\\').Last()} Removed";
                        }
                        else
                        {
                            message = "Failed : Wiki in use!!";
                        }
                    }
                    else
                    {
                        message = "That is not a Valid wiki!";
                    }
                }
                else
                {
                    message = "Operation Canceled!";
                }
            }
        }

        public List<string> GetSearchList()
        {
            return SearchStore.SearchMetaList();
        }

        public string ReadText(string location)
        {
            return FileWorker.ReadAllFileText(location);
        }
        public string[] ReadLines(string location)
        {
            return FileWorker.ReadAllFileLines(location);
        }
    }
}
