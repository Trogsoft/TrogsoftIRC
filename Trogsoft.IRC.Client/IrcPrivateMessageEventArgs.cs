namespace Trogsoft.IRC.Client
{
    public class IrcPrivateMessageEventArgs
    {
        public IrcSource Source { get; set; }
        public string Destination { get; set; }
        public string Message { get; set; }
    }
}