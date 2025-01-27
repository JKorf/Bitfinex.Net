using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net
{
    internal class BitfinexAuthenticationProvider: AuthenticationProvider
    {
        private readonly INonceProvider _nonceProvider;
        private static readonly IMessageSerializer _messageSerializer = new SystemTextJsonMessageSerializer();

        public long GetNonce() => _nonceProvider.GetNonce();

        public BitfinexAuthenticationProvider(ApiCredentials credentials, INonceProvider? nonceProvider) : base(credentials)
        {
            if (credentials.CredentialType != ApiCredentialsType.Hmac)
                throw new Exception("Only Hmac authentication is supported");

            _nonceProvider = nonceProvider ?? new BitfinexNonceProvider();
        }

        public override void AuthenticateRequest(
            RestApiClient apiClient,
            Uri uri,
            HttpMethod method,
            ref IDictionary<string, object>? uriParameters,
            ref IDictionary<string, object>? bodyParameters,
            ref Dictionary<string, string>? headers,
            bool auth,
            ArrayParametersSerialization arraySerialization,
            HttpMethodParameterPosition parameterPosition,
            RequestBodyFormat requestBodyFormat)
        {
            if (!auth)
                return;

            headers ??= new Dictionary<string, string>();

            // Auth requests are always POST
            if (uri.AbsolutePath.Contains("v1"))
            {
                bodyParameters ??= new Dictionary<string, object>();
                bodyParameters.Add("request", uri.AbsolutePath);
                bodyParameters.Add("nonce", _nonceProvider.GetNonce().ToString());

                var signature = _messageSerializer.Serialize(bodyParameters);
                var payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(signature));
                var signedData = Sign(payload);

                headers.Add("X-BFX-APIKEY", _credentials.Key);
                headers.Add("X-BFX-PAYLOAD", payload);
                headers.Add("X-BFX-SIGNATURE", signedData.ToLower(CultureInfo.InvariantCulture));
            }
            else if (uri.AbsolutePath.Contains("v2"))
            {
                var json = bodyParameters == null ? "{}" : _messageSerializer.Serialize(bodyParameters);
                var n = _nonceProvider.GetNonce().ToString();
                var signature = $"/api{uri.AbsolutePath}{n}{json}";
                var signedData = SignHMACSHA384(signature);

                headers.Add("bfx-apikey", _credentials.Key);
                headers.Add("bfx-nonce", n);
                headers.Add("bfx-signature", signedData.ToLower(CultureInfo.InvariantCulture));
            }
        }

        public string Sign(string toSign) => SignHMACSHA384(toSign);        
    }
}
