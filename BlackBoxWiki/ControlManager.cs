using BlackBoxWikiLib;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BlackBoxWiki
{
    internal class WikiControl
    {
        internal Control Handle { get; set; }

        internal FileTypes FileType { get; set; }

        internal Image ImageRes { get; set; }

        internal WikiControl()
        {
            //Add to prevsearch?
        }

        internal void DisposeHandle()
        {
            if (ImageRes != null)
            {
                ImageRes.Dispose();
            }

            if (FileType != FileTypes.Video)
            {
                Handle.Dispose();
            }
            else
            {
                WikiHelper.WikiForm.CloseMediaPlayer();
            }
        }
    }

    internal static class ControlManager
    {
        internal static WikiDirectory WikiDir => WikiHelper.WikiDir;

        static WikiControl wikiControl;

        internal static WikiControl TextControl(string source)
        {
            TextBox textControl = new TextBox
            {
                Name = "wikiTextBox",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Multiline = true,
                WordWrap = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.White,
                Text = WikiDir.ReadText(source)
            };

            wikiControl = new WikiControl
            {
                FileType = FileTypes.Text,
                Handle = textControl
            };

            FileWorker.LogEvent("CREATE -> [Control]=> Text");

            return wikiControl;
        }

        internal static WikiControl RichControl(string source, bool isRich, bool overrideload)
        {
            RichTextBox richControl = new RichTextBox
            {
                Name = "wikiRichTextBox",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Multiline = true,
                DetectUrls = true,
                WordWrap = true,
                ScrollBars = RichTextBoxScrollBars.Both,
                BackColor = Color.White
            };

            if (overrideload)
            {
                richControl.Text = source;
            }
            else
            {
                try
                {
                    if (isRich)
                        richControl.LoadFile(source);
                    else
                        richControl.LoadFile(source, RichTextBoxStreamType.PlainText);
                }
                catch
                {
                    FileWorker.LogEvent("FAILED -> [Control]=> RichText");
                }
            }

            wikiControl = new WikiControl
            {
                FileType = FileTypes.Rich,
                Handle = richControl
            };

            FileWorker.LogEvent("CREATE -> [Control]=> RichText");

            return wikiControl;
        }

        internal static WikiControl ImageControl(string source)
        {
            PictureBox imageControl = new PictureBox
            {
                Name = "wikiPictureBox",
                Dock = DockStyle.Fill,
                Image = File.Exists(source) ? Image.FromFile(source) : null,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            wikiControl = new WikiControl
            {
                FileType = FileTypes.Image,
                Handle = imageControl,
                ImageRes = File.Exists(source) ? imageControl.Image : null
            };

            FileWorker.LogEvent("CREATE -> [Control]=> PictureBox(string)");

            return wikiControl;
        }

        internal static WikiControl ImageControl(Image source)
        {
            PictureBox imageControl = new PictureBox
            {
                Name = "wikiPictureBox",
                Dock = DockStyle.Fill,
                Image = source,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            wikiControl = new WikiControl
            {
                FileType = FileTypes.Image,
                Handle = imageControl,
                ImageRes = source
            };

            FileWorker.LogEvent("CREATE -> [Control]=> PictureBox(image)");

            return wikiControl;
        }

        internal static WikiControl MediaControl(string source)
        {
            Control mediaControl = WikiHelper.WikiForm.GetMediaHandle(source);

            wikiControl = new WikiControl
            {
                FileType = FileTypes.Video,
                Handle = mediaControl
            };

            FileWorker.LogEvent("CREATE -> [Control]=> MediaPlayer");

            return wikiControl;
        }

        internal static WikiControl WebControl(string source)
        {
            try
            {
                WebView2 webControl = new WebView2
                {
                    Name = "wikiWebView",
                    Dock = DockStyle.Fill,
                    Source = new Uri(source)
                };

                wikiControl = new WikiControl
                {
                    FileType = FileTypes.Web,
                    Handle = webControl
                };

                FileWorker.LogEvent("CREATE -> [Control]=> WebBrowser");

                return wikiControl;
            }
            catch
            {
                FileWorker.LogEvent("FAILED -> [Control]=> WebBrowser");

                return null;
            }
        }
    }
}
