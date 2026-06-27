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
using CryptoExchange.Net.RateLimiting.Guards;
using CryptoExchange.Net.Objects.Errors;
using Bitfinex.Net.Interfaces.Clients.ExchangeApi;

namespace Bitfinex.Net.Clients.ExchangeApi
{
    /// <inheritdoc />
    internal class BitfinexRestClientExchangeApiAccount : IBitfinexRestClientExchangeApiAccount
    {
        private static readonly RequestDefinitionCache _definitions = new();
        private readonly BitfinexRestClientExchangeApi _baseClient;

        internal BitfinexRestClientExchangeApiAccount(BitfinexRestClientExchangeApi baseClient)
        {
            _baseClient = baseClient;
        }


        /// <inheritdoc />
        public async Task<HttpResult<BitfinexWallet[]>> GetBalancesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/auth/r/wallets", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexWallet[]>(request, null, ct).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<HttpResult<BitfinexMarginBase>> GetBaseMarginInfoAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/auth/r/info/margin/base", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexMarginBase>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, $"v2/auth/r/info/margin/{symbol}", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexMarginSymbol>(request, null, ct).ConfigureAwait(false);
        }

        public async Task<HttpResult<BitfinexMarginSymbol[]>> GetSymbolMarginInfoSymbolsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, $"v2/auth/r/info/margin/sym_all", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexMarginSymbol[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexMovement[]>> GetMovementsAsync(string? asset = null, IEnumerable<long>? ids = null, string? address = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings);
            parameters.AddRaw("id", ids);
            parameters.Add("address", address);
            parameters.Add("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.Add("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.Add("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, $"v2/" + (asset == null ? "auth/r/movements/hist" : $"auth/r/movements/{asset}/hist"), BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexMovement[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexMovementDetails>> GetMovementsDetailsAsync(long id, CancellationToken ct = default)
        {
            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings)
            {
                { "id", id }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, $"v2/auth/r/movements/info", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexMovementDetails>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexAlert[]>> GetAlertListAsync(CancellationToken ct = default)
        {
            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings)
            {
                { "type", "price" }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, $"/v2/auth/r/alerts", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(45, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexAlert[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price, CancellationToken ct = default)
        {
            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings)
            {
                { "type", "price" },
                { "symbol", symbol },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, $"/v2/auth/w/alert/set", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexAlert>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price, CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, $"/v2/auth/w/alert/price:{symbol}:{price.ToString(CultureInfo.InvariantCulture)}/del", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexSuccessResult>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal price, WalletType type, decimal? leverage = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings)
            {
                { "symbol", symbol },
                { "dir", side == OrderSide.Buy ? 1: -1 },
                { "rate", price.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.Add("type", type);
            parameters.Add("lev", leverage?.ToString(CultureInfo.InvariantCulture));

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, $"/v2/auth/calc/order/avail", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexAvailableBalance>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexLedgerEntry[]>> GetLedgerEntriesAsync(string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? category = null, WalletType? walletType = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 2500);

            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings);
            parameters.Add("category", category);
            parameters.Add("wallet", walletType);
            parameters.Add("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.Add("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.Add("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var url = string.IsNullOrEmpty(asset)
                ? "/v2/auth/r/ledgers/hist" : $"/v2/auth/r/ledgers/{asset}/hist";

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, url, BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexLedgerEntry[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexUserInfo>> GetUserInfoAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/auth/r/info/user", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexUserInfo>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexSummary>> Get30DaySummaryAndFeesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/auth/r/summary", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexSummary>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexWriteResultDepositAddress>> GetDepositAddressAsync(string method, WithdrawWallet toWallet, bool? forceNew = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings)
            {
                { "method", method }
            };
            parameters.Add("wallet", toWallet);
            parameters.Add("op_renew", forceNew == null ? null : forceNew == true ? 1 : 0);

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/auth/w/deposit/address", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(10, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexWriteResultDepositAddress>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexWriteResultTransfer>> WalletTransferAsync(string asset, decimal quantity, WithdrawWallet fromWallet, WithdrawWallet toWallet, string? toAsset = null, string? emailDestination = null, long? userIdDestination = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings)
            {
                { "currency", asset },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.Add("from", fromWallet);
            parameters.Add("to", toWallet);
            parameters.Add("currency_to", toAsset);
            parameters.Add("email_dst", emailDestination);
            parameters.Add("user_id_dst", userIdDestination);

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/auth/w/transfer", BitfinexExchange.RateLimiter.Overall, 1, true);
            return await _baseClient.SendAsync<BitfinexWriteResultTransfer>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexWithdrawalResult>> WithdrawAsync(string withdrawType,
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
                                                                         bool? travelRuleTos = null,
                                                                         string? vaspDid = null,
                                                                         string? vaspName = null,
                                                                         bool? beneficiarySelf = null,
                                                                         string? destFirstname = null,
                                                                         string? destLastname = null,
                                                                         string? destCorpName = null,
                                                                         CancellationToken ct = default)
        {
            withdrawType.ValidateNotNull(nameof(withdrawType));
            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings)
            {
                { "withdraw_type", withdrawType },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.Add("walletselected", wallet);
            parameters.Add("address", address);
            parameters.Add("payment_id", paymentId);
            parameters.Add("account_name", accountName);
            parameters.Add("account_number", accountNumber);
            parameters.Add("swift", bankSwift);
            parameters.Add("bank_name", bankName);
            parameters.Add("bank_address", bankAddress);
            parameters.Add("bank_city", bankCity);
            parameters.Add("bank_country", bankCountry);
            parameters.Add("detail_payment", paymentDetails);
            parameters.Add("expressWire", expressWire == null ? null : expressWire == true ? "1" : "0");
            parameters.Add("intermediary_bank_name", intermediaryBankName);
            parameters.Add("intermediary_bank_address", intermediaryBankAddress);
            parameters.Add("intermediary_bank_city", intermediaryBankCity);
            parameters.Add("intermediary_bank_country", intermediaryBankCountry);
            parameters.Add("intermediary_bank_account", intermediaryBankAccount);
            parameters.Add("intermediary_bank_swift", intermediaryBankSwift);
            parameters.Add("travel_rule_tos", travelRuleTos);
            parameters.Add("vasp_did", vaspDid);
            parameters.Add("vasp_name", vaspName);
            parameters.Add("beneficiary_self", beneficiarySelf);
            parameters.Add("dest_firstname", destFirstname);
            parameters.Add("dest_lastname", destLastname);
            parameters.Add("dest_corp_name", destCorpName);

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v1/withdraw", BitfinexExchange.RateLimiter.Overall, 1, true);
            var result = await _baseClient.SendAsync<BitfinexWithdrawalResult[]>(request, parameters, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<BitfinexWithdrawalResult>(result);

            var data = result.Data.First();
            if (!data.Success)
                return HttpResult.Fail<BitfinexWithdrawalResult>(result, new ServerError(ErrorInfo.Unknown with { Message = data.Message }));

            return HttpResult.Ok(result, data);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexWithdrawalResultV2>> WithdrawV2Async(string method,
                                                                         WithdrawWallet wallet,
                                                                         decimal quantity,
                                                                         string? address = null,
                                                                         string? invoice = null,
                                                                         string? paymentId = null,
                                                                         bool? feeFromWithdrawalAmount = null,
                                                                         string? note = null,
                                                                         bool? travelRuleTos = null,
                                                                         string? vaspDid = null,
                                                                         string? vaspName = null,
                                                                         bool? beneficiarySelf = null,
                                                                         string? destFirstname = null,
                                                                         string? destLastname = null,
                                                                         string? destCorpName = null,
                                                                         CancellationToken ct = default)
        {
            method.ValidateNotNull(nameof(method));
            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings)
            {
                { "method", method },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
			
            parameters.Add("wallet", wallet);
            parameters.Add("address", address);
            parameters.Add("payment_id", paymentId);
            parameters.Add("invoice", invoice);
            parameters.Add("note", note);
            parameters.Add("fee_deduct", feeFromWithdrawalAmount == null ? null : feeFromWithdrawalAmount == true ? 1 : 0);
            parameters.Add("travel_rule_tos", travelRuleTos);
            parameters.Add("vasp_did", vaspDid);
            parameters.Add("vasp_name", vaspName);
            parameters.Add("beneficiary_self", beneficiarySelf);
            parameters.Add("dest_firstname", destFirstname);
            parameters.Add("dest_lastname", destLastname);
            parameters.Add("dest_corp_name", destCorpName);

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/auth/w/withdraw", BitfinexExchange.RateLimiter.Overall, 1, true);
            return await _baseClient.SendAsync<BitfinexWithdrawalResultV2>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexLogin[]>> GetLoginHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 250);

            var parameters = new Parameters(BitfinexExchange._parameterSerializationSettings);
            parameters.Add("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.Add("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.Add("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/auth/r/logins/hist", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexLogin[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexPermission[]>> GetApiKeyPermissionsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/auth/r/permissions", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexPermission[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<BitfinexChangeLog[]>> GetAccountChangeLogAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/auth/r/audit/hist", BitfinexExchange.RateLimiter.Overall, 1, true,
                limitGuard: new SingleLimitGuard(90, TimeSpan.FromSeconds(60), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<BitfinexChangeLog[]>(request, null, ct).ConfigureAwait(false);
        }
    }
}
