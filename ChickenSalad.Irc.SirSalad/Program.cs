using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChickenSalad.IRC.SirSalad
{
    class Program
    {
        static readonly string Channel = "#uakroncs";
        static readonly string User = "SirSalad";
        static readonly string Server = "abstract.slashnet.org";

        static void Main()
        {
            using (var bot = new IRCBot(Server, Channel, User))
            {
                bot.Connect();

                while (true)
                {
                    var command = bot.Read();

                    Console.WriteLine(command.Raw);
                }
            }
        }
    }
}
