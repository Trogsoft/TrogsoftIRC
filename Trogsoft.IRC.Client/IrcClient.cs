using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Trogsoft.IRC.Client
{
    public class IrcClient
    {

        private readonly IrcNetworkRegistration ircNetworkRegistration;
        private TcpClient socket;
        private Stream connectionStream;
        private Thread thread;

        bool useSsl = false;

        public IrcClient(IrcNetworkRegistration ircNetworkRegistration)
        {
            thread = new Thread(reader);
            socket = new TcpClient();
            this.ircNetworkRegistration = ircNetworkRegistration;
        }

        public void Join(string channel)
        {
            sendRaw("JOIN " + channel);
        }

        public event Action<IrcNotificiationEventArgs> OnConnected;
        public event Action<IrcErrorNotification> OnConnectionFailed;
        public event Action<IrcNotificiationEventArgs> OnRegistered;
        public event Action<IrcCertificateValidationEventArgs> OnValidateCertificate;
        public event Action<IrcMessageEventArgs> OnBounce;
        public event Action<IrcChannelNamesEventArgs> OnNames;
        public event Action<IrcPrivateMessageEventArgs> OnPrivateMessage;
        public event Action<IrcStringNotificationEventArgs> OnRawMessage;
        public event Action<IrcStringNotificationEventArgs> OnSendMessage;

        public bool Connect(string host, int port = 6667, bool useSsl = false)
        {

            this.useSsl = useSsl;

            try
            {
                socket.Connect(host, port);

                if (useSsl)
                {
                    connectionStream = new SslStream(socket.GetStream(), true, certificateVerifier);
                    ((SslStream)connectionStream).AuthenticateAsClient("irc.trogsoft.net");
                }
                else
                    connectionStream = socket.GetStream();

                thread.Start();
                register();

                return true;

            }
            catch (Exception ex)
            {
                OnConnectionFailed?.Invoke(new IrcConnectionFailedEventArgs(ex));
                return false;
            }
        }

        public void SendPrivateMessage(string destination, string message)
        {
            sendRaw($"PRIVMSG {destination} :{message}");
        }

        private bool certificateVerifier(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (OnValidateCertificate != null)
            {
                var ea = new IrcCertificateValidationEventArgs(certificate, chain, sslPolicyErrors);
                OnValidateCertificate.Invoke(ea);
                return ea.Success;
            }
            return sslPolicyErrors == SslPolicyErrors.None ? true : false;
        }

        private void reader()
        {
            string currentText = "";
            IrcChannelNamesEventArgs names = new IrcChannelNamesEventArgs();

            while (true)
            {
                var count = connectionStream.ReadByte();

                if (count == 13)
                {

                    string prefix = "";
                    string command = "";
                    string rawParams = "";
                    currentText = currentText.Trim();

                    var parts = currentText.Split(' ');
                    var ci = 0;

                    if (currentText.StartsWith(":"))
                    {
                        prefix = parts[ci];
                        ci++;
                    }

                    command = parts[ci];

                    ci++;
                    rawParams = string.Join(" ", parts.Skip(ci)).TrimStart(':');

                    if (command.Equals("PING", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sendRaw("PONG :" + rawParams);
                    }

                    if (command.Equals("001"))
                        OnRegistered?.Invoke(new IrcNotificiationEventArgs());

                    if (command.Equals("005"))
                        OnBounce?.Invoke(new IrcMessageEventArgs(prefix, command, rawParams));

                    if (command.Equals("353"))
                    {
                        var nick = parts[ci + 1];
                        var ctype = parts[ci + 2];
                        var chan = parts[ci + 3];
                        names.Channel = chan;

                        var allNames = getLongParameter(rawParams).Split(' ');
                        names.Names.AddRange(allNames);
                    }

                    if (command.Equals("366"))
                    {
                        OnNames?.Invoke(names);
                        names = new IrcChannelNamesEventArgs();
                    }

                    if (command.Equals("PRIVMSG"))
                    {
                        OnPrivateMessage.Invoke(new IrcPrivateMessageEventArgs
                        {
                            Source = new IrcSource(prefix),
                            Destination = parts[ci],
                            Message = getLongParameter(rawParams)
                        });
                    }

                    currentText = "";
                }

                if (count > -1)
                    currentText += ((char)count).ToString();
            }
        }

        private string getLongParameter(string rawParams)
        {
            if (rawParams.Contains(':'))
                return rawParams.Split(':').Last().Trim(':');

            return rawParams;
        }

        private void register()
        {
            sendRaw($"NICK {ircNetworkRegistration.Nick}");
            sendRaw($"USER {ircNetworkRegistration.User} 0 * :{ircNetworkRegistration.RealName}");
        }

        private void sendRaw(string rawString)
        {
            var toSend = Encoding.UTF8.GetBytes(rawString + "\r\n");
            connectionStream.Write(toSend, 0, toSend.Length);
        }
    }
}
