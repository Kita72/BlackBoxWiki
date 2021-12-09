using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BlackBoxWikiLib;

namespace BlackBoxWiki
{
    internal static class SearchManager
    {
        internal static WikiSearch CurrentSearch { get; set; }

        static List<WikiSearch> PrevSearch { get; set; } = new List<WikiSearch>();

        static List<WikiSearch> ForwardSearch { get; set; } = new List<WikiSearch>();

        internal static void Search(string meta)
        {
            PushSearch(meta);

            if (CurrentSearch.HasTopics)
            {
                LoadSearchList();

                LoadWikiSearch(meta);
            }
            else
            {
                LoadDuckSearch(meta);
            }
        }

        static void LoadSearchList()
        {
            ArrayList topics = new ArrayList();

            WikiTopic topicHeader = new WikiTopic
            {
                Title = $"Directory [ {CurrentSearch.Meta} ]",
                ID = 0,
                FileType = "SYSTEM"
            };

            topics.Add(topicHeader);

            CurrentSearch.Topics.Sort();

            foreach (WikiTopic topic in CurrentSearch.Topics)
            {
                topics.Add(topic);
            }

            WikiHelper.WikiForm.UpdateSearchListBox(topics, "Display", "ID");
        }

        static void LoadDuckSearch(string meta)
        {
            string duckSearch = $@"https://duckduckgo.com/?q=";

            WikiHelper.WikiForm.RunWebSearch($@"{duckSearch}{meta}");
        }

        static void LoadWikiSearch(string meta)
        {
            string wikiSearch = $@"https://en.wikipedia.org/wiki/";

            WikiHelper.WikiForm.RunWebSearch($@"{wikiSearch}{meta}");
        }

        static AutoCompleteStringCollection SearchCollection { get; set; } = new AutoCompleteStringCollection();

        internal static void LoadSearchList(ToolStripComboBox searchListBox)
        {
            SearchCollection = new AutoCompleteStringCollection
            {
                "Help"
            };

            if (WikiHelper.WikiDir.GetSearchList().Count > 0)
            {
                List<string> getSearchList = WikiHelper.WikiDir.GetSearchList();

                bool reload = false;

                if (searchListBox.Items.Count > 0)
                {
                    if (searchListBox.Items.Count == getSearchList.Count)
                    {
                        for (int i = 0; i < getSearchList.Count - 1; i++)
                        {
                            if (searchListBox.Items[i].ToString() != getSearchList[i].ToString())
                            {
                                reload = true;
                            }
                        }
                    }
                    else
                    {
                        reload = true;
                    }
                }
                else
                {
                    reload = true;
                }

                if (reload && getSearchList != null)
                {
                    searchListBox.BeginUpdate();

                    if (searchListBox.Items.Count != getSearchList.Count)
                    {
                        searchListBox.Items.Clear();

                        SearchCollection.Clear();

                        foreach (string search in getSearchList)
                        {
                            searchListBox.Items.Add(search);

                            SearchCollection.Add(search);
                        }
                    }

                    searchListBox.EndUpdate();
                }
            }

            WikiHelper.WikiForm.UpdateAutoSearch(SearchCollection);
        }

        internal static void PushSearch(string meta)
        {
            CurrentSearch = new WikiSearch(meta, true);

            PrevSearch.Add(CurrentSearch);

            ForwardSearch.Clear();
        }

        internal static bool CanGoBack()
        {
            return PrevSearch.Count > 0;
        }

        internal static void GoBack()
        {
            CycleList(PrevSearch, ForwardSearch);
        }

        internal static bool CanGoForward()
        {
            return ForwardSearch.Count > 0;
        }

        internal static void GoForward()
        {
            CycleList(ForwardSearch, PrevSearch);
        }

        static void CycleList(List<WikiSearch> searches1, List<WikiSearch> searches2)
        {
            if (searches1.Count > 0)
            {
                CurrentSearch = null;

                WikiSearch newSearch = searches1.Last();

                searches2.Add(newSearch);

                WikiHelper.WikiForm.RunSearchRequest(newSearch.Meta);

                searches1.RemoveAt(searches1.Count - 1);
            }
        }
    }
}
