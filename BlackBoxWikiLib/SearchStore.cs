using System.Collections.Generic;

namespace BlackBoxWikiLib
{
    internal static class SearchStore
    {
        internal static SortedDictionary<string, WikiSearch> SearchLibrary { get; set; } = new SortedDictionary<string, WikiSearch>();

        static void CheckDictionarySetUp()
        {
            if (SearchLibrary == null)
                SearchLibrary = new SortedDictionary<string, WikiSearch>();
        }

        internal static void ResetLibrary()
        {
            SearchLibrary.Clear();
        }

        internal static bool HasSearch(string meta)
        {
            CheckDictionarySetUp();

            return SearchLibrary.ContainsKey(meta);
        }

        internal static void AddNewSearch(WikiSearch wikiSearch)
        {
            CheckDictionarySetUp();

            if (!HasSearch(wikiSearch.Meta))
                SearchLibrary.Add(wikiSearch.Meta, wikiSearch);
            else
                CombineMeta(wikiSearch);
        }

        internal static List<WikiTopic> GetTopicsFromSearch(string meta)
        {
            if (HasSearch(meta))
            {
                SearchLibrary.TryGetValue(meta, out var wikiSearch);

                return wikiSearch.Topics;
            }
            else
            {
                return new List<WikiTopic>();
            }
        }

        static void CombineMeta(WikiSearch wikiSearch)
        {
            WikiSearch storedSearch = SearchLibrary[wikiSearch.Meta];

            foreach (WikiTopic topic in wikiSearch.Topics)
            {
                if (!storedSearch.Topics.Contains(topic))
                    storedSearch.Topics.Add(topic);
            }
        }

        static readonly List<string> list = new List<string>();

        internal static List<string> SearchMetaList()
        {
            if (SearchLibrary.Count > 0)
            {
                list.Clear();

                foreach (string key in SearchLibrary.Keys)
                {
                    list.Add(WikiUtility.ConvertFromSys(key));
                }

                list.Sort();

                return list;
            }
            else
            {
                return new List<string>();
            }
        }
    }
}
