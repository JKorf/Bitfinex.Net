using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Interfaces.Clients.GeneralApi;
using Bitfinex.Net.ExtensionMethods;

namespace Bitfinex.Net.Clients.GeneralApi
{
    /// <inheritdoc />
    internal class BitfinexRestClientGeneralApiFunding : IBitfinexRestClientGeneralApiFunding
    {
        private const string ActiveFundingOffersEndpoint = "auth/r/funding/offers/{}";
        private const string FundingOfferHistoryEndpoint = "auth/r/funding/offers/{}/hist";
        private const string FundingLoansEndpoint = "auth/r/funding/loans/{}";
        private const string FundingLoansHistoryEndpoint = "auth/r/funding/loans/{}/hist";
        private const string FundingCreditsEndpoint = "auth/r/funding/credits/{}";
        private const string FundingCreditsHistoryEndpoint = "auth/r/funding/credits/{}/hist";
        private const string FundingTradesEndpoint = "auth/r/funding/trades/{}/hist";
        private const string FundingOfferSubmitEndpoint = "auth/w/funding/offer/submit";
        private const string FundingOfferCancelEndpoint = "auth/w/funding/offer/cancel";
        private const string FundingInfoEndpoint = "auth/r/info/funding/{}";
        private const string FundingAutoRenewEndpoint = "auth/w/funding/auto";
        private const string FundingAutoRenewStatusEndpoint = "auth/r/funding/auto/status";

        private readonly BitfinexRestClientGeneralApi _baseClient;

        internal BitfinexRestClientGeneralApiFunding(BitfinexRestClientGeneralApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingOffer>>> GetActiveFundingOffersAsync(string symbol, CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexFundingOffer>>(_baseClient.GetUrl(ActiveFundingOffersEndpoint.FillPathParameters(symbol), "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingOffer>>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexFundingOffer>>(_baseClient.GetUrl(FundingOfferHistoryEndpoint.FillPathParameters(symbol), "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexFundingOffer>>> SubmitFundingOfferAsync(FundingOrderType fundingOrderType, string symbol, decimal quantity, decimal rate, int period, int? flags = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "type", JsonConvert.SerializeObject(fundingOrderType, new FundingOrderTypeConverter(false)) },
                { "symbol", symbol },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) },
                { "rate", rate.ToString(CultureInfo.InvariantCulture) },
                { "period", period },
            };
            parameters.AddOptionalParameter("flags", flags);

            return await _baseClient.SendRequestAsync<BitfinexWriteResult<BitfinexFundingOffer>>(_baseClient.GetUrl(FundingOfferSubmitEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult>> CloseFundingAsync(int id, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "id", id }
            };

            return await _baseClient.SendRequestAsync<BitfinexWriteResult>(_baseClient.GetUrl("auth/w/funding/close", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexFundingOffer>>> CancelFundingOfferAsync(long offerId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "id", offerId }
            };

            return await _baseClient.SendRequestAsync<BitfinexWriteResult<BitfinexFundingOffer>>(_baseClient.GetUrl(FundingOfferCancelEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult>> KeepFundingAsync(FundType type, IEnumerable<long>? ids = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "type", EnumConverter.GetString(type) }
            };
            parameters.AddOptionalParameter("id", ids);

            return await _baseClient.SendRequestAsync<BitfinexWriteResult>(_baseClient.GetUrl("auth/w/funding/keep", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult>> CancelAllFundingOffersAsync(string? asset = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("currency", asset);

            return await _baseClient.SendRequestAsync<BitfinexWriteResult>(_baseClient.GetUrl("auth/w/funding/offer/cancel/all", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFunding>>> GetFundingLoansAsync(string symbol, CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexFunding>>(_baseClient.GetUrl(FundingLoansEndpoint.FillPathParameters(symbol), "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFunding>>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexFunding>>(_baseClient.GetUrl(FundingLoansHistoryEndpoint.FillPathParameters(symbol), "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingCredit>>> GetFundingCreditsAsync(string symbol, CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexFundingCredit>>(_baseClient.GetUrl(FundingCreditsEndpoint.FillPathParameters(symbol), "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingCredit>>> GetFundingCreditsHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexFundingCredit>>(_baseClient.GetUrl(FundingCreditsHistoryEndpoint.FillPathParameters(symbol), "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexFundingTrade>>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexFundingTrade>>(_baseClient.GetUrl(FundingTradesEndpoint.FillPathParameters(symbol), "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol, CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexFundingInfo>(_baseClient.GetUrl(FundingInfoEndpoint.FillPathParameters(symbol), "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexFundingAutoRenew>>> SubmitFundingAutoRenewAsync(string asset, bool status, decimal? quantity = null, decimal? rate = null, int? period = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "currency", asset },
                { "status", JsonConvert.SerializeObject(status, new BoolToIntConverter(false, true)) },
            };
            parameters.AddOptionalParameter("amount", quantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("rate", rate?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("period", period?.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestAsync<BitfinexWriteResult<BitfinexFundingAutoRenew>>(_baseClient.GetUrl(FundingAutoRenewEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexFundingAutoRenewStatus>> GetFundingAutoRenewStatusAsync(string asset, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "currency", asset },
            };

            return await _baseClient.SendRequestAsync<BitfinexFundingAutoRenewStatus>(_baseClient.GetUrl(FundingAutoRenewStatusEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }
    }
}
