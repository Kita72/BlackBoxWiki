namespace BlackBoxWikiLib
{
    public class WikiFile
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string infoFile;

        public string InfoFile
        {
            get { return infoFile; }
            set { infoFile = value; }
        }

        string topicFile;

        public string TopicFile
        {
            get { return topicFile; }
            set { topicFile = value; }
        }

        string searchFile;

        public string SearchFile
        {
            get { return searchFile; }
            set { searchFile = value; }
        }

        public WikiDirectory WikiDir { get; set; }

        public WikiFile(WikiDirectory wikiDirectory)
        {
            WikiDir = wikiDirectory;
        }

        public void CreateWikiFolder(string directory)
        {
            DirectoryStore.WikiFolder = directory;
            DirectoryStore.FileFolder = $@"{directory}\Files";
            DirectoryStore.BackUpFolder = $@"{directory}\BackUp";

            FileWorker.LogEvent($@"LOAD -> [WIKI FOLDERS]=> {DirectoryStore.WikiFolder} : {DirectoryStore.FileFolder} : {DirectoryStore.BackUpFolder}");

            DirectoryStore.SetUpFileDirectory();
        }

        public void SetUpWikiFiles(string directory)
        {
            FileStore.CheckFiles(directory);
        }
    }
}
