using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChickenSalad.IRC.SirSalad
{
    public class IRCBot : IDisposable
    {
        public string Channel { get; private set; }
        public string Server { get; private set; }
        public int Port { get; private set; }
        public string User { get; private set; }

        private StreamReader Input { get; set; }
        private StreamWriter Output { get; set; }

        private TcpClient TCP { get; set; }
        private Stream TCPStream { get; set; }

        public IRCBot(string server, string channel, string user, int port = 6667)
        {
            Channel = channel;
            Server = server;
            Port = port;
            User = user;
        }

        public void Connect()
        {
            TCP = new TcpClient(Server, Port);
            TCPStream = TCP.GetStream();
            Input = new StreamReader(TCPStream);
            Output = new StreamWriter(TCPStream);

            Send("USER", User, "0", "*", ":Shitty Bot");
            Send("NICK", User);
        }

        public void Disconnect()
        {
            Dispose();
        }

        public void Send(params string[] args)
        {
            string message = String.Format("{0}", String.Join(" ", args));

            Console.WriteLine("\t{0}", message);
            Output.WriteLine(message);

            Output.Flush();
        }

        public IRCCommand Read()
        {
            while (true)
            {
                string line = Input.ReadLine();

                if (line.StartsWith("PING"))
                {
                    int colonIndex = line.IndexOf(':');

                    Send("PONG", line.Substring(colonIndex + 1));

                    continue;
                }
                else
                {
                    var command = new IRCCommand(line);

                    if (command.Number == "001")
                    {
                        Send("JOIN", Channel);
                    }

                    return command;
                }
            }
        }

        public void Dispose()
        {
            Input.Dispose();
            Output.Dispose();
            TCPStream.Dispose();
            TCP.Close();
        }
    }
}
