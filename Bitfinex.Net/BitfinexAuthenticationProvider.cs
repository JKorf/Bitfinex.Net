using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Bitfinex.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net
{
    internal class BitfinexAuthenticationProvider: AuthenticationProvider
    {
        private readonly INonceProvider _nonceProvider;
        private readonly HMACSHA384 encryptor;
        private readonly object locker;

        public long GetNonce() => _nonceProvider.GetNonce();

        public BitfinexAuthenticationProvider(ApiCredentials credentials, INonceProvider? nonceProvider) : base(credentials)
        {
            if (credentials.Secret == null)
                throw new ArgumentException("ApiKey/Secret needed");

            _nonceProvider = nonceProvider ?? new BitfinexNonceProvider();
            locker = new object();
            encryptor = new HMACSHA384(Encoding.UTF8.GetBytes(credentials.Secret.GetString()));
        }

        public override Dictionary<string, string> AddAuthenticationToHeaders(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition parameterPosition, ArrayParametersSerialization arraySerialization)
        {
            if(Credentials.Key == null)
                throw new ArgumentException("ApiKey/Secret needed");

            var result = new Dictionary<string, string>();
            if (!signed)
                return result;

            if (uri.Contains("v1"))
            {
                var signature = JsonConvert.SerializeObject(parameters);

                var payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(signature));
                var signedData = Sign(payload);

                result.Add("X-BFX-APIKEY", Credentials.Key.GetString());
                result.Add("X-BFX-PAYLOAD", payload);
                result.Add("X-BFX-SIGNATURE", signedData.ToLower(CultureInfo.InvariantCulture));
            }
            else if (uri.Contains("v2"))
            {
                var json = JsonConvert.SerializeObject(parameters.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value));
                var n = _nonceProvider.GetNonce().ToString();
                var signature = $"/api{uri.Split(new[] { ".com" }, StringSplitOptions.None)[1]}{n}{json}";
                var signedData = Sign(signature);

                result.Add("bfx-apikey", Credentials.Key.GetString());
                result.Add("bfx-nonce", n);
                result.Add("bfx-signature", signedData.ToLower(CultureInfo.InvariantCulture));
            }

            return result;
        }

        public override Dictionary<string, object> AddAuthenticationToParameters(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition parameterPosition, ArrayParametersSerialization arraySerialization)
        {
            if (!signed)
                return parameters;

            if (uri.Contains("v1"))
            {
                parameters.Add("request", uri.Split(new[] { ".com" }, StringSplitOptions.None)[1]);
                parameters.Add("nonce", _nonceProvider.GetNonce().ToString());
            }
            else if (uri.Contains("v2"))
            {
                // Nothing
            }

            return parameters;
        }

        public override string Sign(string toSign)
        {
            lock(locker)
                return ByteToString(encryptor.ComputeHash(Encoding.UTF8.GetBytes(toSign)));
        }
    }
}
