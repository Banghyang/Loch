using Loch.Core;
using Loch.Network;
using LochServer.Network;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Loch
{
    public partial class Form1 : Form
    {
        private ServerProcessing _server;

        public Form1()
        {
            InitializeComponent();

            //Конфигурация основного окна
            this.BackColor = Color.FromArgb(30, 30, 30);

            //Конфигурация чата
            txtLog.ReadOnly = true;
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Font = new Font("Consolas", 10);
            txtLog.BorderStyle = BorderStyle.None;

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
            lstUsers.Columns.Add("",-2);
            lstUsers.HeaderStyle = ColumnHeaderStyle.None;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {

                var config = new ConfigImport();
 
                _server = new ServerProcessing(config.Ip, config.Port, msg => AddLog(msg));

                _ = Task.Run(() => _server.StartAsync());

                AddLog("Сервер работает и ждет клиентов.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске: {ex.Message}\n\nДетали:\n{ex.StackTrace}",
                                "Ошибка запуска", MessageBoxButtons.OK, MessageBoxIcon.Error);

                AddLog($"КРИТИЧЕСКАЯ ОШИБКА: {ex.Message}", true);
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
            AddLog($"Пользователь {clientId} подключился");

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
                    AddLog($"Пользователь {clientId} отключился");
                    return;
                }
            }
        }
    }
}