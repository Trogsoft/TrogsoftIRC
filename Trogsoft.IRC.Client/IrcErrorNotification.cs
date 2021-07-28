using System;

namespace Trogsoft.IRC.Client
{
    public class IrcErrorNotification
    {
        public Exception Error { get; private set; }

        public IrcErrorNotification(Exception ex)
        {
            this.Error = ex;
        }
    }
}