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

namespace ChickenSalad.IRC.SirSalad
{
    class Program
    {
        static void Main()
        {
            using (var tcpClient = new TcpClient("abstract.slashnet.org", 6667))
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    using (var output = new StreamWriter(networkStream))
                    {
                        using (var input = new StreamReader(networkStream))
                        {
                            output.WriteLine("USER SirSalad 0 * :Shitty Bot");
                            output.Flush();

                            output.WriteLine("NICK SirSalad");
                            output.Flush();

                            var engine = new ScriptEngine();

                            while (tcpClient.Connected)
                            {
                                string line = input.ReadLine();

                                if (line == null)
                                {
                                    break;
                                }

                                var command = new IRCCommand(line);

                                if (command.Command == "PING")
                                {
                                    output.WriteLine("PONG :" + command.Trail);
                                    output.Flush();
                                }

                                if (command.Command == "001")
                                {
                                    output.WriteLine("JOIN #uakroncs");
                                    output.Flush();
                                }

                                string roslynString = "!roslyn> ";
                                if (command.Command == "PRIVMSG" && command.Trail.StartsWith(roslynString))
                                {
                                    string code = command.Trail.Substring(roslynString.Length).Trim();

                                    new[]
                                    {
                                        typeof (Type).Assembly,
                                        typeof (ICollection).Assembly,
                                        typeof (ListDictionary).Assembly,
                                        typeof (Console).Assembly,
                                        typeof (ScriptEngine).Assembly,
                                        typeof (IEnumerable<>).Assembly,
                                        typeof (IQueryable).Assembly
                                    }.ToList().ForEach(asm => engine.AddReference(asm));

                                    var session = engine.CreateSession();

                                    new string[]
                                    {
                                        "System",
                                        "System.Collections",
                                        "System.Collections.Generic",
                                        "System.Linq",
                                    }.ToList().ForEach(str => session.ImportNamespace(str));

                                    try
                                    {
                                        var result = session.Execute(code);

                                        if (result != null)
                                        {
output.WriteLine("PRIVMSG #uakroncs :{0}", result.ToString());
                                            output.Flush();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        output.WriteLine("PRIVMSG #uakroncs :{0}", e.Message);
                                        output.Flush();
                                    }
                                }

                                Console.WriteLine(JsonConvert.SerializeObject(command, Formatting.Indented));
                            }
                        }
                    }
                }
            }
        }
    }
}
