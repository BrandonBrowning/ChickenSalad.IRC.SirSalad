using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Roslyn;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using GalaSoft.MvvmLight.Messaging;

namespace ChickenSalad.IRC.SirSalad
{
    class Program
    {
        static TcpClient TcpClient { get; set; }
        static NetworkStream NetworkStream { get; set; }
        static StreamWriter Output { get; set; }
        static StreamReader Input { get; set; }

        static void BotLoop()
        {
            Output.WriteLine("USER SirSalad 0 * :Shitty Bot");
            Output.Flush();

            Output.WriteLine("NICK SirSalad");
            Output.Flush();

            var roslyn = new RoslynExecutor();

            while (TcpClient.Connected)
            {
                string line = Input.ReadLine();

                if (line == null)
                {
                    break;
                }

                var command = new IRCCommand(line);

                if (command.Command == "PING")
                {
                    Output.WriteLine("PONG :" + command.Trail);
                    Output.Flush();
                }

                if (command.Command == "001")
                {
                    Output.WriteLine("JOIN #uakroncs");
                    Output.Flush();
                }

                string roslynString = "!roslyn> ";
                if (command.Command == "PRIVMSG" && command.Trail.StartsWith(roslynString))
                {
                    string code = command.Trail.Substring(roslynString.Length).Trim();
                    string response = "";

                    if (code.ToLower().Contains("reflection") || code.ToLower().Contains("diagnostics") || code.ToLower().Contains("thread"))
                    {
                        response = "That looks scary...";
                    }
                    else
                    {
                        response = roslyn.ExecuteCode(code);
                    }

                    if (!String.IsNullOrEmpty(response))
                    {
                        int nameBangIndex = command.Prefix.IndexOf('!');
                        Output.WriteLine("PRIVMSG #uakroncs :{0}: {1}", command.Prefix.Substring(0, nameBangIndex), response);
                        Output.Flush();
                    }
                }

                Console.WriteLine(JsonConvert.SerializeObject(command, Formatting.Indented));
            }
        }

        static void Main()
        {
            using (TcpClient = new TcpClient("abstract.slashnet.org", 6667))
            {
                using (NetworkStream = TcpClient.GetStream())
                {
                    using (Output = new StreamWriter(NetworkStream))
                    {
                        using (Input = new StreamReader(NetworkStream))
                        {
                            try
                            {
                                BotLoop();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                                Output.WriteLine("QUIT");
                            }
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
