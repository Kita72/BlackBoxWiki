using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlackBoxWikiLib
{
    internal static class TopicStore
    {
        internal static SortedDictionary<int, WikiTopic> TopicLibrary { get; set; } = new SortedDictionary<int, WikiTopic>();

        static void CheckDictionarySetUp()
        {
            if (TopicLibrary == null)
                TopicLibrary = new SortedDictionary<int,WikiTopic>();
        }

        internal static void ResetLibrary()
        {
            TopicLibrary.Clear();
        }

        internal static bool HasTopic(int id)
        {
            CheckDictionarySetUp();

            return TopicLibrary.ContainsKey(id);
        }

        internal static WikiTopic GetTopic(int id)
        {
            return TopicLibrary[id];
        }

        static int GetNewID()
        {
            int newID = 0;
            int idChecked = 1;

            while (newID == 0)
            {
                if (!HasTopic(idChecked))
                {
                    newID = idChecked;
                }
                else
                {
                    idChecked++;
                }
            }

            return newID;
        }

        internal static void AddNewTopic(WikiTopic wikiTopic)
        {
            CheckDictionarySetUp();

            wikiTopic.ID = GetNewID();

            TopicLibrary.Add(wikiTopic.ID, wikiTopic);

            CreateSearch(wikiTopic);
        }

        internal static void RemoveTopic(WikiTopic wikiTopic)
        {
            if (HasTopic(wikiTopic.ID))
            {
                TopicLibrary.Remove(wikiTopic.ID);

                RefreshSearch();
            }
        }

        static void CreateSearch(WikiTopic wikiTopic)
        {
            WikiSearch search = new WikiSearch(WikiUtility.ConvertToSys(wikiTopic.Title));

            search.Topics.Add(wikiTopic);

            SearchStore.AddNewSearch(search);

            foreach (string meta in wikiTopic.MetaTags)
            {
                search = new WikiSearch(WikiUtility.ConvertToSys(meta));

                search.Topics.Add(wikiTopic);

                SearchStore.AddNewSearch(search);
            }
        }

        static void RefreshSearch()
        {
            SearchStore.ResetLibrary();

            foreach (WikiTopic topic in TopicLibrary.Values)
            {
                CreateSearch(topic);
            }
        }

        internal static void SaveTopicData()
        {
            List<string> data = new List<string>();

            foreach (WikiTopic topic in TopicLibrary.Values)
            {
                data.Add(topic.ToString());
            }

            if (data.Count == 0)
                data.Add("VOID");

            FileWorker.WriteAllFileLines(FileStore.TopicFile, data.ToArray(), false);
        }

        internal static void LoadTopicData()
        {
            ResetLibrary();

            string[] data = FileWorker.ReadAllFileLines(FileStore.TopicFile);

            if (data.Length > 0 && !data[0].Contains("VOID"))
            {
                foreach (string line in data)
                {
                    WikiTopic topic = new WikiTopic();

                    string[] getValues = line.Split('~');

                    if (getValues.Length == 6)
                    {
                        topic.ID = Convert.ToInt32(getValues[0]);
                        topic.Title = getValues[1];
                        topic.FileName = getValues[2];
                        topic.FileType = getValues[3];
                        List<string> metaList = getValues[4].Split(',').ToList();
                        topic.MetaTags = metaList;
                        topic.Information = getValues[5];
                    }

                    if (topic.ID > 0)
                    {
                        TopicLibrary.Add(topic.ID, topic);
                    }
                }

                FileWorker.LogEvent($"TOPIC -> [LOADED]=> {TopicLibrary.Count} topics");

                RefreshSearch();
            }
        }

        internal static void MergeTopicData(string filePath, string fileFolder)
        {
            string[] data = FileWorker.ReadAllFileLines(filePath);

            if (data.Length > 0 && !data[0].Contains("VOID"))
            {
                int preCount = TopicLibrary.Count;

                foreach (string line in data)
                {
                    WikiTopic topic = new WikiTopic();

                    string[] getValues = line.Split('~');

                    if (getValues.Length == 6)
                    {
                        topic.ID = Convert.ToInt32(getValues[0]);
                        topic.Title = getValues[1];
                        topic.FileName = getValues[2];
                        topic.FileType = getValues[3];
                        List<string> metaList = getValues[4].Split(',').ToList();
                        topic.MetaTags = metaList;
                        topic.Information = getValues[5];

                        int newID = GetNewID();

                        if (File.Exists($@"{fileFolder}\{topic.ID}.{topic.FileType}"))
                        {
                            FileWorker.CopyFile($@"{fileFolder}\{topic.ID}.{topic.FileType}", $@"{DirectoryStore.FileFolder}\{newID}.{topic.FileType}");
                        }
                        else
                        {
                            FileWorker.LogEvent($"TOPIC -> [MERGED]=> Failed Copy : {$@"{fileFolder}\{topic.ID}.{topic.FileType}"}");
                        }

                        topic.ID = newID;

                        if (topic.ID > 0 && !HasTopic(newID))
                        {
                            TopicLibrary.Add(topic.ID, topic);
                        }
                        else
                        {
                            FileWorker.LogEvent($"TOPIC -> [MERGED]=> Failed ID {newID}");
                        }
                    }
                }

                FileWorker.LogEvent($"TOPIC -> [MERGED]=> {TopicLibrary.Count - preCount} new topics");

                RefreshSearch();
            }
        }
    }
}
