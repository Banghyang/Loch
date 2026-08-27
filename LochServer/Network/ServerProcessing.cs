using Loch.Core;
using LochServer.Network;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Network
{
    public class ServerProcessing
    {
        private readonly string _ip;
        private readonly int _port;
        private TcpListener _server;
        private bool _isRunning;
        private readonly Action<string> _logAction;

        public  ServerProcessing(string ip, int port, Action<string> logAction = null)
        {
            _ip = ip;
            _port = port;
            _logAction = logAction ?? ((msg) => Console.WriteLine(msg));

        }
        public async Task StartAsync()
        {
            _isRunning = true;
            _server = new TcpListener(IPAddress.Parse(_ip), _port);
            _server.Start();

            while (_isRunning)
            {
                try
                {

                    TcpClient _client = await _server.AcceptTcpClientAsync();
                    string _clientId = _client.Client.RemoteEndPoint?.ToString() ?? "Unknown";

                    ClientHandler handler = new ClientHandler(_client, _clientId, _logAction);
                    _ = handler.StartHandlingAsync();
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logAction($"{ex}");
                }
            }
        }

         public void Stop()
         {
            _isRunning = false;
            _server?.Stop();
         }
        
    }
}
