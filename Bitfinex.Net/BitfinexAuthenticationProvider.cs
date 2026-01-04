using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Bitfinex.Net
{
    internal class BitfinexAuthenticationProvider : AuthenticationProvider
    {
        private readonly INonceProvider _nonceProvider;
        private static readonly IStringMessageSerializer _messageSerializer = new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(BitfinexExchange._serializerContext));

        public override ApiCredentialsType[] SupportedCredentialTypes => [ApiCredentialsType.Hmac];
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

            request.BodyParameters ??= new Dictionary<string, object>();
            request.Headers ??= new Dictionary<string, string>();

            if (request.Path.Contains("v1"))
            {
                request.BodyParameters.Add("request", request.Path);
                request.BodyParameters.Add("nonce", _nonceProvider.GetNonce().ToString());
                var requestBody = GetSerializedBody(_messageSerializer, request.BodyParameters);
                var encodedBody = Convert.ToBase64String(Encoding.ASCII.GetBytes(requestBody));

                request.Headers.Add("X-BFX-APIKEY", _credentials.Key);
                request.Headers.Add("X-BFX-PAYLOAD", encodedBody);
                request.Headers.Add("X-BFX-SIGNATURE", SignHMACSHA384(encodedBody).ToLowerInvariant());

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

        public override Query? GetAuthenticationQuery(SocketApiClient apiClient, SocketConnection connection)
        {
            var n = GetNonce().ToString();
            var authentication = new BitfinexAuthentication
            {
                Event = "auth",
                ApiKey = ApiKey,
                Nonce = n,
                Payload = "AUTH" + n
            };


            authentication.Signature = SignHMACSHA384(authentication.Payload).ToLower(CultureInfo.InvariantCulture);
            return new BitfinexAuthQuery(apiClient, authentication);
        }
    }
}
