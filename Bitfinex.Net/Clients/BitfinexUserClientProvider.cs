using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace Bitfinex.Net.Clients
{
    /// <inheritdoc />
    public class BitfinexUserClientProvider : UserClientProvider<
        IBitfinexRestClient,
        IBitfinexSocketClient,
        BitfinexRestOptions,
        BitfinexSocketOptions,
        BitfinexCredentials,
        BitfinexEnvironment
        >, IBitfinexUserClientProvider
    {
        /// <inheritdoc />
        public override string ExchangeName => BitfinexExchange.ExchangeName;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="optionsDelegate">Options to use for created clients</param>
        public BitfinexUserClientProvider(Action<BitfinexOptions>? optionsDelegate = null)
            : this(null, null, Options.Create(ApplyOptionsDelegate(optionsDelegate).Rest), Options.Create(ApplyOptionsDelegate(optionsDelegate).Socket))
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public BitfinexUserClientProvider(
            HttpClient? httpClient,
            ILoggerFactory? loggerFactory,
            IOptions<BitfinexRestOptions> restOptions,
            IOptions<BitfinexSocketOptions> socketOptions)
            : base(httpClient, loggerFactory, restOptions, socketOptions)
        {
        }

        /// <inheritdoc />
        protected override IBitfinexRestClient ConstructRestClient(HttpClient client, ILoggerFactory? loggerFactory, IOptions<BitfinexRestOptions> options)
            => new BitfinexRestClient(client, loggerFactory, options);
        /// <inheritdoc />
        protected override IBitfinexSocketClient ConstructSocketClient(ILoggerFactory? loggerFactory, IOptions<BitfinexSocketOptions> options)
            => new BitfinexSocketClient(options, loggerFactory);
    }
}