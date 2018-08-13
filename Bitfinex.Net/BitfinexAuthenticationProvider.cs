using System;
using System.Security.Cryptography;
using System.Text;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;

namespace Bitfinex.Net
{
    public class BitfinexAuthenticationProvider: AuthenticationProvider
    {
        private readonly HMACSHA384 encryptor;
        private readonly object locker;

        public BitfinexAuthenticationProvider(ApiCredentials credentials) : base(credentials)
        {
            locker = new object();
            encryptor = new HMACSHA384(Encoding.UTF8.GetBytes(credentials.Secret.GetString()));
        }

        public override string AddAuthenticationToUriString(string uri, bool signed)
        {
            throw new NotImplementedException();
        }

        public override IRequest AddAuthenticationToRequest(IRequest request, bool signed)
        {
            throw new NotImplementedException();
        }

        public override string Sign(string toSign)
        {
            lock(locker)
                return ByteToString(encryptor.ComputeHash(Encoding.UTF8.GetBytes(toSign)));
        }
    }
}
