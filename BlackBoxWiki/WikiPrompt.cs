using System.Drawing;
using System.Windows.Forms;

namespace BlackBoxWiki
{
    internal static class WikiPrompt
    {
        public static string ShowDialog(string text, string caption, string btnA, string btnB)
        {
            int mainWidth = 515;
            int mainHeight = 175;

            int bodyTop = 30;
            int bodyWidth = 400;
            int bodyLeft = 50;

            Form prompt = new Form()
            {
                Text = caption,
                ShowInTaskbar = false,
                ControlBox = false,
                TopMost = true,
                BackColor = Color.FromArgb(64, 64, 64),
                ForeColor = Color.Gold,
                Width = mainWidth,
                Height = mainHeight,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label()
            {
                Text = text,
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                Width = bodyWidth,
                BackColor = Color.FromArgb(64, 64, 64),
                ForeColor = Color.Gold,
                Top = bodyTop,
                Left = bodyLeft
            };

            Button confirmation = new Button()
            {
                Text = btnA,
                TabIndex = 2,
                BackColor = Color.Gold,
                ForeColor = Color.FromArgb(64, 64, 64),
                FlatStyle = FlatStyle.Flat,
                Width = bodyWidth / 2,
                Height = 25,
                Top = bodyTop + 40,
                Left = bodyLeft, 
                DialogResult = DialogResult.OK
            };

            Button cancellation = new Button()
            {
                Text = btnB,
                TabIndex = 3,
                BackColor = Color.Gold,
                ForeColor = Color.FromArgb(64, 64, 64),
                FlatStyle = FlatStyle.Flat,
                Width = bodyWidth / 2,
                Height = 25,
                Top = bodyTop + 40,
                Left = bodyWidth / 2 + bodyLeft,
                DialogResult = DialogResult.Cancel
            };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancellation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancellation);

            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancellation;

            return prompt.ShowDialog() == DialogResult.OK ? btnA : btnB;
        }
    }
}
