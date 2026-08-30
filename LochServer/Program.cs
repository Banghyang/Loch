using Loch.Core;
using LochClient.GUI;
using System;
using System.Windows.Forms;

namespace Loch
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var config = new ConfigImport();

            ApplicationConfiguration.Initialize();

            using (Auth loginForm = new Auth(config))
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            Application.Run(new Form1(config));
        }
    }
}