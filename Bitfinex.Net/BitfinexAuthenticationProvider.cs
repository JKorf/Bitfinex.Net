﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using Bitfinex.Net.Objects.Internal;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;

namespace Bitfinex.Net
{
    internal class BitfinexAuthenticationProvider: AuthenticationProvider
    {
        private readonly INonceProvider _nonceProvider;

        public long GetNonce() => _nonceProvider.GetNonce();
        public string GetApiKey() => _credentials.Key!.GetString();

        public BitfinexAuthenticationProvider(ApiCredentials credentials, INonceProvider? nonceProvider) : base(credentials)
        {
            if (credentials.CredentialType != ApiCredentialsType.Hmac)
                throw new Exception("Only Hmac authentication is supported");

            _nonceProvider = nonceProvider ?? new BitfinexNonceProvider();
        }

        public override void AuthenticateRequest(RestApiClient apiClient, Uri uri, HttpMethod method, IDictionary<string, object> uriParams, IDictionary<string, object> bodyParams, Dictionary<string, string> headers, bool auth, ArrayParametersSerialization arraySerialization, HttpMethodParameterPosition parameterPosition, RequestBodyFormat bodyFormat)
        {
            if (!auth)
                return;

            // Auth requests are always POST
            if (uri.AbsolutePath.Contains("v1"))
            {
                bodyParams.Add("request", uri.AbsolutePath);
                bodyParams.Add("nonce", _nonceProvider.GetNonce().ToString());

                var signature = JsonConvert.SerializeObject(bodyParams);
                var payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(signature));
                var signedData = Sign(payload);

                headers.Add("X-BFX-APIKEY", _credentials.Key!.GetString());
                headers.Add("X-BFX-PAYLOAD", payload);
                headers.Add("X-BFX-SIGNATURE", signedData.ToLower(CultureInfo.InvariantCulture));
            }
            else if (uri.AbsolutePath.Contains("v2"))
            {
                var json = JsonConvert.SerializeObject(bodyParams);
                var n = _nonceProvider.GetNonce().ToString();
                var signature = $"/api{uri.AbsolutePath}{n}{json}";
                var signedData = SignHMACSHA384(signature);

                headers.Add("bfx-apikey", _credentials.Key!.GetString());
                headers.Add("bfx-nonce", n);
                headers.Add("bfx-signature", signedData.ToLower(CultureInfo.InvariantCulture));
            }
        }

        public string Sign(string toSign) => SignHMACSHA384(toSign);        
    }
}
