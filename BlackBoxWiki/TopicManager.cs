using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BlackBoxWikiLib;
using BlackBoxWiki.Properties;
using System.Text;

namespace BlackBoxWiki
{
    internal static class TopicManager
    {
        static WikiDirectory WikiDir { get { return WikiHelper.WikiDir; } }

        static string ScrapedMeta { get; set; } = string.Empty;

        static WikiTopic Topic { get; set; }

        static WikiTopic EditTopic { get; set; }

        static bool IsEditMode { get; set; } = false;

        internal static void StartNewTopic(WikiMessage msg)
        {
            if (msg.Response && msg.ReplyText.Length > 2)
            {
                Topic = new WikiTopic
                {
                    Title = msg.ReplyText
                };

                GetFile();
            }
            else
            {
                RunErrorMessage();
            }
        }
        internal static void StartEditTopic(WikiTopic wikiTopic, WikiMessage msg)
        {
            if (msg.Response)
            {
                IsEditMode = true;

                EditTopic = wikiTopic;

                Topic = wikiTopic;

                GetMeta();
            }
            else
            {
                RunErrorMessage();
            }
        }

        internal static void GetFileReply(WikiMessage msg)
        {
            ScrapedMeta = string.Empty;

            if (msg.Response)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = WikiDir.MyDocFolder;
                    openFileDialog.Filter = "Topic File (*.*)|*.*";
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (openFileDialog.FileName.Length > 0)
                        {
                            msg.ReplyText = openFileDialog.FileName;

                            Topic.FileType = msg.ReplyText.Split('\\').Last().Split('.').Last();

                            ScrapedMeta = ScrapeMetaData(Topic, msg.ReplyText);

                            SetTopicFile(msg);
                        }
                    }
                }
            }
            else if (!msg.Response)
            {
                GetLink();
            }
            else
            {
                RunErrorMessage();
            }
        }

        internal static void GetLinkReply(WikiMessage msg)
        {
            if (msg.Response && msg.ReplyText.Contains("http"))
            {
                Topic.FileType = "HTTP";

                SetTopicFile(msg);
            }
            else
            {
                RunErrorMessage();
            }
        }

        internal static void SetTopicFile(WikiMessage msg)
        {
            if (msg.Response && msg.ReplyText.Length > 2)
            {
                Topic.FileName = msg.ReplyText;

                GetMeta();
            }
            else
            {
                RunErrorMessage();
            }
        }

        internal static void GetMetaReply(WikiMessage msg)
        {
            msg.ReplyText = $"{Topic.Title},{msg.ReplyText}".Trim(',');

            List<string> list = msg.ReplyText.Split(',').ToList();

            SetMeta(msg, list);
        }

        internal static void SetMeta(WikiMessage msg, List<string> metaList)
        {
            if (msg.Response)
            {
                Topic.MetaTags = metaList;

                GetInformation();
            }
            else
            {
                RunErrorMessage();
            }
        }

        internal static void SetInfo(WikiMessage msg)
        {
            if (msg.Response && msg.ReplyText.Length > 0)
            {
                Topic.Information = msg.ReplyText;

                SubmitTopic();
            }
            else
            {
                if (!msg.Response)
                {
                    RunErrorMessage();
                }
                else
                {
                    Topic.Information = "[Edit File] - To add information on this file!";

                    SubmitTopic();
                }
            }
        }

        internal static void SubmitReply(WikiMessage msg)
        {
            if (msg.Response)
            {
                if (IsEditMode)
                {
                    Topic.EditToLibrary(EditTopic);
                }
                else
                {
                    Topic.AddToLibrary(WikiDir.MoveFile);
                }

                WikiHelper.WikiForm.CloseMessage();
            }
            else
            {
                if (!msg.Response)
                {
                    RunErrorMessage();
                }
                else
                {
                    WikiHelper.WikiForm.CloseMessage();
                }
            }
        }

        static void RunErrorMessage()
        {
            string content = "[Failed]-> Setup Incomplete";

            WikiMessage message = WikiHelper.WikiForm.WikiMsg;

            message.Message = content;
            message.Reply = WikiMessage.ReplyType.SystemMsg;
            message.Btn1 = "OK";
            message.Btn2 = "Cancel";
            message.ShowBtn2 = false;
            message.IsMultiLine = false;
            message.IsReplyVisible = false;
            message.RunFinalMsg = false;

            message.StartMessage();
        }

        static void GetFile()
        {
            string content = "Adding File? or Link?";

            WikiMessage message = WikiHelper.WikiForm.WikiMsg;

            message.Message = content;
            message.Reply = WikiMessage.ReplyType.UploadTopic;
            message.Btn1 = "File";
            message.Btn2 = "Link";
            message.ShowBtn2 = true;
            message.IsMultiLine = false;
            message.IsReplyVisible = false;
            message.RunFinalMsg = true;

            message.StartMessage();
        }

        static void GetLink()
        {
            string content = $"Enter URL (web address) for {Topic.Title}";

            WikiMessage message = WikiHelper.WikiForm.WikiMsg;

            message.Message = content;
            message.Reply = WikiMessage.ReplyType.UploadLink;
            message.Btn1 = "OK";
            message.Btn2 = "Cancel";
            message.ShowBtn2 = true;
            message.IsMultiLine = false;
            message.IsReplyVisible = true;
            message.RunFinalMsg = true;

            message.StartMessage();
        }

        static void GetMeta()
        {
            string function = IsEditMode ? "Edit" : "Add";

            string content = $"{function} connected topics, separated by commas (topic1, topic2)";

            WikiMessage message = WikiHelper.WikiForm.WikiMsg;

            message.Message = content;
            message.Reply = WikiMessage.ReplyType.AddMeta;
            message.MetaList = IsEditMode ? Topic.GetMetaString(false) : $"{ScrapedMeta}";
            message.Btn1 = "OK";
            message.Btn2 = "Cancel";
            message.ShowBtn2 = true;
            message.IsMultiLine = true;
            message.IsReplyVisible = true;
            message.RunFinalMsg = true;

            message.StartMessage();
        }

        static void GetInformation()
        {
            string function = IsEditMode ? "Edit" : "Add";

            string content = $"{function} description for {Topic.GetCleanFileName(true)}";

            WikiMessage message = WikiHelper.WikiForm.WikiMsg;

            message.Message = content;
            message.Reply = WikiMessage.ReplyType.AddInfo;
            message.Btn1 = "OK";
            message.Btn2 = "Cancel";
            message.PreLoadRepy = Topic.Information;
            message.ShowBtn2 = true;
            message.IsMultiLine = true;
            message.IsReplyVisible = true;
            message.RunFinalMsg = true;

            message.StartMessage();
        }

        static void SubmitTopic()
        {
            string function = IsEditMode ? "[Edit]" : string.Empty;

            string content = $"Submit {Topic.Title} {function} : {Topic.GetCleanFileName(true)}";

            WikiMessage message = WikiHelper.WikiForm.WikiMsg;

            message.Message = content;
            message.Reply = WikiMessage.ReplyType.SubmitTopic;
            message.Btn1 = "Submit";
            message.Btn2 = "Cancel";
            message.ShowBtn2 = true;
            message.IsMultiLine = false;
            message.IsReplyVisible = false;
            message.RunFinalMsg = true;

            message.StartMessage();
        }

        static bool ScrapeFile => WikiDir.ScrapeFile;
        static int ScrapeLetter => WikiDir.ScrapeLetter;
        static int ScrapeLevel => WikiDir.ScrapeFrequency;

        static string[] UpdateComonWordResource()
        {
            return Resources.CommonWords.Split('\n');
        }

        static string ScrapeMetaData(WikiTopic topic, string path)
        {
            bool isText = topic.FileType.ToUpper() == "TXT";

            if (!isText)
                isText = topic.FileType.ToUpper() == "RTF";

            if (ScrapeFile && isText)
            {
                Dictionary<string, int> DuplicateWords = new Dictionary<string, int>();

                List<string> CleanEntries = new List<string>();

                List<string> Enteries = new List<string>();

                if (topic.FileType.ToUpper() == "TXT" && File.Exists(path))
                {
                    CleanEntries = File.ReadAllText(path).Replace('\n', ' ').Replace('\r', ' ').Split(' ').ToList();
                }
                else
                {
                    RichTextBox richTextBox = new RichTextBox();

                    richTextBox.LoadFile(path, RichTextBoxStreamType.RichText);

                    string cleanText = richTextBox.Text.Replace('\n', ' ').Replace('\r', ' ');

                    CleanEntries = cleanText.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                if (CleanEntries != null)
                {
                    List<string> commonWords = LoadCommonWords(WikiDir);

                    foreach (string word in CleanEntries)
                    {
                        if (word.Length > ScrapeLetter)
                        {
                            if (!Enteries.Contains(word))
                            {
                                if (word.All(char.IsLetter))
                                {
                                    if (!commonWords.Contains(word.ToUpper()))
                                    {
                                        {
                                            Enteries.Add(word);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (DuplicateWords.ContainsKey(word.ToUpper()))
                                {
                                    DuplicateWords[word.ToUpper()]++;
                                }
                                else
                                {
                                    DuplicateWords.Add(word.ToUpper(), 1);
                                }
                            }
                        }
                    }

                    if (DuplicateWords.Count > 0)
                    {
                        Enteries = new List<string>();

                        foreach (var duplicate in DuplicateWords)
                        {
                            if (duplicate.Value > ScrapeLevel)
                            {
                                Enteries.Add(duplicate.Key);
                            }
                        }
                    }
                }

                StringBuilder sb = new StringBuilder();

                int charCounter = 0;

                foreach (string entry in Enteries)
                {
                    charCounter += entry.Length;

                    if (charCounter < 950)
                        sb.Append($"{entry}, ");
                }

                return sb.ToString().TrimEnd();
            }
            else
            {
                return string.Empty;
            }
        }

        static List<string> LoadCommonWords(WikiDirectory wikiDirectory)
        {
            List<string> commonWords = new List<string>();

            string[] excludedWordList;

            string path = $@"{wikiDirectory.MyWikiFolder}\CommonWords.txt";

            if (File.Exists(path))
            {
                excludedWordList = File.ReadAllLines(path);
            }
            else
            {
                if (UpdateComonWordResource() != null)
                    excludedWordList = UpdateComonWordResource();
                else
                    excludedWordList = new string[0];
            }

            foreach (string word in excludedWordList)
            {
                if (word.Contains('='))
                {
                    string getWord = word.Split('=').Last();

                    if (getWord.Contains('\n'))
                        getWord = getWord.Replace("\n", "");

                    if (getWord.Contains('\r'))
                        getWord = getWord.Replace("\r", "");

                    if (getWord.Length > 0)
                    {
                        commonWords.Add(getWord.ToUpper());
                    }
                }
            }

            return commonWords;
        }

        internal static WikiTopic GetTopic(int id)
        {
            WikiTopic topic = new WikiTopic(id);

            if (topic.ID != -1)
            {
                return topic;
            }
            else
            {
                return null;
            }
        }
    }
}
