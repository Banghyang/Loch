using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loch.Core
{
    internal class ConfigImport
    {
        private readonly string _ip;
        private readonly int _port;

        public ConfigImport()
        {
            string[] config = File.ReadAllLines("Config.txt");
            _ip = config[0];
            _port = int.Parse(config[1]);
        }
        public string Ip => _ip;
        public int Port => _port;
    }


}
