using Bitfinex.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;
using Bitfinex.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.RateLimiting.Guards;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc />
    internal class BitfinexRestClientSpotApiAccount : IBitfinexRestClientSpotApiAccount
    {
        private static readonly RequestDefinitionCache _definitions = new();
        private readonly BitfinexRestClientSpotApi _baseClient;

        internal BitfinexRestClientSpotApiAccount(BitfinexRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }


        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexWallet>>> GetBalancesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/auth/r/wallets", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<IEnumerable<BitfinexWallet>>(request, null, ct).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/auth/r/info/margin/base", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexMarginBase>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/info/margin/{symbol}", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexMarginSymbol>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexMovement>>> GetMovementsAsync(string? asset = null, IEnumerable<long>? ids = null, string? address = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("id", ids);
            parameters.AddOptionalParameter("address", address);
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/" + (asset == null ? "auth/r/movements/hist" : $"auth/r/movements/{asset}/hist"), BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<IEnumerable<BitfinexMovement>>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMovementDetails>> GetMovementsDetailsAsync(long id, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "id", id }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"v2/auth/r/movements/info", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexMovementDetails>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexAlert>>> GetAlertListAsync(CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                { "type", "price" }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"/v2/auth/r/alerts", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(45, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<IEnumerable<BitfinexAlert>>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                { "type", "price" },
                { "symbol", symbol },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"/v2/auth/w/alert/set", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexAlert>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, $"/v2/auth/w/alert/price:{symbol}:{price.ToString(CultureInfo.InvariantCulture)}/del", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexSuccessResult>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal price, WalletType type, decimal? leverage = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                { "symbol", symbol },
                { "dir", side == OrderSide.Buy ? 1: -1 },
                { "rate", price.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddEnum("type", type);
            parameters.AddOptionalParameter("lev", leverage?.ToString(CultureInfo.InvariantCulture));

            var request = _definitions.GetOrCreate(HttpMethod.Post, $"/v2/auth/calc/order/avail", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexAvailableBalance>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexLedgerEntry>>> GetLedgerEntriesAsync(string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? category = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);

            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("category", category);
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var url = string.IsNullOrEmpty(asset)
                ? "/v2/auth/r/ledgers/hist" : $"/v2/auth/r/ledgers/{asset}/hist";

            var request = _definitions.GetOrCreate(HttpMethod.Post, url, BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<IEnumerable<BitfinexLedgerEntry>>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexUserInfo>> GetUserInfoAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/auth/r/info/user", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexUserInfo>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexSummary>> Get30DaySummaryAndFeesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/auth/r/summary", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexSummary>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexDepositAddress>>> GetDepositAddressAsync(string method, WithdrawWallet toWallet, bool? forceNew = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection
            {
                { "method", method }
            };
            parameters.AddEnum("wallet", toWallet);
            parameters.AddOptionalParameter("op_renew", forceNew == null ? null : forceNew == true ? 1 : 0);

            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/auth/w/deposit/address", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(10, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexWriteResult<BitfinexDepositAddress>>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexTransfer>>> WalletTransferAsync(string asset, decimal quantity, WithdrawWallet fromWallet, WithdrawWallet toWallet, string? toAsset = null, string? emailDestination = null, long? userIdDestination = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new ParameterCollection
            {
                { "currency", asset },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddEnum("from", fromWallet);
            parameters.AddEnum("to", toWallet);
            parameters.AddOptionalParameter("currency_to", toAsset);
            parameters.AddOptionalParameter("email_dst", emailDestination);
            parameters.AddOptionalParameter("user_id_dst", userIdDestination);

            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/auth/w/transfer", BitfinexExchange.RateLimiter.Overal, 1, true);
            return await _baseClient.SendAsync<BitfinexWriteResult<BitfinexTransfer>>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWithdrawalResult>> WithdrawAsync(string withdrawType,
                                                                         WithdrawWallet wallet,
                                                                         decimal quantity,
                                                                         string? address = null,
                                                                         string? accountNumber = null,
                                                                         string? bankSwift = null,
                                                                         string? bankName = null,
                                                                         string? bankAddress = null,
                                                                         string? bankCity = null,
                                                                         string? bankCountry = null,
                                                                         string? paymentDetails = null,
                                                                         bool? expressWire = null,
                                                                         string? intermediaryBankName = null,
                                                                         string? intermediaryBankAddress = null,
                                                                         string? intermediaryBankCity = null,
                                                                         string? intermediaryBankCountry = null,
                                                                         string? intermediaryBankAccount = null,
                                                                         string? intermediaryBankSwift = null,
                                                                         string? accountName = null,
                                                                         string? paymentId = null,
                                                                         CancellationToken ct = default)
        {
            withdrawType.ValidateNotNull(nameof(withdrawType));
            var parameters = new ParameterCollection
            {
                { "withdraw_type", withdrawType },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddEnum("walletselected", wallet);
            parameters.AddOptionalParameter("address", address);
            parameters.AddOptionalParameter("payment_id", paymentId);
            parameters.AddOptionalParameter("account_name", accountName);
            parameters.AddOptionalParameter("account_number", accountNumber);
            parameters.AddOptionalParameter("swift", bankSwift);
            parameters.AddOptionalParameter("bank_name", bankName);
            parameters.AddOptionalParameter("bank_address", bankAddress);
            parameters.AddOptionalParameter("bank_city", bankCity);
            parameters.AddOptionalParameter("bank_country", bankCountry);
            parameters.AddOptionalParameter("detail_payment", paymentDetails);
            parameters.AddOptionalParameter("expressWire", expressWire == null ? null : expressWire == true ? "1": "0");
            parameters.AddOptionalParameter("intermediary_bank_name", intermediaryBankName);
            parameters.AddOptionalParameter("intermediary_bank_address", intermediaryBankAddress);
            parameters.AddOptionalParameter("intermediary_bank_city", intermediaryBankCity);
            parameters.AddOptionalParameter("intermediary_bank_country", intermediaryBankCountry);
            parameters.AddOptionalParameter("intermediary_bank_account", intermediaryBankAccount);
            parameters.AddOptionalParameter("intermediary_bank_swift", intermediaryBankSwift);

            var request = _definitions.GetOrCreate(HttpMethod.Post, "v1/withdraw", BitfinexExchange.RateLimiter.Overal, 1, true);
            var result = await _baseClient.SendAsync<IEnumerable<BitfinexWithdrawalResult>>(request, parameters, ct).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexWithdrawalResult>(default);

            var data = result.Data.First();
            if (!data.Success)
                return result.AsError<BitfinexWithdrawalResult>(new ServerError(data.Message));
            return result.As(data);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWithdrawalResultV2>> WithdrawV2Async(string method,
                                                                         WithdrawWallet wallet,
                                                                         decimal quantity,
                                                                         string? address = null,
                                                                         string? invoice = null,
                                                                         string? paymentId = null,
                                                                         bool? feeFromWithdrawalAmount = null,
                                                                         string? note = null,
                                                                         CancellationToken ct = default)
        {
            method.ValidateNotNull(nameof(method));
            var parameters = new ParameterCollection
            {
                { "method", method },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddEnum("wallet", wallet);
            parameters.AddOptionalParameter("address", address);
            parameters.AddOptionalParameter("payment_id", paymentId);
            parameters.AddOptionalParameter("invoice", invoice);
            parameters.AddOptionalParameter("note", note);
            parameters.AddOptionalParameter("fee_deduct", feeFromWithdrawalAmount == null ? null : feeFromWithdrawalAmount == true ? 1: 0);

            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/auth/w/withdraw", BitfinexExchange.RateLimiter.Overal, 1, true);
            return await _baseClient.SendAsync<BitfinexWithdrawalResultV2>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexLogin>>> GetLoginHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 250);

            var parameters = new ParameterCollection();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/auth/r/logins/hist", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<IEnumerable<BitfinexLogin>>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexPermission>>> GetApiKeyPermissionsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/auth/r/permissions", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<IEnumerable<BitfinexPermission>>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexChangeLog>>> GetAccountChangeLogAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/auth/r/audit/hist", BitfinexExchange.RateLimiter.Overal, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<IEnumerable<BitfinexChangeLog>>(request, null, ct).ConfigureAwait(false);
        }
    }
}
