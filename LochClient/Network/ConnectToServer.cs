using Loch.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private ConfigImport _config;


        public  ConnectToServer(ConfigImport config, Action<string> logAction = null)
        {
            _config = config;
            _ip = config.Ip;
            _port = config.Port;
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
                SendPaswword();
                ReadingServer();
            }
            catch(Exception ex)
            {
                _logAction($"[Не удалось подключиться. Повтор через {_reconnectTimer.Interval / 1000} сек...]");
                _reconnectTimer.Start();
            }
        }

        private async void ReconnectTimer_Tick(object sender, EventArgs e)
        {
            _reconnectTimer.Stop();
            _logAction("[Попытка переподключения...]");
            await Connection();
        }

        private async void SendPaswword()
        {
            SendInput _sendInput = new SendInput(_stream, _config);
            _sendInput.SendMessage($"AUTH:{_config.ServerPassword}", _config.ServerPassword);
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

                    byte[] packet = new byte[bytesRead];
                    Buffer.BlockCopy(buffer, 0, packet, 0, bytesRead);

                    string decrypt = _config.Crypt.DecryptMessage(packet, _config.ServerPassword);
                    if (decrypt.StartsWith("/./users "))
                    {
                        string idsPart = decrypt.Substring(8).Trim();
                        var userIds = idsPart.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        UserListUpdated?.Invoke(userIds);
                    }
                    else
                    {
                        _logAction($"{decrypt}");

                    }
                }
            }
            catch (Exception ex)
            {
                _logAction?.Invoke($"[{ex}]");
            }
            finally
            {
                _reconnectTimer?.Stop();
                _reconnectTimer?.Dispose();
                _tcpClient?.Close();
                _stream?.Close();
                _logAction($"[Клиент отключен]");
            }
        }
    }
}
