using System.Linq;

namespace Trogsoft.IRC.Client
{
    public class IrcSource
    {
        public IrcSourceType Type { get; set; }
        public string Host { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public override string ToString()
        {
            return Name;
        }

        public IrcSource(string input)
        {
            input = input.Trim(':');
            if (input.Contains("!") && input.Contains("@"))
            {
                Type = IrcSourceType.User;
                var ebits = input.Split('!');
                var abits = input.Split('@');
                Name = ebits.First();
                Host = ebits.Last().Split('@').First();
                Address = abits.Last();
            }
            else
            {
                Name = input;
                Host = input;
                Address = input;
            }
        }

    }
}