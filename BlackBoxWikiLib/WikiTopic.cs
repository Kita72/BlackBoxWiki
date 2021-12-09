using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackBoxWikiLib
{
    public class WikiTopic : IComparable
    {
        int IComparable.CompareTo(object obj)
        {
            WikiTopic t = (WikiTopic)obj;
            return string.Compare(Display, t.Display);
        }

        int id;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        string title;
        public string Title
        {
            get { return WikiUtility.ConvertFromSys(title); }
            set { title = WikiUtility.ConvertToSys(value.Trim()); }
        }

        public string MetaTitle => title;

        string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        string fileType;
        public string FileType
        {
            get { return fileType.ToLower(); }
            set { fileType = WikiUtility.ConvertToSys(value.Trim()); }
        }

        List<string> metaTags;
        public List<string> MetaTags
        {
            get { return ConvertedMeta(metaTags); }
            set { metaTags = CleanedMeta(value); }
        }

        public void AddToMeta(string meta)
        {
            if (!metaTags.Contains(WikiUtility.ConvertToSys(meta)))
                metaTags.Add(WikiUtility.ConvertToSys(meta));
        }

        List<string> ConvertedMeta(List<string> SysMeta)
        {
            List<string> converted = new List<string>();

            if (SysMeta.Count > 0)
            {
                foreach (var meta in SysMeta)
                {
                    if (meta != title)
                        converted.Add(WikiUtility.ConvertFromSys(meta));
                }
            }

            return converted;
        }

        List<string> CleanedMeta(List<string> dirtyMeta)
        {
            List<string> cleaned = new List<string>();

            if (dirtyMeta.Count > 0)
            {
                foreach (var meta in dirtyMeta)
                {
                    if (!cleaned.Contains(WikiUtility.ConvertToSys(meta.Trim())))
                        cleaned.Add(WikiUtility.ConvertToSys(meta.Trim()));
                }
            }

            if (!cleaned.Contains(title) && title.Length > 0)
            {
                cleaned.Add(title);
            }

            return cleaned;
        }

        string information;
        public string Information
        {
            get { return information; }
            set { information = value.Trim(); }
        }

        public string Display => id == 0 ? Title : GetCleanFileName(true); 

        public WikiTopic()
        {
            id = 0;
            title = string.Empty;
            fileName = string.Empty;
            fileType = string.Empty;
            metaTags = new List<string>();
            information = string.Empty;
        }

        public WikiTopic(int searchID)
        {
            if (TopicStore.HasTopic(searchID))
            {
                WikiTopic topic = TopicStore.GetTopic(searchID);

                id = topic.id;
                title = topic.title;
                fileName = topic.fileName;
                fileType = topic.fileType;
                metaTags = topic.metaTags;
                information = topic.information;
            }
            else
            {
                if (searchID != 0)
                {
                    id = -1;
                    title = "VOID";
                    fileName = "VOID";
                    fileType = "VOID";
                    metaTags = new List<string>() { "VOID" };
                    information = "VOID";
                }
            }
        }

        public void AddToLibrary(bool moveFile, bool isEditMode = false)
        {
            TopicStore.AddNewTopic(this);

            if (!isEditMode)
            {
                if (fileType == "HTTP")
                {
                    AddLinkFile(moveFile);
                }
                else
                {
                    UpdateFileLocation(moveFile);
                }
            }
        }

        public void EditToLibrary(WikiTopic editTopic)
        {
            editTopic.RemovefromLibrary(string.Empty, editTopic);

            AddToLibrary(false, true);
        }

        public void RemovefromLibrary(string backUpDir, WikiTopic editTopic = null)
        {
            if (editTopic == null)
            {
                TopicStore.RemoveTopic(this);

                FileWorker.MoveFile(GetStoredFileName(), $@"{backUpDir}/{GetCleanFileName(true)}");
            }
            else
            {
                TopicStore.RemoveTopic(editTopic);
            }
        }

        void AddLinkFile(bool moveFile)
        {
            if (fileName.Length > 0 && fileType == "HTTP")
            {
                if (fileName.StartsWith("http"))
                {
                    FileWorker.CreateFile($@"{GetStoredFileName()}", fileName);
                }
                else
                {
                    UpdateFileLocation(moveFile);
                }
            }
        }

        string ReadLinkFile()
        {
            return FileWorker.ReadAllFileText(GetStoredFileName());
        }

        void UpdateFileLocation(bool moveFile)
        {
            if (moveFile)
            {
                FileWorker.MoveFile(FileName, GetStoredFileName());
            }
            else
            {
                FileWorker.CopyFile(FileName, GetStoredFileName());
            }
        }

        public string GetCleanFileName(bool withExt)
        {
            if (FileName.StartsWith("http"))
            {
                return CleanLinkName(withExt);
            }
            else
            {
                if (fileType == "SYSTEM")
                {
                    return Title;
                }
                else
                {
                    return CleanFileName(withExt);
                }
            }
        }

        public string GetStoredFileName()
        {
            return $@"{DirectoryStore.FileFolder}\{id}.{FileType}";
        }

        public string GetLink()
        {
            return ReadLinkFile();
        }

        string CleanLinkName(bool withExt)
        {
            string Link = fileName;

            if (Link.Length > 0)
            {
                if (Link.Contains(@"https://"))
                    Link = Link.Replace(@"https://", "");

                if (Link.Contains(@"http://"))
                    Link = Link.Replace(@"http://", "");

                if (Link.Contains("www."))
                    Link = Link.Replace("www.", "");

                if (Link.Contains('/'))
                    Link = Link.Split('/').First();

                if (!withExt)
                    Link = Link.Split('.').First();
            }

            return Link;
        }

        string CleanFileName(bool withExt)
        {
            if (withExt)
                return fileName.Split('\\').Last();
            else
                return fileName.Split('\\').Last().Split('.').First();
        }

        public string GetMetaString(bool isSys)
        {
            if (metaTags.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (string meta in isSys ? metaTags : MetaTags)
                {
                    sb.Append($"{meta}, ");
                }

                return sb.ToString().Trim().Trim(',');
            }
            else
            {
                return string.Empty;
            }
        }

        public override string ToString()
        {
            if (ID > 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append($"{id}");
                sb.Append($"~{title}");
                sb.Append($"~{GetCleanFileName(true)}");
                sb.Append($"~{fileType}");
                sb.Append($"~{GetMetaString(true)}");
                sb.Append($"~{information}");

                FileWorker.LogEvent("topic", "information", $"=> {sb}");

                return sb.ToString();
            }
            else
            {
                return "VOID";
            }
        }
    }
}
