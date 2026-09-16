using System;
using System.Windows.Forms;
using LochServer.Core;
using LochServer.GUI;

namespace LochServer
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