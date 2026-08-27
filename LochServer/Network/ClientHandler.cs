using LochServer.Network;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Loch.Network
{
    internal class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly string _clientId;
        private readonly object _lock = new object();
        private readonly Action<string> _logAction;

        public ClientHandler(TcpClient client, string clientId, Action<string> logAction)
        {
            _clientId = clientId ?? throw new ArgumentNullException(nameof(client));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
        }

        public async Task StartHandlingAsync()
        {
            
            var clientInformation = new ClientInfo(_client, _clientId);

            ClientInfo.Add(clientInformation);

            string id = clientInformation.ClientId;
            NetworkStream stream = clientInformation.Stream;
            bool connected = clientInformation.TcpClient.Connected;

            _ = Task.Run(async () =>
            {
                await Task.Delay(200); // 200 мс задержки
                await SendUserListAsync();
            });

            byte[] buffer = new byte[1024];

            try
            {
                while (_client.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    _logAction?.Invoke($"{id}: {message}");

                    byte[] data = Encoding.UTF8.GetBytes($"{message}");
                    await BroadcastMessageAsync($"{id}: {message}", clientInformation);
                }
            }
            catch (Exception ex)
            {
                _logAction($"Ошибка при чтении {id}: {ex.Message}");
            }
            finally
            {
                _client.Close();
                ClientInfo.Remove(clientInformation);
                await BroadcastMessageAsync($"Пользователь {id} отключился", clientInformation);
                await SendUserListAsync();
            }
        }

        private async Task BroadcastMessageAsync(string message, ClientInfo sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            var clientsCopy = ClientInfo.GetAll();

            foreach (var client in clientsCopy)
            {
                if (client == sender) continue;

                try
                {
                    if (client.TcpClient.Connected && client.Stream.CanWrite)
                    {
                        await client.Stream.WriteAsync(data, 0, data.Length);
                        await client.Stream.FlushAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logAction($"Ошибка отправки клиенту {client.ClientId}: {ex.Message}");

                    lock (_lock)
                    {
                        if (!client.TcpClient.Connected) _logAction($"GG");
                           ClientInfo.Remove(client);
                    }
                }
            }
        }

        private async Task SendUserListAsync()
        {
            var userIds = ClientInfo.GetAllIds();
            string userListMessage = "\x1C/users " + string.Join(",", userIds);
            byte[] data = Encoding.UTF8.GetBytes(userListMessage);

            var clients = ClientInfo.GetAll();
            foreach (var client in clients)
            {
                try
                {
                    if (client.TcpClient.Connected && client.Stream.CanWrite)
                    {
                        await client.Stream.WriteAsync(data, 0, data.Length);
                        await client.Stream.FlushAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logAction($"Ошибка отправки списка {client.ClientId}: {ex.Message}");
                }
            }
        }

    }
}
