using Bitfinex.Net.Clients.MessageHandlers;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
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

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc cref="IBitfinexRestClientSpotApi" />
    internal partial class BitfinexRestClientSpotApi : RestApiClient, IBitfinexRestClientSpotApi
    {
        #region fields
        /// <inheritdoc />
        public new BitfinexRestOptions ClientOptions => (BitfinexRestOptions)base.ClientOptions;

        protected override ErrorMapping ErrorMapping => BitfinexErrors.Errors;

        protected override IRestMessageHandler MessageHandler { get; } = new BitfinexRestMessageHandler(BitfinexErrors.Errors);
        #endregion

        /// <inheritdoc />
        public string ExchangeName => "Bitfinex";

        #region Api clients
        /// <inheritdoc />
        public IBitfinexRestClientSpotApiAccount Account { get; }
        /// <inheritdoc />
        public IBitfinexRestClientSpotApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public IBitfinexRestClientSpotApiTrading Trading { get; }
        #endregion

        #region ctor

        internal BitfinexRestClientSpotApi(ILogger logger, HttpClient? httpClient, BitfinexRestOptions options) :
            base(logger, httpClient, options.Environment.RestAddress, options, options.SpotOptions)
        {
            Account = new BitfinexRestClientSpotApiAccount(this);
            ExchangeData = new BitfinexRestClientSpotApiExchangeData(this);
            Trading = new BitfinexRestClientSpotApiTrading(this);
        }

        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new BitfinexNonceProvider());

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
                => BitfinexExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverTime);

        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(BitfinexExchange._serializerContext));
        /// <inheritdoc />
        protected override IStreamMessageAccessor CreateAccessor() => new SystemTextJsonStreamMessageAccessor(SerializerOptions.WithConverters(BitfinexExchange._serializerContext));

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
        public override TimeSyncInfo? GetTimeSyncInfo() => null;

        /// <inheritdoc />
        public override TimeSpan? GetTimeOffset() => null;

        public IBitfinexRestClientSpotApiShared SharedClient => this;
    }
}
