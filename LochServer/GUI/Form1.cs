using Loch.Core;
using Loch.Network;
using LochServer.Network;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Loch
{
    public partial class Form1 : Form
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        private ServerProcessing _server;
        private readonly ConfigImport _config;
        public Form1(ConfigImport config)
        {
            InitializeComponent();

            this.Text = "Loch Server";

            _config = config;

            //Конфигурация основного окна
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            //Конфигурация чата
            txtLog.ReadOnly = true;
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Font = new Font("Consolas", 10);
            txtLog.BorderStyle = BorderStyle.None;

            txtLog.HandleCreated += (s, e) =>
            {
                // "DarkMode_Explorer" заставляет Windows рисовать тёмный скроллбар
                SetWindowTheme(txtLog.Handle, "DarkMode_Explorer", null);
            };

            //Эвенты списка пользователей
            ClientInfo.OnClientAdded += OnClientAdded;
            ClientInfo.OnClientRemoved += OnClientRemoved;

            //Конфигурация списка пользователей
            lstUsers.BackColor = Color.FromArgb(30, 30, 30);
            lstUsers.ForeColor = Color.LightGreen;
            lstUsers.Font = new Font("Consolas", 10);
            lstUsers.View = View.Details;
            lstUsers.FullRowSelect = true;
            lstUsers.MultiSelect = false;
            lstUsers.Scrollable = true;
            lstUsers.Columns.Clear();
            lstUsers.Columns.Add("", -2);
            lstUsers.HeaderStyle = ColumnHeaderStyle.None;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _server = new ServerProcessing(_config, msg => AddLog(msg));

                _ = Task.Run(() => _server.StartAsync());

                AddLog("[Сервер работает и ожидает клиентов.]");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Ошибка при запуске: {ex.Message}\n\nДетали:\n{ex.StackTrace}]",
                                "Ошибка запуска", MessageBoxButtons.OK, MessageBoxIcon.Error);

                AddLog($"[КРИТИЧЕСКАЯ ОШИБКА: {ex.Message}]", true);
            }
        }

        private void AddLog(string message, bool isError = false)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => AddLog(message, isError)));
                return;
            }

            string time = DateTime.Now.ToString("HH:mm:ss");

            txtLog.AppendText($"[{time}] {message}\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void OnClientAdded(string clientId)
        {
            if (lstUsers.InvokeRequired)
            {
                lstUsers.Invoke(() => OnClientAdded(clientId));
                return;
            }

            lstUsers.Items.Add(clientId);

        }

        private void OnClientRemoved(string clientId)
        {

            if (lstUsers.InvokeRequired)
            {
                lstUsers.Invoke(() => OnClientRemoved(clientId));
                return;
            }

            foreach (ListViewItem item in lstUsers.Items)
            {
                if (item.Text == clientId)
                {
                    lstUsers.Items.Remove(item);
                    return;
                }
            }
        }

        private void darkTextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}