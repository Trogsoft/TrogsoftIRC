using System;

namespace Trogsoft.IRC.Client
{
    internal class IrcConnectionFailedEventArgs : IrcErrorNotification
    {
        public IrcConnectionFailedEventArgs(Exception ex) : base(ex)
        {
        }
    }
}