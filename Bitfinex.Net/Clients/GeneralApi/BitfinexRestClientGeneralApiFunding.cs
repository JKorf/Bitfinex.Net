using Bitfinex.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using CryptoExchange.Net.RateLimiting.Guards;

namespace Bitfinex.Net.Clients.GeneralApi
{
    /// <inheritdoc />
    internal class BitfinexRestClientGeneralApiFunding : IBitfinexRestClientGeneralApiFunding
    {
        private static readonly RequestDefinitionCache _definitions = new();
        private readonly BitfinexRestClientGeneralApi _baseClient;

        internal BitfinexRestClientGeneralApiFunding(BitfinexRestClientGeneralApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingOffer[]>> GetActiveFundingOffersAsync(string symbol, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/funding/offers/{symbol}", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexFundingOffer[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingOffer[]>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/funding/offers/{symbol}/hist", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexFundingOffer[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResultFundingOffer>> SubmitFundingOfferAsync(FundingOrderType fundingOrderType, string symbol, decimal quantity, decimal rate, int period, int? flags = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "symbol", symbol },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) },
                { "rate", rate.ToString(CultureInfo.InvariantCulture) },
                { "period", period },
            };
            parameters.AddEnum("type", fundingOrderType);
            parameters.AddOptionalParameter("flags", flags);

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/w/funding/offer/submit", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexWriteResultFundingOffer>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult>> CloseFundingAsync(int id, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "id", id }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/w/funding/close", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexWriteResult>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResultFundingOffer>> CancelFundingOfferAsync(long offerId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                { "id", offerId }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/w/funding/offer/cancel", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexWriteResultFundingOffer>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult>> KeepFundingAsync(FundType type, IEnumerable<long>? ids = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                { "type", EnumConverter.GetString(type) }
            };
            parameters.AddOptionalParameter("id", ids);

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/w/funding/keep", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexWriteResult>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult>> CancelAllFundingOffersAsync(string? asset = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("currency", asset);

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/w/funding/offer/cancel/all", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexWriteResult>(request, parameters, ct).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFunding[]>> GetFundingLoansAsync(string symbol, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/funding/loans/{symbol}", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexFunding[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFunding[]>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/funding/loans/{symbol}/hist", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexFunding[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingCredit[]>> GetFundingCreditsAsync(string symbol, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/funding/credits/{symbol}", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexFundingCredit[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingCredit[]>> GetFundingCreditsHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/funding/credits/{symbol}/hist", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexFundingCredit[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingTrade[]>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/funding/trades/{symbol}/hist", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexFundingTrade[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/info/funding/{symbol}", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexFundingInfo>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResultFundingAutoRenew>> SubmitFundingAutoRenewAsync(string asset, bool status, decimal? quantity = null, decimal? rate = null, int? period = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "currency", asset },
                { "status", status ? 1 : 0 },
            };
            parameters.AddOptionalParameter("amount", quantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("rate", rate?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("period", period?.ToString(CultureInfo.InvariantCulture));

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/w/funding/auto", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexWriteResultFundingAutoRenew>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingAutoRenewStatus>> GetFundingAutoRenewStatusAsync(string asset, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "currency", asset },
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/funding/auto/status", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexFundingAutoRenewStatus>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
