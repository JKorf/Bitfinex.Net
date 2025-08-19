using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Bitfinex.Net
{
    internal class BitfinexAuthenticationProvider : AuthenticationProvider
    {
        private readonly INonceProvider _nonceProvider;
        private static readonly IStringMessageSerializer _messageSerializer = new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(BitfinexExchange._serializerContext));

        public long GetNonce() => _nonceProvider.GetNonce();

        public BitfinexAuthenticationProvider(ApiCredentials credentials, INonceProvider? nonceProvider) : base(credentials)
        {
            if (credentials.CredentialType != ApiCredentialsType.Hmac)
                throw new Exception("Only Hmac authentication is supported");

            _nonceProvider = nonceProvider ?? new BitfinexNonceProvider();
        }

        public override void ProcessRequest(RestApiClient apiClient, RestRequestConfiguration request)
        {
            if (!request.Authenticated)
                return;

            if (request.Path.Contains("v1"))
            {
                request.BodyParameters.Add("request", request.Path);
                request.BodyParameters.Add("nonce", _nonceProvider.GetNonce().ToString());
                var requestBody = GetSerializedBody(_messageSerializer, request.BodyParameters);
                var encodedBody = Convert.ToBase64String(Encoding.ASCII.GetBytes(requestBody));

                request.Headers.Add("X-BFX-APIKEY", _credentials.Key);
                request.Headers.Add("X-BFX-PAYLOAD", encodedBody);
                request.Headers.Add("X-BFX-SIGNATURE", Sign(encodedBody).ToLowerInvariant());

                request.SetBodyContent(requestBody);
            }
            else
            {
                var requestBody = GetSerializedBody(_messageSerializer, request.BodyParameters);
                var nonce = _nonceProvider.GetNonce().ToString();
                var signature = SignHMACSHA384($"/api{request.Path}{nonce}{requestBody}");

                request.Headers.Add("bfx-apikey", _credentials.Key);
                request.Headers.Add("bfx-nonce", nonce);
                request.Headers.Add("bfx-signature", signature.ToLower(CultureInfo.InvariantCulture));

                request.SetBodyContent(requestBody);
            }
        }

        public string Sign(string toSign) => SignHMACSHA384(toSign);
    }
}
