using Bitfinex.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Clients.GeneralApi;
using Bitfinex.Net.Clients.SpotApi;

namespace Bitfinex.Net.Clients
{
    /// <inheritdoc cref="IBitfinexClient" />
    public class BitfinexClient : BaseRestClient, IBitfinexClient
    {
        #region Api clients

        /// <inheritdoc />
        public IBitfinexClientGeneralApi GeneralApi { get; }
        /// <inheritdoc />
        public IBitfinexClientSpotApi SpotApi { get; }

        #endregion

        #region constructor/destructor
        /// <summary>
        /// Create a new instance of BitfinexClient using the default options
        /// </summary>
        public BitfinexClient() : this(BitfinexClientOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of BitfinexClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BitfinexClient(BitfinexClientOptions options) : base("Bitfinex", options)
        {
            if (options == null)
                throw new ArgumentException("Cant pass null options, use empty constructor for default");

            GeneralApi = AddApiClient(new BitfinexClientGeneralApi(log, this, options));
            SpotApi = AddApiClient(new BitfinexClientSpotApi(log, this, options));
        }
        #endregion

        #region methods

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(BitfinexClientOptions options)
        {
            BitfinexClientOptions.Default = options;
        }

        #endregion

        #region private methods
        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override Error ParseErrorResponse(JToken data)
        {
            if (!(data is JArray))
            {
                if (data["error"] != null && data["code"] != null && data["error_description"] != null)
                    return new ServerError((int)data["code"]!, data["error"] + ": " + data["error_description"]);
                if (data["message"] != null)
                    return new ServerError(data["message"]!.ToString());
                else
                    return new ServerError(data.ToString());
            }

            var error = data.ToObject<BitfinexError>();
            return new ServerError(error!.ErrorCode, error.ErrorMessage);

        }

        internal Task<WebCallResult<T>> SendRequestAsync<T>(
            RestApiClient apiClient,
            Uri uri,
            HttpMethod method,
            CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null,
            bool signed = false) where T : class
                => base.SendRequestAsync<T>(apiClient, uri, method, cancellationToken, parameters, signed);
        #endregion
    }
}
