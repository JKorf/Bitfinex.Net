using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Bitfinex.Net.Objects.Internal;
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

        public long GetNonce() => _nonceProvider.GetNonce();

        public BitfinexAuthenticationProvider(ApiCredentials credentials, INonceProvider? nonceProvider) : base(credentials)
        {
            _nonceProvider = nonceProvider ?? new BitfinexNonceProvider();
        }

        public override void AuthenticateRequest(RestApiClient apiClient, Uri uri, HttpMethod method, Dictionary<string, object> providedParameters, bool auth, ArrayParametersSerialization arraySerialization, HttpMethodParameterPosition parameterPosition, out SortedDictionary<string, object> uriParameters, out SortedDictionary<string, object> bodyParameters, out Dictionary<string, string> headers)
        {
            uriParameters = parameterPosition == HttpMethodParameterPosition.InUri ? new SortedDictionary<string, object>(providedParameters) : new SortedDictionary<string, object>();
            bodyParameters = parameterPosition == HttpMethodParameterPosition.InBody ? new SortedDictionary<string, object>(providedParameters) : new SortedDictionary<string, object>();
            headers = new Dictionary<string, string>();

            if (!auth)
                return;

            // Auth requests are always POST
            if (uri.AbsolutePath.Contains("v1"))
            {
                bodyParameters.Add("request", uri.AbsolutePath);
                bodyParameters.Add("nonce", _nonceProvider.GetNonce().ToString());

                var signature = JsonConvert.SerializeObject(bodyParameters);
                var payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(signature));
                var signedData = Sign(payload);

                headers.Add("X-BFX-APIKEY", Credentials.Key!.GetString());
                headers.Add("X-BFX-PAYLOAD", payload);
                headers.Add("X-BFX-SIGNATURE", signedData.ToLower(CultureInfo.InvariantCulture));
            }
            else if (uri.AbsolutePath.Contains("v2"))
            {
                var json = JsonConvert.SerializeObject(bodyParameters);
                var n = _nonceProvider.GetNonce().ToString();
                var signature = $"/api{uri.AbsolutePath}{n}{json}";
                var signedData = SignHMACSHA384(signature);

                headers.Add("bfx-apikey", Credentials.Key!.GetString());
                headers.Add("bfx-nonce", n);
                headers.Add("bfx-signature", signedData.ToLower(CultureInfo.InvariantCulture));
            }
        }

        public override string Sign(string toSign) => SignHMACSHA384(toSign);

        
    }
}
