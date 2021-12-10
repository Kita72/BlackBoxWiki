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

                FileWorker.LogEvent("main", "start", $@"=> {Application.ProductName.ToUpper()}");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Wiki());
            }
            else
            {
                string result = WikiPrompt.ShowDialog("Is MyWiki Running?", "Start Up", "YES", "NO");

                if (result == "YES")
                {
                    Application.Exit();
                }
                
                if (result == "NO")
                {
                    WikiHelper.WikiDir.StopWiki();

                    Application.Restart();
                }
            }
        }
    }
}
