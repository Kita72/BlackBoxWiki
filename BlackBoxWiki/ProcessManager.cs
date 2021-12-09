using BlackBoxWikiLib;
using System.Collections.Generic;
using System.Diagnostics;

namespace BlackBoxWiki
{
    public enum FileTypes
    {
        Unknown,
        Text,
        Rich,
        Image,
        Video,
        Web
    }

    internal static class ProcessManager
    {
        static FileTypes FileType { get; set; }
        internal static WikiDirectory WikiDir => WikiHelper.WikiDir;

        static readonly List<string> textTypes = new List<string> { "TXT" };
        static readonly List<string> richTextTypes = new List<string> { "RTF" };
        static readonly List<string> imageTypes = new List<string> { "JPG", "BMP", "GIF", "PNG", "TIFF", "JPEG" };
        static readonly List<string> videoTypes = new List<string> { "MP4", "MOV", "WMV", "FLV", "AVI", "WEBM", "MKV" };
        static readonly List<string> linkTypes = new List<string> { "HTTPS", "HTTP" };


        static FileTypes GetFileType(string type)
        {
            if (textTypes.Contains(type.ToUpper()))
                return FileTypes.Text;
            if (richTextTypes.Contains(type.ToUpper()))
                return FileTypes.Rich;
            if (imageTypes.Contains(type.ToUpper()))
                return FileTypes.Image;
            if (videoTypes.Contains(type.ToUpper()))
                return FileTypes.Video;
            if (linkTypes.Contains(type.ToUpper()))
                return FileTypes.Web;
            else
                return FileTypes.Unknown;
        }

        internal static WikiControl GetControl(WikiTopic topic)
        {
            if (topic != null)
            {
                return ControlPicker(topic);
            }

            return null;
        }

        static WikiControl ControlPicker(WikiTopic topic)
        {
            FileType = GetFileType(topic.FileType);

            FileWorker.LogEvent("process", "file", $"=> {FileType}");

            WikiControl control = null;

            switch(FileType)
            {
                case FileTypes.Text:
                    {
                        if (WikiDir.ReadText(topic.GetStoredFileName()).Contains("http"))
                        {
                            try
                            {
                                string cleanText = WikiDir.ReadText(topic.GetStoredFileName());

                                control = ControlManager.RichControl(cleanText, false, true);

                                break;
                            }
                            catch
                            {
                                //Log Error
                            }
                        }

                        control = ControlManager.TextControl(topic.GetStoredFileName());

                        break;
                    }
                case FileTypes.Rich:
                    {
                        control = ControlManager.RichControl(topic.GetStoredFileName(), true, false);

                        break;
                    }
                case FileTypes.Image:
                    {
                        control = ControlManager.ImageControl(topic.GetStoredFileName());

                        break;
                    }
                case FileTypes.Video:
                    {
                        control = ControlManager.MediaControl(topic.GetStoredFileName());

                        break;
                    }
                case FileTypes.Web:
                    {
                        control = ControlManager.WebControl(topic.GetLink());

                        break;
                    }
                case FileTypes.Unknown:
                    {
                        RunProcess(topic);

                        break;
                    }
            }

            return control;
        }

        internal static void RunProcess(WikiTopic wikiTopic)
        {
            try
            {
                Process.Start(wikiTopic.GetStoredFileName());
            }
            catch
            {
                //log error
            }
        }
    }
}
