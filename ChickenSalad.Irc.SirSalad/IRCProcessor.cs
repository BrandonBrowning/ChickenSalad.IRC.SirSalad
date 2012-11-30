using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChickenSalad.IRC.SirSalad
{
    public class IRCProcessor
    {
        private TcpClient TcpClient { get; set; }

        public IRCProcessor(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        public void Process()
        {
            
        }
    }
}
