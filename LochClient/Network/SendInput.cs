using Loch.Core;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace LochClient.Network
{
    internal class SendInput
    {
        private readonly NetworkStream _stream;
        private string _message;
        private ConfigImport _config;
        public SendInput(NetworkStream stream, ConfigImport config)
        {
            _config = config;
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public async Task SendMessage(string message, string password)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            byte[] encrypted = _config.Crypt.EncryptMessage(message, password);
            await _stream.WriteAsync(encrypted, 0, encrypted.Length);
        }
    }

}

