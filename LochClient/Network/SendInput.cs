using System.IO;
using System.Net.Sockets;
using System.Text;

namespace LochClient.Network
{
    internal class SendInput
    {
        private readonly NetworkStream _stream;
        private string _message;
        public SendInput(NetworkStream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public async Task SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            byte[] data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length);
        }
    }

}

