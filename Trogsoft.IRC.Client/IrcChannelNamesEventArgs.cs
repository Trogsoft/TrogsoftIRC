using System.Collections.Generic;

namespace Trogsoft.IRC.Client
{
    public class IrcChannelNamesEventArgs
    {
        public string Channel { get; set; }
        public List<string> Names { get; } = new List<string>();
    }
}