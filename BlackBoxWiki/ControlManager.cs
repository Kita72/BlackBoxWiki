using BlackBoxWikiLib;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BlackBoxWiki
{
    internal static class ControlManager
    {
        
        internal static Image ResourceImage;

        internal static Control WikiControl { get; set; }

        internal static WikiDirectory WikiDir => WikiHelper.WikiDir;

        internal static TextBox TextControl(string source)
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

            WikiControl = textControl;

            FileWorker.LogEvent("CREATE -> [Control]=> Text");

            return textControl;
        }

        internal static RichTextBox RichControl(string source, bool isRich, bool overrideload)
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
                richControl.Rtf = source;
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

            WikiControl = richControl;

            FileWorker.LogEvent("CREATE -> [Control]=> RichText");

            return richControl;
        }

        internal static PictureBox ImageControl(string source)
        {
            if (File.Exists(source))
                ResourceImage = Image.FromFile(source);

            PictureBox imageControl = new PictureBox
            {
                Name = "wikiPictureBox",
                Dock = DockStyle.Fill,
                Image = ResourceImage,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            WikiControl = imageControl;

            FileWorker.LogEvent("CREATE -> [Control]=> PictureBox(string)");

            return imageControl;
        }

        internal static PictureBox ImageControl(Image source)
        {
            ResourceImage = source;

            PictureBox imageControl = new PictureBox
            {
                Name = "wikiPictureBox",
                Dock = DockStyle.Fill,
                Image = ResourceImage,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            WikiControl = imageControl;

            FileWorker.LogEvent("CREATE -> [Control]=> PictureBox(image)");

            return imageControl;
        }

        internal static Control MediaControl(string source)
        {
            Control mediaControl = WikiHelper.WikiForm.GetMediaHandle(source);

            WikiControl = mediaControl;

            FileWorker.LogEvent("CREATE -> [Control]=> MediaPlayer");

            return mediaControl;
        }

        internal static WebView2 WebControl(string source)
        {
            try
            {
                WebView2 webControl = new WebView2
                {
                    Name = "wikiWebView",
                    Dock = DockStyle.Fill,
                    Source = new Uri(source)
                };

                WikiControl = webControl;

                FileWorker.LogEvent("CREATE -> [Control]=> WebBrowser");

                return webControl;
            }
            catch
            {
                FileWorker.LogEvent("FAILED -> [Control]=> WebBrowser");

                return null;
            }
        }
    }
}
