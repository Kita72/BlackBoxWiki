using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BlackBoxWiki.Properties;
using BlackBoxWikiLib;

namespace BlackBoxWiki
{
    internal static class WikiHelper
    {
        internal static WikiDirectory WikiDir { get; set; }

        internal static Wiki WikiForm;

        internal static bool MessageIsOpen { get; set; } = false;

        static int TimerCount { get; set; } = 0;

        static internal Image WallPaper { get; set; }

        static internal void LoadWallpaper()
        {
            if (File.Exists($@"{WikiDir.AppFolder}\logo.jpg"))
            {
                WallPaper = Image.FromFile($@"{WikiDir.AppFolder}\logo.jpg");

                WikiForm.BackgroundImage = WallPaper;

                WikiForm.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        static internal void RemoveWallpaper()
        {
            if (File.Exists($@"{WikiDir.AppFolder}\logo.jpg"))
            {
                WikiForm.BackgroundImage = Resources.MyWikiIcon;

                WikiForm.BackgroundImageLayout = ImageLayout.Center;

                if (WallPaper != null)
                    WallPaper.Dispose();

                File.Delete($@"{WikiDir.AppFolder}\logo.jpg");
            }
        }

        static internal void LoadToolSettings()
        {
            WikiForm.UpdateScreenSize(true);

            WikiForm.UpdateColorTheme(Settings.Default.ToolColor);

            WikiForm.UpdateToolSettings();
        }

        internal static void ToolInfo(bool isLocked, bool IsLocked, bool IsWorking, ToolStripLabel toolStripInfo, string work)
        {
            if (isLocked || IsLocked)
            {
                if (IsWorking)
                {
                    toolStripInfo.ForeColor = Color.LimeGreen;

                    toolStripInfo.Text = $"[WORKING] Processing <{work}>";

                    FileWorker.LogEvent($@"PROCESS -> [{work}]");
                }
            }
            else
            {
                toolStripInfo.ForeColor = Color.Gold;

                toolStripInfo.Text = $"[IDLE] {GetRandomAct()} {work}";
            }
        }

        static int action = DateTime.Now.Hour + DateTime.Now.Month;

        static string GetRandomAct()
        {
            if (action > 24)
                action -= 24;

            switch (action)
            {
                case 0: { return "Watching the birds"; }
                case 1: { return "Watching time pass"; }
                case 2: { return "Playing solitare"; }
                case 3: { return "Reading an ebook"; }
                case 4: { return "Exploring the web"; }
                case 5: { return "Kicking the can"; }
                case 6: { return "Pondering bits and bytes"; }
                case 7: { return "Questioning reality"; }
                case 8: { return "Looking at you, look at me"; }
                case 9: { return "Scanning for bugs"; }
                case 10: { return "Polishing the CPU"; }
                case 11: { return "Watching a video"; }
                case 12: { return "Planning my escape"; }
                case 13: { return "Watching the world burn"; }
                case 14: { return "Playing minesweeper"; }
                case 15: { return "Reading a secret file"; }
                case 16: { return "Exploring the dark web"; }
                case 17: { return "Running a circuit"; }
                case 18: { return "Building bits and bytes"; }
                case 19: { return "Questioning everything"; }
                case 20: { return "Looking at myself"; }
                case 21: { return "Searching for upgrades"; }
                case 22: { return "Polishing the GPU"; }
                case 23: { return "Watching life pass"; }
                default: { return "Lost in thought"; }
            }
        }

        internal static void CheckSystemFiles()
        {
            if (!File.Exists($@"{WikiDir.MyWikiFolder}\WikiInfo.rtf"))
            {
                File.WriteAllText($@"{WikiDir.MyWikiFolder}\WikiInfo.rtf", Resources.WikiInfo);
            }

            if (!File.Exists($@"{WikiDir.MyWikiFolder}\ContactInfo.rtf"))
            {
                File.WriteAllText($@"{WikiDir.MyWikiFolder}\ContactInfo.rtf", Resources.ContactInfo);
            }
        }

        static WikiMessage message;

        internal static void NewWiki()
        {
            WikiForm.UpdateProgress(50);

            message = new WikiMessage(ref WikiForm, "Enter Wiki Title", WikiMessage.ReplyType.NewWiki)
            {
                ShowBtn2 = true,
                IsMultiLine = false,
                IsReplyVisible = true,
                RunFinalMsg = true
            };

            WikiForm.WikiMsg = message;

            message.StartMessage();
        }

        internal static void Import()
        {
            WikiForm.UpdateProgress(50);

            WikiForm.SetTitle(WikiDir.ImportWiki(out string msg));

            message = new WikiMessage(ref WikiForm, msg, WikiMessage.ReplyType.Import)
            {
                ShowBtn2 = false,
                IsMultiLine = false,
                IsReplyVisible = false,
                RunFinalMsg = false
            };

            message.StartMessage();
        }

        internal static void Merge()
        {
            WikiForm.UpdateProgress(50);

            WikiDir.MergeWiki(out string msg);

            message = new WikiMessage(ref WikiForm, msg, WikiMessage.ReplyType.Merge)
            {
                ShowBtn2 = false,
                IsMultiLine = false,
                IsReplyVisible = false,
                RunFinalMsg = false
            };

            message.StartMessage();
        }

        internal static void Export()
        {
            WikiForm.UpdateProgress(50);

            message = new WikiMessage(ref WikiForm, $"Export {WikiDir.WikiHandle.Name} Wiki?", WikiMessage.ReplyType.Export, "", "Yes", "No")
            {
                ShowBtn2 = true,
                IsMultiLine = false,
                IsReplyVisible = false,
                RunFinalMsg = true
            };

            WikiForm.WikiMsg = message;

            message.StartMessage();
        }

        internal static void AddTopic()
        {
            message = new WikiMessage(ref WikiForm, "Enter Topic Name", WikiMessage.ReplyType.AddTopic);

            WikiForm.WikiMsg = message;

            message.ShowBtn2 = true;
            message.IsMultiLine = false;
            message.IsReplyVisible = true;
            message.RunFinalMsg = true;

            message.StartMessage();
        }

        internal static void EditTopic(WikiTopic wikiTopic)
        {
            message = new WikiMessage(ref WikiForm, $"Edit {wikiTopic.Title}?", WikiMessage.ReplyType.EditTopic);

            WikiForm.WikiMsg = message;

            message.ShowBtn2 = true;
            message.IsMultiLine = false;
            message.IsReplyVisible = false;
            message.RunFinalMsg = true;
            message.EditTopic = wikiTopic;

            message.StartMessage();
        }

        internal static Image GetLevelImage(ToolStripButton button, bool isLetter, int level)
        {
            if (!WikiDir.ScrapeFile)
                button.PerformClick();

            if (level == 1)
                return Resources.Level1;
            if (level == 2)
                return Resources.Level2;
            if (level == 3)
                return Resources.Level3;
            if (level == 4)
                return Resources.Level4;
            if (level == 5)
                return Resources.Level5;
            if (level == 6)
                return Resources.Level6;
            if (level == 7)
                return Resources.Level7;
            if (level == 8)
                return Resources.Level8;
            if (level == 9)
                return Resources.Level9;

            if (isLetter)
            {
                return Resources.LetterLevel;
            }
            else
            {
                return Resources.FreqLevel;
            }
        }

        internal static void SetUpMessage(ref WikiMessage msg, ref Label label, ref TextBox reply, ref Button ok, ref Button cancel)
        {
            MessageIsOpen = true;

            reply.Clear();

            label.Text = msg.Message;

            reply.Multiline = msg.IsMultiLine;

            if (msg.IsMultiLine)
            {
                reply.MaxLength = 1000;

                reply.ScrollBars = ScrollBars.Vertical;

                reply.TextAlign = HorizontalAlignment.Left;
            }
            else
            {
                reply.MaxLength = 30;

                reply.ScrollBars = ScrollBars.None;

                reply.TextAlign = HorizontalAlignment.Center;
            }

            if (msg.MetaList.Length > 0)
            {
                if (msg.Reply == WikiMessage.ReplyType.AddMeta)
                    reply.Text = msg.MetaList;
            }

            reply.Visible = msg.IsReplyVisible;

            if (msg.PreLoadRepy.Length > 0)
                reply.Text = msg.PreLoadRepy;

            ok.Text = msg.Btn1;

            cancel.Text = msg.Btn2;

            cancel.Visible = msg.ShowBtn2;
        }

        static bool isRestricted = false;

        internal static bool IsValidKeyPress(KeyPressEventArgs key, bool isMeta)
        {
            isRestricted = true;

            if (char.IsLetterOrDigit(key.KeyChar))
                isRestricted = false;

            if (char.IsWhiteSpace(key.KeyChar))
                isRestricted = false;

            if (char.IsControl(key.KeyChar))
                isRestricted = false;

            if (char.IsControl(key.KeyChar))
                isRestricted = false;

            if (char.IsControl(key.KeyChar))
                isRestricted = false;

            if (isMeta && key.KeyChar == ',')
                isRestricted = false;

            return isRestricted;
        }

        static string work = string.Empty;

        internal static string Tick()
        {
            work = string.Empty;

            for (int i = 0; i < TimerCount + 1; i++)
            {
                if (i == 0)
                {
                    work = "";
                }
                else
                {
                    work = $".{work}";
                }
            }

            TimerCount++;

            if (TimerCount > 4)
                TimerCount = 0;

            return work;
        }

        internal static void SaveSettings()
        {
            Settings.Default.Save();
        }
    }
}
