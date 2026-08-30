using Loch.Core;
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
        private readonly ConfigImport _config;
        private readonly object _lock = new object();
        private readonly Action<string> _logAction;

        public ClientHandler(TcpClient client, string clientId, ConfigImport config, Action<string> logAction = null)
        {
            _clientId = clientId ?? throw new ArgumentNullException(nameof(client));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logAction = logAction ?? ((msg) => Console.WriteLine(msg));
            _config = config;

        }

        public bool VerifyPassword(NetworkStream stream, string message)
        {

            if (message == $"AUTH:{_config.ServerPassword}")
            {
                _logAction?.Invoke($"[Клиент {_clientId} авторизован.]");
                return true;
            }
            else
            {
                _logAction($"[Неудачная авторизация с {_clientId}]");
                _logAction?.Invoke($"[Попытка: {message}]");
                _client.Close();
                return false;
            }
        }

        public async Task StartHandlingAsync()
        {
            var clientInformation = new ClientInfo(_client, _clientId);

            ClientInfo.Add(clientInformation);

            string id = clientInformation.ClientId;
            NetworkStream stream = clientInformation.Stream;
            bool connected = clientInformation.TcpClient.Connected;
            clientInformation.VerifyStatus = false;

            _ = Task.Run(async () =>
            {
                await Task.Delay(200); // 200 мс задержки
                await SendUserListAsync();
            });

            byte[] buffer = new byte[4096];

            try
            {
                while (_client.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0) break;

                    byte[] packet = new byte[bytesRead];

                    Buffer.BlockCopy(buffer, 0, packet, 0, bytesRead);

                    string decrypted = _config.Crypt.DecryptMessage(packet, _config.ServerPassword);

                    if (clientInformation.VerifyStatus != true)
                    {
                        clientInformation.VerifyStatus = VerifyPassword(stream, decrypted);
                        continue;
                    }                   

                    _logAction?.Invoke($"{id}: {decrypted}");

                    await BroadcastMessageAsync($"{id}: {packet}", clientInformation);
                }
            }
            catch (Exception ex)
            {
                _logAction($"[Ошибка при чтении {id}: {ex.Message}]");
            }
            finally
            {
                _client.Close();
                ClientInfo.Remove(clientInformation);
                await BroadcastMessageAsync($"[Пользователь {id} отключился]", clientInformation);
                await SendUserListAsync();
            }
        }

        private async Task BroadcastMessageAsync(string message, ClientInfo sender)
        {
            var clientsCopy = ClientInfo.GetAll();

            byte[] encrypted = _config.Crypt.EncryptMessage(message, _config.ServerPassword);

            foreach (var client in clientsCopy)
            {
                if (client == sender) continue;

                try
                {
                    if (client.TcpClient.Connected && client.Stream.CanWrite)
                    {
                        await client.Stream.WriteAsync(encrypted, 0, encrypted.Length);
                        await client.Stream.FlushAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logAction($"[Ошибка отправки клиенту {client.ClientId}: {ex.Message}]");

                    lock (_lock)
                    {
                        if (!client.TcpClient.Connected) _logAction($"[GG]");
                           ClientInfo.Remove(client);
                    }
                }
            }
        }

        private async Task SendUserListAsync()
        {
            var userIds = ClientInfo.GetAllIds();
            string userListMessage = "/./users " + string.Join(",", userIds);
            byte[] encrypted = _config.Crypt.EncryptMessage(userListMessage, _config.ServerPassword);
            var clients = ClientInfo.GetAll();
            foreach (var client in clients)
            {
                try
                {
                    if (client.TcpClient.Connected && client.Stream.CanWrite)
                    {
                        await client.Stream.WriteAsync(encrypted, 0, encrypted.Length);
                        await client.Stream.FlushAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logAction($"[Ошибка отправки списка {client.ClientId}: {ex.Message}]");
                }
            }
        }

    }
}
