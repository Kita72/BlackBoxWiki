using System;
using System.Windows.Forms;
using BlackBoxWikiLib;
using BlackBoxWiki.Properties;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Drawing;
using System.Linq;

namespace BlackBoxWiki
{
    public partial class Wiki : Form
    {
        WikiDirectory WikiDir { get { return WikiHelper.WikiDir; } set { WikiHelper.WikiDir = value; } }

        bool FullSize { get; set; } = false;

        bool ToolTipControl { get; set; } = true;

        bool IsLocked { get; set; } = false;

        bool SingleClickControl { get; set; } = true;

        int OldValue { get; set; } = 0;

        WikiControl WikiCntrl { get; set; }

        internal WikiMessage WikiMsg { get; set; }

        internal Wiki()
        {
            InitializeComponent();

            WikiHelper.CheckSystemFiles();

            WikiHelper.WikiForm = this;

            wikiSystemWatcher.Path = WikiDir.AppFolder;

            WikiHelper.LoadWallpaper();

            WikiHelper.LoadToolSettings();

            UpdateToolInfo(false, "", false);

            SetTitle("MyWiki");
        }

        internal void UpdateToolInfo(bool IsWorking = false, string work = "...", bool isLocked = false)
        {
            WikiHelper.ToolInfo(isLocked, IsLocked, IsWorking, toolStripInfo, work);

            IsLocked = isLocked;

            wikiToolStrip.Enabled = !IsLocked;

            if (!wikiToolStrip.Enabled)
            {
                wikiToolStrip.Hide();
            }
            else
            {
                wikiToolStrip.Show();
            }

            if (IsLocked)
            {
                toolStripProgressBar.Value = 0;
            }
        }

        internal void UpdateProgress(int amount)
        {
            if (amount == 0)
            {
                toolStripProgressBar.Visible = false;

                toolStripProgressBar.Value = 0;

                OldValue = 0;
            }
            else
            {
                if (OldValue + amount > 99)
                {
                    toolStripProgressBar.Value = 100;

                }
                else
                {
                    toolStripProgressBar.Value = OldValue + amount;
                }

                toolStripProgressBar.Visible = true;

                OldValue = toolStripProgressBar.Value;
            }
        }

        void HomeLogo_Click(object sender, EventArgs e)
        {
            CloseScreens();

            ClearAllText();

            UpdateScreenSize(true);

            toolStripSearchText.Focus();
        }

        internal void SetTitle(string title)
        {
            if (title.Length > 2)
            {
                toolStripTitle.Text = title;

                int updatedWidth = 350 + (88 - toolStripTitle.Width);

                toolStripSearchText.Size = new Size(updatedWidth, toolStripSearchText.Height);

                if (toolStripSearchText.Width < toolStripDirectory.Width)
                {
                    int diff = toolStripDirectory.Width - toolStripSearchText.Width;

                    int newWidth = toolStripSearchText.Width + diff / 2;

                    toolStripSearchText.Size = new Size(newWidth, toolStripSearchText.Height);

                    toolStripDirectory.Size = new Size(newWidth, toolStripDirectory.Height);
                }
            }
        }

        internal string GetTitle()
        {
            return toolStripTitle.Text;
        }

        void NewWiki_Click(object sender, EventArgs e)
        {
            UpdateToolInfo(true, "New Wiki", true);

            WikiHelper.NewWiki();
        }

        void WikiImport_Click(object sender, EventArgs e)
        {
            CloseScreens();

            ClearAllText();

            UpdateToolInfo(true, "Wiki Import", true);

            WikiHelper.Import();
        }

        private void WikiMerge_Click(object sender, EventArgs e)
        {
            if (GetTitle() != toolStripTitle.Tag.ToString())
            {
                CloseScreens();

                ClearAllText();

                UpdateToolInfo(true, "Wiki Merge", true);

                WikiHelper.Merge();
            }
        }

        void WikiExport_Click(object sender, EventArgs e)
        {
            if (GetTitle() != toolStripTitle.Tag.ToString())
            {
                UpdateToolInfo(true, "Wiki Export", true);

                WikiHelper.Export();
            }
        }

        void WikiDelete_Click(object sender, EventArgs e)
        {
            UpdateToolInfo(true, "Remove Wiki", true);

            UpdateProgress(50);

            string reply = WikiPrompt.ShowDialog("Delete a Wiki?", "Delete Wiki", "YES", "NO");

            UpdateProgress(75);

            if (reply == "YES")
            {
                CloseScreens();

                ClearAllText();

                WikiDir.DeleteWiki(GetTitle(), out string msg);

                reply = WikiPrompt.ShowDialog(msg, "Remove Wiki", "Exit", "Info");
            }

            UpdateToolInfo(false, "", false);

            if (reply == "Info")
            {
                Manual_Click("Wiki Remove", EventArgs.Empty);
            }
        }

        void BackArrow_Click(object sender, EventArgs e)
        {
            if (WikiCntrl != null && WikiCntrl.FileType == FileTypes.Web)
            {
                WebView2 webControl = WikiCntrl.Handle as WebView2;

                if (webControl.CanGoBack)
                {
                    webControl.GoBack();
                }
                else
                {
                    if (SearchManager.CanGoBack())
                        SearchManager.GoBack();
                }
            }
            else
            {
                if (SearchManager.CanGoBack())
                    SearchManager.GoBack();
            }
        }

        void ForwardArrow_Click(object sender, EventArgs e)
        {
            if (WikiCntrl != null && WikiCntrl.FileType == FileTypes.Web)
            {
                WebView2 webControl = WikiCntrl.Handle as WebView2;

                if (webControl.CanGoForward)
                {
                    webControl.GoForward();
                }
                else
                {
                    if (SearchManager.CanGoForward())
                        SearchManager.GoForward();
                }
            }
            else
            {
                if (SearchManager.CanGoForward())
                    SearchManager.GoForward();
            }
        }

        internal void RunSearchRequest(string meta)
        {
            toolStripSearchText.Text = WikiUtility.ConvertFromSys(meta);

            toolStripSearch.PerformClick();
        }

        string GetInfoString()
        {
            if (SingleClickControl)
            {
                return "Topic Directory, [SINGLE CLICK] file/link to preview!";
            }
            else
            {
                return "Topic Directory, [DOUBLE CLICK] file/link to preview!";
            }
        }

        void SearchEntry_Click(object sender, EventArgs e)
        {
            if (toolStripSearchText.Text.Length > 0)
            {
                toolStripSearchText.Clear();
            }
        }

        void SearchButton_Click(object sender, EventArgs e)
        {
            SearchManager.Search(toolStripSearchText.Text);

            FileWorker.LogEvent($"Search -> [{toolStripSearchText.Text}]=> Start");
        }

        void SearchDirectory_Click(object sender, EventArgs e)
        {
            SearchManager.LoadSearchList(toolStripDirectory);

            if (toolStripDirectory.Items.Count > 0)
            {
                InfoTextBox.Text = GetInfoString();
            }
            else
            {
                toolStripSearchText.Focus();
            }
        }

        void SearchDirectory_IndexUpdated(object sender, EventArgs e)
        {
            if (toolStripDirectory.SelectedIndex != -1)
            {
                SearchManager.Search(toolStripDirectory.SelectedItem.ToString());
            }
        }

        void SearchList_Click(object sender, EventArgs e)
        {
            if (SingleClickControl)
            {
                SearchListProcessClick();
            }
            else
            {
                if (SearchListBox.SelectedIndex != -1 && SearchListBox.SelectedIndex != 0)
                {
                    int topicID = Convert.ToInt32(SearchListBox.SelectedValue);

                    WikiTopic topic = TopicManager.GetTopic(topicID);

                    if (topic != null)
                    {
                        InfoTextBox.Text = topic.Information;
                    }
                    else
                    {
                        InfoTextBox.Text = GetInfoString();
                    }
                }
                else
                {
                    InfoTextBox.Text = GetInfoString();
                }
            }
        }

        void SearchList_DoubleClick(object sender, EventArgs e)
        {
            if (!SingleClickControl)
            {
                SearchListProcessClick();
            }
        }

        int LastClicked { get; set; } = -1;

        void SearchListProcessClick()
        {
            if (LastClicked != SearchListBox.SelectedIndex)
            {
                LastClicked = SearchListBox.SelectedIndex;

                if (SearchListBox.SelectedIndex != -1 && SearchListBox.SelectedIndex != 0)
                {
                    int topicID = Convert.ToInt32(SearchListBox.SelectedValue);

                    WikiTopic topic = TopicManager.GetTopic(topicID);

                    if (topic != null)
                    {
                        CloseScreens();

                        WikiCntrl = ProcessManager.GetControl(topic);

                        if (WikiCntrl != null)
                        {
                            UpdateControlType(WikiCntrl);

                            SearchScreen(WikiCntrl.Handle);

                            InfoTextBox.Text = topic.Information;

                            FileWorker.LogEvent($"SEARCH -> [Search List]=> {topic.Display}");
                        }
                        else
                        {
                            FileWorker.LogEvent($"FAILED -> [Search List]=> {topic.Display}");
                        }
                    }
                }
                else
                {
                    InfoTextBox.Text = GetInfoString();

                    if (SearchListBox.SelectedIndex == 0)
                    {
                        string directoryName = SearchListBox.GetItemText(SearchListBox.Items[0]);

                        directoryName = directoryName.Replace("Directory [", "").TrimEnd(']').Trim();

                        toolStripSearchText.Text = directoryName;

                        toolStripSearch.PerformClick();
                    }
                }

                LastClicked = -1;
            }
        }

        internal void UpdateAutoSearch(AutoCompleteStringCollection collection)
        {
            toolStripSearchText.AutoCompleteCustomSource = collection;

            replyTextBox.AutoCompleteCustomSource = collection;
        }

        void RichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (e.LinkText.Contains("http"))
            {
                string wikiLink = string.Empty;

                if (e.LinkText.ToUpper().Contains("MYWIKI"))
                {
                    wikiLink = e.LinkText.Split('#').Last();
                }

                string link = wikiLink == string.Empty ? e.LinkText : wikiLink;

                if (link == e.LinkText)
                {
                    SendToScreen(ControlManager.WebControl(link));
                }
                else
                {
                    if (link.Length > 0)
                    {
                        toolStripSearchText.Text = link;

                        toolStripSearch.PerformClick();
                    }
                }

                FileWorker.LogEvent($"RUN -> [{link}]=> RichText(link)");
            }
        }

        void WebView_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            if (sender != null)
            {
                WebView2 webView = sender as WebView2;

                if (webView.Source != null)
                    Clipboard.SetText(webView.Source.OriginalString);
            }
        }

        internal void UpdateSearchListBox(ArrayList source, string display, string id)
        {
            SearchListBox.BeginUpdate();

            SearchListBox.ValueMember = id;

            SearchListBox.DisplayMember = display;

            SearchListBox.DataSource = source;

            SearchListBox.EndUpdate();

            SearchListBox.Refresh();

            UpdateScreenSize(false);
        }

        internal Control GetMediaHandle(string video)
        {
            wikiMediaPlayer.Dock = DockStyle.Fill;

            wikiMediaPlayer.Visible = true;

            wikiMediaPlayer.URL = video;

            return wikiMediaPlayer;
        }

        internal void CloseMediaPlayer()
        {
            wikiMediaPlayer.Ctlcontrols.stop();

            wikiMediaPlayer.Dock = DockStyle.None;

            wikiMediaPlayer.Visible = false;
        }

        internal void RunWebSearch(string source)
        {
            WikiControl control = ControlManager.WebControl(source);

            SendToScreen(control);

            FileWorker.LogEvent("SEARCH -> [WebBrowser]=> Link");
        }

        void AddTopic_Click(object sender, EventArgs e)
        {
            if (WikiDir.WikiFolder != null)
            {
                UpdateToolInfo(true, "New Topic", true);

                WikiHelper.AddTopic();
            }
        }

        void OpenTopic_Click(object sender, EventArgs e)
        {
            if (SearchListBox.Visible && SearchListBox.SelectedIndex != -1)
            {
                int topicID = Convert.ToInt32(SearchListBox.SelectedValue);

                WikiTopic topic = TopicManager.GetTopic(topicID);

                if (topic != null)
                {
                    CloseScreens();

                    ProcessManager.RunProcess(topic);

                    ClearAllText();
                }
            }
        }

        void EditTopic_Click(object sender, EventArgs e)
        {
            if (SearchListBox.Visible && SearchListBox.SelectedIndex != -1)
            {
                int topicID = Convert.ToInt32(SearchListBox.SelectedValue);

                WikiTopic topic = TopicManager.GetTopic(topicID);

                if (topic != null)
                {
                    CloseScreens();

                    UpdateToolInfo(true, "Edit Topic", true);

                    WikiHelper.EditTopic(topic);

                    ClearAllText();
                }
                else
                {
                    string Reply = WikiPrompt.ShowDialog("Not able to edit this file/link!", "Edit Topic", "OK", "Info");

                    if (Reply == "Info")
                    {
                        YouTube_Click(sender, e);
                    }
                }
            }
        }

        void RemoveTopic_Click(object sender, EventArgs e)
        {
            if (SearchListBox.Visible && SearchListBox.SelectedIndex != -1)
            {
                int topicID = Convert.ToInt32(SearchListBox.SelectedValue);

                WikiTopic topic = TopicManager.GetTopic(topicID);

                if (topic != null && topic.ID > 0)
                {
                    CloseScreens();

                    topic.RemovefromLibrary(WikiDir.BackUpFolder);

                    ClearAllText();

                    SearchListBox.DataSource = null;

                    SearchListBox.Refresh();

                    UpdateScreenSize(true);
                }
            }
        }

        internal void UpdateToolSettings()
        {
            WikiDir.MoveFile = !Settings.Default.MoveFile;
                toolStripCopy.PerformClick();

            WikiDir.ScrapeFile = !Settings.Default.ScrapeFile;
                toolStripScrape.PerformClick();

            WikiDir.ScrapeLetter = Settings.Default.ScrapeLevel;
                toolStripLetterLevel.DropDownItems[WikiDir.ScrapeLetter].PerformClick();

            WikiDir.ScrapeFrequency = Settings.Default.ScrapeFreq;
                toolStripFreqLevel.DropDownItems[WikiDir.ScrapeFrequency].PerformClick();
        }

        void CopyMove_Click(object sender, EventArgs e)
        {
            if (!WikiDir.MoveFile)
            {
                toolStripCopy.Text = "Move File";

                toolStripCopy.Image = Resources.MoveFileNew;

                WikiDir.MoveFile = true;
            }
            else
            {
                toolStripCopy.Text = "Copy File";

                toolStripCopy.Image = Resources.CopyFileNew;

                WikiDir.MoveFile = false;
            }

            Settings.Default.MoveFile = WikiDir.MoveFile;
        }

        void ScrapeFile_Click(object sender, EventArgs e)
        {
            if (!WikiDir.ScrapeFile)
            {
                toolStripScrape.Image = Resources.ScrapeFileOld;

                WikiDir.ScrapeFile = true;

                toolStripScrape.Text = "Scrape File [ON]";
            }
            else
            {
                toolStripScrape.Image = Resources.ScrapeFileNew;

                WikiDir.ScrapeFile = false;

                toolStripScrape.Text = "Scrape File [OFF]";
            }

            Settings.Default.ScrapeFile = WikiDir.ScrapeFile;
        }

        void CharLevel_Click(object sender, EventArgs e)
        {
            WikiDir.ScrapeLetter = Convert.ToInt32(sender.ToString());

            Settings.Default.ScrapeLevel = Convert.ToInt32(sender.ToString());

            toolStripLetterLevel.Image = WikiHelper.GetLevelImage(toolStripScrape, true, WikiDir.ScrapeLetter);
        }

        void ScrapeLetter_Click(object sender, EventArgs e)
        {
            toolStripLetterLevel.ShowDropDown();
        }

        void FreqLevel_Click(object sender, EventArgs e)
        {
            WikiDir.ScrapeFrequency = Convert.ToInt32(sender.ToString());

            Settings.Default.ScrapeFreq = Convert.ToInt32(sender.ToString());

            toolStripFreqLevel.Image = WikiHelper.GetLevelImage(toolStripScrape, false, WikiDir.ScrapeFrequency);
        }

        void ScrapeFrequency_Click(object sender, EventArgs e)
        {
            toolStripFreqLevel.ShowDropDown();
        }

        private void Manual_Click(object sender, EventArgs e)
        {
            SendToScreen(ControlManager.RichControl($@"{WikiDir.MyWikiFolder}\WikiInfo.rtf", true, false));
        }

        void Contact_Click(object sender, EventArgs e)
        {
            SendToScreen(ControlManager.RichControl($@"{WikiDir.MyWikiFolder}\ContactInfo.rtf", true, false));
        }

        void YouTube_Click(object sender, EventArgs e)
        {
            SendToScreen(ControlManager.WebControl(@"https://www.youtube.com/channel/UCuu_5wegzY9sgZiOFVl68KA"));
        }

        void Donate_Click(object sender, EventArgs e)
        {
            SendToScreen(ControlManager.WebControl(@"https://paypal.me/CalFyre?country.x=CA&locale.x=en_US"));
        }

        void ClearAllText()
        {
            toolStripSearchText.Clear();

            toolStripDirectory.Items.Clear();
        }

        void UpdateControlType(WikiControl control)
        {
            if (control.FileType == FileTypes.Rich)
            {
                RichTextBox richTextBox = control.Handle as RichTextBox;

                richTextBox.LinkClicked += RichTextBox_LinkClicked;
            }

            if (control.FileType == FileTypes.Web)
            {
                WebView2 webView = control.Handle as WebView2;

                webView.SourceChanged += WebView_SourceChanged;
            }
        }

        internal void SendToScreen(WikiControl control)
        {
            CloseScreens();

            WikiCntrl = control;

            UpdateControlType(control);

            if (!FullSize)
            {
                UpdateScreenSize(false);

                SearchScreen(control.Handle);
            }
            else
            {
                UpdateScreenSize(true);

                FullScreen(control.Handle);
            }
        }

        void CloseScreens()
        {
            CloseFullScreen();

            CloseSearchScreen();
        }

        readonly List<Control> FullScreenControl = new List<Control>();

        void FullScreen(Control control)
        {
            if (control != null && FullScreenControl.Count == 0)
            {
                FullScreenControl.Add(control);

                CloseSearchScreen();

                Controls.Add(control);

                control.BringToFront();
            }
        }

        void CloseFullScreen()
        {
            if (FullScreenControl.Count > 0)
            {
                Controls.Remove(FullScreenControl[0]);

                ClearResources(FullScreenControl);
            }
        }

        readonly List<Control> SearchScreenControl = new List<Control>();

        void SearchScreen(Control control)
        {
            if (control != null && SearchScreenControl.Count == 0)
            {
                SearchScreenControl.Add(control);

                CloseFullScreen();

                viewerPanel.Visible = true;

                viewerPanel.Controls.Add(control, 0, 0);

                control.BringToFront();
            }
        }

        void CloseSearchScreen()
        {
            if (SearchScreenControl.Count > 0)
            {
                viewerPanel.Visible = false;

                viewerPanel.Controls.Remove(SearchScreenControl[0]);

                ClearResources(SearchScreenControl);
            }
        }

        void ClearResources(List<Control> listToClean)
        {
            if (listToClean != null && listToClean.Count > 0)
            {
                listToClean.Clear();

                if (WikiCntrl != null)
                    WikiCntrl.DisposeHandle();
            }
        }

        internal void UpdateScreenSize(bool isFullSize)
        {
            if (!isFullSize)
            {
                FullSize = false;

                toolStripFullScreen.Image = Resources.FullScreenBox;
            }
            else
            {
                FullSize = true;

                toolStripFullScreen.Image = Resources.FullScreenFill;
            }
        }

        void ScreenSize_Click(object sender, EventArgs e)
        {
            if (FullSize && FullScreenControl.Count > 0)
            {
                FullSize = false;

                toolStripFullScreen.Image = Resources.FullScreenBox;

                Control control = FullScreenControl[0];

                FullScreenControl.Clear();

                viewerPanel.Visible = true;

                CloseScreens();

                SearchScreen(control);
            }
            else if (!FullSize && SearchScreenControl.Count > 0)
            {
                FullSize = true;

                toolStripFullScreen.Image = Resources.FullScreenFill;

                Control control = SearchScreenControl[0];

                SearchScreenControl.Clear();

                viewerPanel.Visible = false;

                CloseScreens();

                FullScreen(control);
            }
        }

        internal void OpenMessage(WikiMessage wikiMSG)
        {
            CloseScreens();

            WikiHelper.SetUpMessage(ref wikiMSG, ref messageLabel, ref replyTextBox, ref messageOK, ref messageCancel);

            Controls.Add(messagePanel);

            MessageVisible(true);
        }

        internal void MessageOK_Click(object sender, EventArgs e)
        {
            if (WikiMsg != null && WikiMsg.RunFinalMsg)
            {
                SendResponse(true);
            }
            else
            {
                CloseMessage();
            }
        }

        internal void MessageCancel_Click(object sender, EventArgs e)
        {
            if (messagePanel.Visible)
            {
                if (WikiMsg != null && WikiMsg.RunFinalMsg)
                {
                    SendResponse(false);
                }
                else
                {
                    CloseMessage();
                }
            }
        }

        void SendResponse(bool reply)
        {
            MessageVisible(false);

            WikiMsg.ReplyText = replyTextBox.Text;

            WikiMsg.Response = reply;

            WikiMsg.FinishMessage();
        }

        internal void CloseMessage()
        {
            MessageReset();

            UpdateToolInfo(false, "", false);
        }

        internal void MessageVisible(bool isVisible)
        {
            messagePanel.Visible = isVisible;
        }

        void MessageReset()
        {
            MessageVisible(false);

            WikiMsg = null;

            replyTextBox.Clear();

            WikiHelper.MessageIsOpen = false;

            Controls.Remove(messagePanel);
        }

        void ReplyEntry_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (WikiMsg != null)
            {
                bool keyRestriction = WikiHelper.IsValidKeyPress(e, WikiMsg.Reply == WikiMessage.ReplyType.AddMeta);

                if (keyRestriction)
                {
                    e.Handled = true;
                }
            }
        }

        void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender.Equals(toolStripSearchText))
                {
                    SearchButton_Click(sender, e);

                    toolStripSearchText.Clear();
                }

                if (sender.Equals(replyTextBox))
                {
                    MessageOK_Click(sender, e);

                    replyTextBox.Clear();
                }

                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void InfoBox_Click(object sender, EventArgs e)
        {
            if (SingleClickControl)
            {
                SingleClickControl = false;

                InfoTextBox.BackColor = Color.FromArgb(255, 64, 64, 64);

                InfoTextBox.ForeColor = Color.WhiteSmoke;
            }
            else
            {
                SingleClickControl = true;

                InfoTextBox.BackColor = Color.WhiteSmoke;

                InfoTextBox.ForeColor = Color.FromArgb(255, 64, 64, 64);
            }

            if (InfoTextBox.Text.StartsWith("Topic Directory"))
                InfoTextBox.Text = GetInfoString();
        }

        void WallPaper_Click(object sender, EventArgs e)
        {
            string reply = WikiPrompt.ShowDialog("Edit Wallpaper?", "Wiki Wallpaper", "YES", "Cancel");

            if (reply == "YES")
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = WikiDir.MyDocFolder;
                    openFileDialog.Filter = "Image Files(*.jpg) | *.jpg";
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (openFileDialog.FileName.Length > 0)
                        {
                            string wallpaper = openFileDialog.FileName;

                            if (WikiHelper.WallPaper != null)
                            {
                                BackgroundImage = null;

                                WikiHelper.WallPaper.Dispose();
                            }

                            try
                            {
                                File.Copy(wallpaper, $@"{WikiDir.AppFolder}\logo.jpg", true);

                                WikiHelper.WallPaper = Image.FromFile($@"{WikiDir.AppFolder}\logo.jpg");

                                BackgroundImage = WikiHelper.WallPaper;

                                BackgroundImageLayout = ImageLayout.Stretch;
                            }
                            catch
                            {
                                FileWorker.LogEvent("ERROR - > Failed to edit wallpaper");
                            }
                        }
                    }
                }
            }
        }

        void ColorTheme_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Color newColor = colorDialog.Color;

                    int computeColor = newColor.R + newColor.G + newColor.B;

                    if (computeColor < 30)
                    {
                        newColor = Color.FromArgb(255, 10, 10, 10);
                    }

                    if (computeColor > 690)
                    {
                        newColor = Color.FromArgb(255, 230, 230, 230);
                    }

                    UpdateColorTheme(newColor);

                    Settings.Default.ToolColor = newColor;
                }
            }
        }

        internal void UpdateColorTheme(Color color)
        {
            wikiToolStrip.BackColor = color;
            footerToolStrip.BackColor = color;

            toolStripColor.ForeColor = color;
            toolStripColor.BackColor = color;
        }

        void ToolTip_Click(object sender, EventArgs e)
        {
            if (ToolTipControl)
            {
                ToolTipControl = false;

                toolStripTips.Text = "Tool Tips [OFF]";

                toolStripTips.Image = Resources.ToolTipOld;
            }
            else
            {
                ToolTipControl = true;

                toolStripTips.Text = "Tool Tips [ON]";

                toolStripTips.Image = Resources.ToolTipNew;
            }

            ToggleToolTips();
        }

        void ToggleToolTips()
        {
            foreach (var control in wikiToolStrip.Items)
            {
                if (control is ToolStripButton button)
                {
                    if (button.AutoToolTip)
                    {
                        button.Text = ToolTipControl ? button.Tag.ToString() : string.Empty;
                    }
                }

                if (control is ToolStripDropDownButton dropDown)
                {
                    if (dropDown.AutoToolTip)
                    {
                        dropDown.Text = ToolTipControl ? dropDown.Tag.ToString() : string.Empty;
                    }
                }
            }

            foreach (var control in footerToolStrip.Items)
            {
                if (control is ToolStripButton button)
                {
                    if (button.AutoToolTip)
                    {
                        button.Text = ToolTipControl ? button.Tag.ToString() : string.Empty;
                    }
                }
            }
        }

        void AITimer_Tick(object sender, EventArgs e)
        {
            if (!IsLocked && !WikiHelper.MessageIsOpen)
            {
                UpdateToolInfo(false, WikiHelper.Tick());

                if (toolStripProgressBar.Value > 0 && toolStripProgressBar.Value < 100)
                {
                    UpdateProgress(100);
                }
                else
                {
                    UpdateProgress(0);
                }

                replyTextBox.Clear();
            }
        }

        bool IsForcedClosed { get; set; } = false;

        void WikiRunning_Delete(object sender, FileSystemEventArgs e)
        {
            IsForcedClosed = true;

            FileWorker.LogEvent($@"STOPPED -> [File Deleted]=> Forced Close()");

            Close();
        }

        void Wiki_Closing(object sender, FormClosingEventArgs e)
        {
            WikiUtility.SaveAll();

            WikiHelper.SaveSettings();

            if (wikiSystemWatcher != null)
                wikiSystemWatcher.Dispose();

            if (WikiCntrl != null)
                WikiCntrl.DisposeHandle();

            if (!IsForcedClosed)
                WikiDir.StopWiki();

            FileWorker.LogEvent($@"STOPPED -> [{Application.ProductName.ToUpper()}]=> Closing()");

            FileWorker.SaveLog();
        }
    }
}
