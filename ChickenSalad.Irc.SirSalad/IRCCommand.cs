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
        public string Prefix { get; private set; }
        public string Command { get; private set; }
        public string Params { get; private set; }
        public string Trail { get; private set; }

        public string Raw { get; private set; }

        private static readonly string CommandFormat = @"^(:(?<prefix>\S+) )?(?<command>\S+)( (?!:)(?<params>.+?))?( :(?<trail>.+))?$";
        private static readonly Regex CommandRegex;

        static IRCCommand()
        {
            CommandRegex = new Regex(CommandFormat);
        }

        public IRCCommand(string raw)
        {
            Raw = raw;

            var match = CommandRegex.Match(raw);
            Prefix = match.Groups["prefix"].Value;
            Command = match.Groups["command"].Value;
            Params = match.Groups["params"].Value;
            Trail = match.Groups["trail"].Value;
        }
    }
}
