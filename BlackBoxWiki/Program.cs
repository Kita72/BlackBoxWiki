using System;
using System.Windows.Forms;
using BlackBoxWikiLib;

namespace BlackBoxWiki
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            WikiHelper.WikiDir = new WikiDirectory();

            if (!WikiHelper.WikiDir.CheckRunning() && WikiHelper.WikiDir.IsValidDirectorySetup())
            {
                WikiHelper.WikiDir.StartWiki();

                FileWorker.LogEvent($@"STARTED -> [{Application.ProductName.ToUpper()}]=> Main()");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Wiki());
            }
            else
            {
                string result = WikiPrompt.ShowDialog("Is the Wiki Running?", "Start Up", "YES", "NO");

                if (result == "YES")
                {
                    Application.Exit();
                }
                
                if (result == "NO")
                {
                    WikiHelper.WikiDir.StopWiki();

                    Application.Exit(); //TODO: change to restart when done testing
                }
            }
        }
    }
}
