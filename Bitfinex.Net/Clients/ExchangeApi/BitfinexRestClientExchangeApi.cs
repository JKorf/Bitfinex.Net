using Bitfinex.Net.Clients.MessageHandlers;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.RateLimiting.Interfaces;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.ExchangeApi
{
    /// <inheritdoc cref="IBitfinexRestClientExchangeApi" />
    internal partial class BitfinexRestClientExchangeApi : RestApiClient<BitfinexEnvironment, BitfinexAuthenticationProvider, BitfinexCredentials>, IBitfinexRestClientExchangeApi
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
        public IBitfinexRestClientExchangeApiAccount Account { get; }
        /// <inheritdoc />
        public IBitfinexRestClientExchangeApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public IBitfinexRestClientExchangeApiTrading Trading { get; }
        #endregion

        #region ctor

        internal BitfinexRestClientExchangeApi(ILoggerFactory? loggerFactory, HttpClient? httpClient, BitfinexRestOptions options) :
            base(loggerFactory, BitfinexExchange.ExchangeName, httpClient, options.Environment.RestAddress, options, options.ExchangeOptions)
        {
            Account = new BitfinexRestClientExchangeApiAccount(this);
            ExchangeData = new BitfinexRestClientExchangeApiExchangeData(this);
            Trading = new BitfinexRestClientExchangeApiTrading(this);
        }

        #endregion

        /// <inheritdoc />
        protected override BitfinexAuthenticationProvider CreateAuthenticationProvider(BitfinexCredentials credentials)
            => new BitfinexAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new BitfinexNonceProvider());

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
                => BitfinexExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverTime);

        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(BitfinexExchange._serializerContext));

        internal Task<HttpResult<T>> SendAsync<T>(
            RequestDefinition definition,
            Parameters? parameters,
            CancellationToken cancellationToken) where T : class
                => base.SendAsync<T>(definition, parameters, cancellationToken);

        protected override async ValueTask<bool> ShouldRetryRequestAsync<T>(IRateLimitGate? gate, HttpResult<T> callResult, int tries)
        {
            if (await base.ShouldRetryRequestAsync(gate, callResult, tries).ConfigureAwait(false))
                return true;

            if (callResult.Error?.ErrorType != ErrorType.InvalidTimestamp)
                return false;

            if (tries <= 3)
            {
                _logger.Log(LogLevel.Warning, "Received nonce error; retrying request");
                return true;
            }

            return false;
        }

        public IBitfinexRestClientExchangeApiShared SharedClient => this;
    }
}
