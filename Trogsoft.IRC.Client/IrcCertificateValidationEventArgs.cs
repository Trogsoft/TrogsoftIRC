using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Trogsoft.IRC.Client
{
    public class IrcCertificateValidationEventArgs : IrcBooleanEventArgs
    {
        public X509Certificate Certificate { get; }
        public X509Chain Chain { get; }
        public SslPolicyErrors SslPolicyErrors { get;  }
        public IrcCertificateValidationEventArgs(X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            this.Certificate = certificate;
            this.Chain = chain;
            this.SslPolicyErrors = policyErrors;
            this.Success = policyErrors == SslPolicyErrors.None ? true : false;
        }
    }
}
