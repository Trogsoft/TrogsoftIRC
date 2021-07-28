namespace Trogsoft.IRC.Client
{
    public class IrcMessageEventArgs
    {
        public IrcMessageEventArgs(string prefix, string command, string rawParams)
        {
            Prefix = prefix;
            Command = command;
            ArgumentString = rawParams;
        }

        public string Prefix { get; set; }
        public string Command { get; set; }
        public string ArgumentString { get; }
    }
}