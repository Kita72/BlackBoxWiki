using System.IO;
using System.Text;

namespace BlackBoxWikiLib
{
    public static class WikiUtility
    {
        public static string ConvertToSys(string text)
        {
            return text.Replace(' ', '_').ToUpper();
        }

        public static string ConvertFromSys(string text)
        {
            return FirstCharToUpper(text.Replace('_', ' '));
        }

        static readonly StringBuilder TextBuilder = new StringBuilder();

        static string FirstCharToUpper(string input)
        {
            if (input.Length > 0)
            {
                TextBuilder.Clear();

                string[] multiWord = input.Split(' ');

                if (multiWord.Length > 0)
                {
                    for (int i = 0; i < multiWord.Length; i++)
                    {
                        TextBuilder.Append(multiWord[i][0].ToString().ToUpper() + multiWord[i].Substring(1).ToLower() + " ");
                    }
                }
                else
                {
                    TextBuilder.Append(input[0].ToString().ToUpper() + input.Substring(1).ToLower());
                }

                return TextBuilder.ToString().Trim();
            }
            else
            {
                return input;
            }
        }

        public static void ResetAll()
        {
            FileWorker.LogEvent("RESET -> [All Libraries]");

            TopicStore.ResetLibrary();

            SearchStore.ResetLibrary();
        }

        public static void SaveAll()
        {
            if (File.Exists(FileStore.TopicFile))
            {
                FileWorker.LogEvent("SAVE -> [Library - TOPIC]");

                TopicStore.SaveTopicData();
            }
            else
            {
                FileWorker.LogEvent("SAVE -> [Library - NONE]");
            }
        }
    }
}
