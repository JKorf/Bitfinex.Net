using Bitfinex.Net.Clients.MessageHandlers;
using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.GeneralApi
{
    /// <inheritdoc cref="IBitfinexRestClientGeneralApi" />
    internal class BitfinexRestClientGeneralApi : RestApiClient, IBitfinexRestClientGeneralApi
    {
        #region fields
        internal string? AffiliateCode { get; set; }

        protected override IRestMessageHandler MessageHandler { get; } = new BitfinexRestMessageHandler(BitfinexErrors.Errors);

        /// <inheritdoc />
        public new BitfinexRestOptions ClientOptions => (BitfinexRestOptions)base.ClientOptions;
        #endregion

        #region Api clients
        /// <inheritdoc />
        public IBitfinexRestClientGeneralApiFunding Funding { get; }
        #endregion

        #region ctor

        internal BitfinexRestClientGeneralApi(ILogger logger, HttpClient? httpClient, BitfinexRestOptions options) :
            base(logger, httpClient, options.Environment.RestAddress, options, options.SpotOptions)
        {
            Funding = new BitfinexRestClientGeneralApiFunding(this);

            AffiliateCode = options.AffiliateCode;
        }

        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new BitfinexNonceProvider());

        internal Task<WebCallResult<T>> SendAsync<T>(
            RequestDefinition definition,
            ParameterCollection? parameters,
            CancellationToken cancellationToken) where T : class
                => SendToAddressAsync<T>(BaseAddress, definition, parameters, cancellationToken);

        internal Task<WebCallResult<T>> SendToAddressAsync<T>(
            string uri,
            RequestDefinition definition,
            ParameterCollection? parameters,
            CancellationToken cancellationToken) where T : class
                => base.SendAsync<T>(uri, definition, parameters, cancellationToken);

        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(BitfinexExchange._serializerContext));
        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null) => $"t{baseAsset.ToUpperInvariant()}{quoteAsset.ToUpperInvariant()}";

    }
}
