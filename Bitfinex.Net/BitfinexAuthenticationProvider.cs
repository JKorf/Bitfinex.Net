using System;
using System.Security.Cryptography;
using System.Text;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using Newtonsoft.Json;

namespace Bitfinex.Net
{
    public class BitfinexAuthenticationProvider: AuthenticationProvider
    {
        private readonly HMACSHA384 encryptor;

        public BitfinexAuthenticationProvider(ApiCredentials credentials) : base(credentials)
        {
            encryptor = new HMACSHA384(Encoding.UTF8.GetBytes(credentials.Secret));
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
            return ByteToString(encryptor.ComputeHash(Encoding.UTF8.GetBytes(toSign)));
        }
    }
}
