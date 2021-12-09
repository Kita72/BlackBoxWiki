using System;
using System.Collections.Generic;
using System.Text;

namespace BlackBoxWikiLib
{
    public class WikiSearch : IComparable
    {
        int IComparable.CompareTo(object obj)
        {
            WikiSearch t = (WikiSearch)obj;
            return string.Compare(Meta, t.Meta);
        }

        string meta;
        public string Meta
        {
            get { return meta; }
            set { meta = value; }
        }

        public bool HasTopics => topics.Count > 0;

        List<WikiTopic> topics;
        public List<WikiTopic> Topics
        {
            get { return topics; }
            set { topics = value; }
        }

        public WikiSearch(string newSearch, bool topicCheck = false)
        {
            meta = WikiUtility.ConvertToSys(newSearch);

            if (SearchStore.HasSearch(meta))
            {
                if (topicCheck)
                    SearchTopics(meta);
                else
                    topics = new List<WikiTopic>();
            }
            else
            {
                topics = new List<WikiTopic>();
            }
        }

        void SearchTopics(string newSearch)
        {
            topics = SearchStore.GetTopicsFromSearch(newSearch);
        }

        public string TopicIdString()
        {
            if (topics.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (WikiTopic topic in topics)
                {
                    sb.Append($"{topic.ID},");
                }

                return sb.ToString().Trim(',');
            }
            else
            {
                return string.Empty;
            }
        }

        public override string ToString()
        {
            if (Meta.Length > 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append($"{Meta}");
                sb.Append($"~{TopicIdString()}");

                FileWorker.LogEvent("search", "information", $"=> {sb}");

                return sb.ToString();
            }
            else
            {
                return "VOID";
            }
        }
    }
}
