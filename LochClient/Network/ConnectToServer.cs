using Loch.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LochClient.Network
{
    internal class ConnectToServer
    {
        private TcpClient _tcpClient;
        public NetworkStream _stream;
        private readonly string _ip;
        private readonly int _port;
        public event Action<string[]> UserListUpdated;
        private System.Windows.Forms.Timer _reconnectTimer;
        private readonly Action<string> _logAction;
        int reconnectIntervalMs = 3000;


        public  ConnectToServer(string ip, int port, Action<string> logAction = null)
        {
            _ip = ip;
            _port = port;
            _logAction = logAction ?? ((msg) => Console.WriteLine(msg));
            _tcpClient = new TcpClient();
            _reconnectTimer = new System.Windows.Forms.Timer();
            _reconnectTimer.Interval = reconnectIntervalMs;
            _reconnectTimer.Tick += ReconnectTimer_Tick;
            Connection();
        }
        public NetworkStream GetStream() => _stream;

        public async Task Connection()
        {
            try
            {
                _tcpClient.Connect(_ip, _port);
                _stream = _tcpClient.GetStream();
                ReadingServer();
            }
            catch(Exception ex)
            {
                _logAction($"Не удалось подключиться. Повтор через {_reconnectTimer.Interval / 1000} сек...");
                _reconnectTimer.Start();
            }
        }

        private async void ReconnectTimer_Tick(object sender, EventArgs e)
        {
            _reconnectTimer.Stop();
            _logAction("Попытка переподключения...");
            await Connection();
        }

        public async Task ReadingServer()
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (_tcpClient.Connected)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (message.StartsWith("\x1C/users "))
                    {
                        string idsPart = message.Substring(7).Trim();
                        var userIds = idsPart.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        UserListUpdated?.Invoke(userIds);
                    }
                    else
                    {
                        _logAction($"{message}");

                        string logPath = Path.Combine(Application.StartupPath, "debug.txt");
                        File.AppendAllText(logPath, $"{DateTime.Now}: {message}\n");
                    }
                }
            }
            catch (Exception ex)
            {
                _logAction?.Invoke($"{ex}");
            }
            finally
            {
                _reconnectTimer?.Stop();
                _reconnectTimer?.Dispose();
                _tcpClient?.Close();
                _stream?.Close();
                _logAction($"Клиент отключен.");
            }
        }
    }
}
