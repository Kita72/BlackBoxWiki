using BlackBoxWikiLib;
using System;
using System.Text;

namespace BlackBoxWiki
{
    internal class WikiMessage
    {
        Wiki WikiForm { get; set; }

        internal string Message { get; set; }

        internal string ReplyText { get; set; }

        internal ReplyType Reply { get; set; }

        internal enum ReplyType
        {
            NewWiki,
            Import,
            Merge,
            Export,
            AddTopic,
            EditTopic,
            UploadTopic,
            UploadLink,
            AddMeta,
            AddInfo,
            SubmitTopic,
            SystemMsg
        }

        internal string MetaList { get; set; }

        internal string Btn1 { get; set; }

        internal string Btn2 { get; set; }

        internal bool ShowBtn2 { get; set; } = true;

        internal bool IsMultiLine { get; set; } = false;

        internal bool IsReplyVisible { get; set; } = false;

        internal bool RunFinalMsg { get; set; } = false;

        internal bool Response { get; set; } = false;

        internal WikiTopic EditTopic { get; set; }

        internal string PreLoadRepy { get; set; } = string.Empty;

        internal WikiMessage(ref Wiki wiki, string message, ReplyType replyType, string metaList = "", string btn1 = "OK", string btn2 = "Cancel")
        {
            WikiForm = wiki;
            Message = message;
            Reply = replyType;
            MetaList = metaList;
            Btn1 = btn1;
            Btn2 = btn2;
        }

        internal void StartMessage()
        {
            WikiForm.UpdateProgress(10);

            WikiForm.OpenMessage(this);

            FileWorker.LogEvent(ToString());
        }

        internal void FinishMessage()
        {
            RunFinalMsg = false;

            WikiForm.UpdateProgress(10);

            switch (Reply)
            {
                case ReplyType.NewWiki:
                    {
                        WikiForm.SetTitle(WikiHelper.WikiDir.StartNewWiki(ReplyText, out string message));

                        Message = message;

                        Btn1 = "OK";
                        IsReplyVisible = false;
                        ShowBtn2 = false;

                        WikiForm.UpdateProgress(75);

                        WikiForm.OpenMessage(this);

                        break;
                    }
                case ReplyType.Import: //No Reply
                    {
                        break;
                    }
                case ReplyType.Merge: //No Reply
                    {
                        break;
                    }
                case ReplyType.Export:
                    {
                        WikiHelper.WikiDir.ExportWiki(Response, out string message);

                        Message = message;

                        Btn1 = "OK";
                        IsReplyVisible = false;
                        ShowBtn2 = false;

                        WikiForm.OpenMessage(this);

                        WikiForm.UpdateProgress(75);

                        if (!Message.StartsWith("Failed"))
                        {
                            WikiForm.MessageOK_Click("Export", EventArgs.Empty);

                            WikiForm.SendToScreen(ControlManager.WebControl(@"https://www.filehosting.org/").Handle);
                        }

                        break;
                    }
                case ReplyType.AddTopic:
                    {
                        TopicManager.StartNewTopic(this);

                        break;
                    }
                case ReplyType.EditTopic:
                    {
                        TopicManager.StartEditTopic(EditTopic, this);

                        break;
                    }
                case ReplyType.UploadTopic:
                    {
                        TopicManager.GetFileReply(this);

                        break;
                    }
                case ReplyType.UploadLink:
                    {
                        TopicManager.GetLinkReply(this);

                        break;
                    }
                case ReplyType.AddMeta:
                    {
                        ReplyText = $"{ReplyText}".Trim().Trim(',');

                        TopicManager.GetMetaReply(this);

                        break;
                    }
                case ReplyType.AddInfo:
                    {
                        TopicManager.SetInfo(this);

                        break;
                    }
                case ReplyType.SubmitTopic:
                    {
                        TopicManager.SubmitReply(this);

                        break;
                    }
                case ReplyType.SystemMsg: //No Reply
                    {
                        break;
                    }
            }

            FileWorker.LogEvent(ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Message -> [");
            sb.Append($"{Message} : {Reply} : {ReplyText} : ");
            sb.Append($"{Response} : {Btn1} : {Btn2} : {ShowBtn2} : ");
            sb.Append($"{IsReplyVisible} : {IsMultiLine} : {RunFinalMsg}]");

            return sb.ToString();
        }
    }
}
