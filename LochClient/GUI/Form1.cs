using Loch.Core;
using LochClient.Network;
using MarkdownToRtf;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Loch
{
    public partial class Form1 : Form
    {
        private ConnectToServer _connection;
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isConnected;
        private readonly Action<string> _logAction;
        private readonly ConfigImport _config;
        public event Action<string> MessageReceived;

        public Form1(ConfigImport config)
        {
            InitializeComponent();

            _config = config;
            //Конфигурация основного окна
            this.BackColor = Color.FromArgb(30, 30, 30);

            //Конфигурация окна чата
            txtLog.ReadOnly = true;
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ForeColor = Color.LightGreen;
            txtLog.Font = new Font("Consolas", 10);
            txtLog.BorderStyle = BorderStyle.None;

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

            //Конфигурация поля ввода
            EntryBox.BackColor = Color.FromArgb(30, 30, 30);
            EntryBox.ForeColor = Color.LightGreen;
            EntryBox.Font = new Font("Consolas", 10);

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {

                _connection = new ConnectToServer(_config, msg => AddLog(msg));


                txtLog.Clear();

                _connection.MessageReceived += OnMessageReceived;
                _connection.UserListUpdated += OnUserListUpdated;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении: {ex.Message}\n\nДетали:\n{ex.StackTrace}",
                                "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);

                AddLog($"КРИТИЧЕСКАЯ ОШИБКА: {ex.Message}", true);
            }
        }

        private void EntryBox_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.KeyCode == Keys.Enter)
            {

                string message = EntryBox.Text.Trim();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    SendInput messageSender = new SendInput(_connection._stream, _config);
                    messageSender.SendMessage(message, _config.ServerPassword);
                    DisplayMessage($"You: {message}");
                }

                EntryBox.Clear();

                e.SuppressKeyPress = true;
            }
        }

        private void DisplayMessage(string message)
        {
            AppendFormattedMessage(message);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Пусто
        }

        private void OnUserListUpdated(string[] userIds)
        {
            UpdateUserList(userIds);
        }

        private void UpdateUserList(string[] userIds)
        {
            if (lstUsers.InvokeRequired)
            {
                lstUsers.Invoke(() => UpdateUserList(userIds));
                return;
            }

            lstUsers.Items.Clear();
            foreach (var id in userIds)
            {
                lstUsers.Items.Add(id);
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

        private void OnMessageReceived(string message)
        {
            AppendFormattedMessage(message);
        }

        private void AppendFormattedMessage(string message)
            {
                try
                {
                    string rtf = MarkdownToRtfConverter.Convert(message);

                    if (string.IsNullOrWhiteSpace(rtf))
                    {
                        txtLog.AppendText(message + Environment.NewLine);
                        return;
                    }

                    if (txtLog.InvokeRequired)
                    {
                        txtLog.Invoke(() => InsertRtf(rtf));
                    }
                    else
                    {
                        InsertRtf(rtf);
                    }
                }
                catch (Exception ex)
                {
                    txtLog.AppendText(message + Environment.NewLine);
                }
            }

            private void InsertRtf(string rtf)
            {
                txtLog.SelectionStart = txtLog.TextLength;
                txtLog.SelectionLength = 0;

                txtLog.SelectedRtf = rtf;

                txtLog.ScrollToCaret();
            }

    private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}