using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using Bitfinex.Net.Objects.Internal;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc cref="IBitfinexRestClientSpotApi" />
    internal partial class BitfinexRestClientSpotApi : RestApiClient, IBitfinexRestClientSpotApi
    {
        #region fields
        internal string? AffiliateCode { get; set; }

        /// <inheritdoc />
        public new BitfinexRestOptions ClientOptions => (BitfinexRestOptions)base.ClientOptions;

        protected override ErrorMapping ErrorMapping => BitfinexErrors.Errors;
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

            AffiliateCode = options.AffiliateCode;
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

        /// <inheritdoc />
        protected override Error ParseErrorResponse(int httpStatusCode, KeyValuePair<string, string[]>[] responseHeaders, IMessageAccessor accessor, Exception? exception)
        {
            if (!accessor.IsValid)
                return new ServerError(ErrorInfo.Unknown, exception);

            if (accessor.GetNodeType() != NodeType.Array)
            {
                var error = accessor.GetValue<string?>(MessagePath.Get().Property("error"));
                var errorCode = accessor.GetValue<int?>(MessagePath.Get().Property("code"));
                var errorDesc = accessor.GetValue<string?>(MessagePath.Get().Property("error_description"));
                if (error != null && errorCode != null && errorDesc != null)
                    return new ServerError(errorCode.Value.ToString(), GetErrorInfo(errorCode.Value, $"{error}: {errorDesc}"));
                
                var message = accessor.GetValue<string?>(MessagePath.Get().Property("message"));
                if (message != null)
                    return new ServerError(ErrorInfo.Unknown with { Message = message }, exception);

                return new ServerError(ErrorInfo.Unknown, exception: exception);
            }

            var code = accessor.GetValue<int?>(MessagePath.Get().Index(1));
            var msg = accessor.GetValue<string>(MessagePath.Get().Index(2));
            if (msg == null)
                return new ServerError(ErrorInfo.Unknown, exception: exception);

            if (code == null)
                return new ServerError(ErrorInfo.Unknown with { Message = msg }, exception);

            return new ServerError(code.Value.ToString(), GetErrorInfo(code.Value, msg), exception);
        }
    }
}
