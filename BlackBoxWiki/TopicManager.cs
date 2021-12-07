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

            SetMeta(msg, msg.ReplyText.Split(',').ToList());
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

        static WikiMessage message;

        static string function;

        static string content;

        static void RunErrorMessage()
        {
            content = "[Failed]-> Setup Incomplete";

            message = WikiHelper.WikiForm.WikiMsg;

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
            content = "Adding File? or Link?";

            message = WikiHelper.WikiForm.WikiMsg;

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
            content = $"Enter URL (web address) for {Topic.Title}";

            message = WikiHelper.WikiForm.WikiMsg;

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
            function = IsEditMode ? "Edit" : "Add";

            content = $"{function} connected topics, separated by commas (topic1, topic2)";

            message = WikiHelper.WikiForm.WikiMsg;

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
            function = IsEditMode ? "Edit" : "Add";

            content = $"{function} description for {Topic.GetCleanFileName(true)}";

            message = WikiHelper.WikiForm.WikiMsg;

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
            function = IsEditMode ? "[Edit]" : string.Empty;

            content = $"Submit {Topic.Title} {function} : {Topic.GetCleanFileName(true)}";

            message = WikiHelper.WikiForm.WikiMsg;

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

        static string[] UpdateComonWordResource(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllLines(path);
            }
            else
            {
                return Resources.CommonWords.Split('\n');
            }
        }

        static RichTextBox richTextBox;

        static Dictionary<string, int> duplicateWords;

        static List<string> cleanEntries;

        static List<string> enteries;

        static StringBuilder sb;

        static bool isText;

        static string cleanText;

        static List<string> commonWords;

        static int charCounter;

        static string ScrapeMetaData(WikiTopic topic, string path)
        {
            isText = topic.FileType.ToUpper() == "TXT";

            if (!isText)
                isText = topic.FileType.ToUpper() == "RTF";

            if (ScrapeFile && isText)
            {
                duplicateWords = new Dictionary<string, int>();

                cleanEntries = new List<string>();

                enteries = new List<string>();

                if (topic.FileType.ToUpper() == "TXT" && File.Exists(path))
                {
                    cleanText = File.ReadAllText(path).Replace('\n', ' ').Replace('\r', ' ');

                    cleanEntries = cleanText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    richTextBox = new RichTextBox();

                    richTextBox.LoadFile(path);

                    cleanText = richTextBox.Text.Replace('\n', ' ').Replace('\r', ' ');

                    cleanEntries = cleanText.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                if (cleanEntries != null)
                {
                    commonWords = LoadCommonWords(WikiDir);

                    foreach (string word in cleanEntries)
                    {
                        if (word.Length > ScrapeLetter)
                        {
                            if (!enteries.Contains(word))
                            {
                                if (word.All(char.IsLetter))
                                {
                                    if (!commonWords.Contains(word.ToUpper()))
                                    {
                                        {
                                            enteries.Add(word);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (duplicateWords.ContainsKey(word.ToUpper()))
                                {
                                    duplicateWords[word.ToUpper()]++;
                                }
                                else
                                {
                                    duplicateWords.Add(word.ToUpper(), 1);
                                }
                            }
                        }
                    }

                    if (duplicateWords.Count > 0)
                    {
                        enteries = new List<string>();

                        foreach (var duplicate in duplicateWords)
                        {
                            if (duplicate.Value > ScrapeLevel)
                            {
                                enteries.Add(duplicate.Key);
                            }
                        }
                    }
                }

                sb = new StringBuilder();

                charCounter = 0;

                foreach (string entry in enteries)
                {
                    charCounter += entry.Length;

                    if (charCounter < 950)
                        sb.Append($"{entry}, ");
                }

                duplicateWords.Clear();

                cleanEntries.Clear();

                enteries.Clear();

                return sb.ToString().TrimEnd();
            }
            else
            {
                return string.Empty;
            }
        }

        static List<string> commonWordList;

        static string commonPath;

        static string[] excludedWordList;

        static List<string> LoadCommonWords(WikiDirectory wikiDirectory)
        {
            commonWordList = new List<string>();

            commonPath = $@"{wikiDirectory.MyWikiFolder}\CommonWords.txt";

            excludedWordList = UpdateComonWordResource(commonPath);

            foreach (string word in excludedWordList)
            {
                string cleanedWord = word;

                if (cleanedWord.Contains('\n'))
                    cleanedWord = word.Replace("\n", "");

                if (cleanedWord.Contains('\r'))
                    cleanedWord = cleanedWord.Replace("\r", "");

                if (cleanedWord.Length > 0)
                {
                    commonWordList.Add(cleanedWord.ToUpper());
                }
            }

            return commonWordList;
        }

        static WikiTopic getTopic;

        internal static WikiTopic GetTopic(int id)
        {
            getTopic = new WikiTopic(id);

            if (getTopic.ID != -1)
            {
                return getTopic;
            }
            else
            {
                return null;
            }
        }
    }
}
