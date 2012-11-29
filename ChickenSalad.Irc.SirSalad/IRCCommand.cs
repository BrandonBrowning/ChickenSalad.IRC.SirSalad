using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChickenSalad.IRC.SirSalad
{
    public class IRCCommand
    {
        public string Body { get; private set; }
        public string Number { get; private set; }
        public string Raw { get; private set; }
        public string Source { get; private set; }

        private static readonly string CommandFormat = @"^:(?<source>[^ ]+)(?<number> \d+)(?<body> .*)$";
        private static readonly Regex CommandRegex;

        static IRCCommand()
        {
            CommandRegex = new Regex(CommandFormat);
        }

        public IRCCommand(string raw)
        {
            Raw = raw;

            var match = CommandRegex.Match(raw);
            Source = match.Groups["source"].Value.Trim();
            Body = match.Groups["body"].Value.Trim();
            Number = match.Groups["number"].Value.Trim();
        }
    }
}
