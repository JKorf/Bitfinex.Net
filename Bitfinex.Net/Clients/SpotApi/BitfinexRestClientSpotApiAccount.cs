using Bitfinex.Net.Converters;
using Bitfinex.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
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
using Bitfinex.Net.ExtensionMethods;

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc />
    internal class BitfinexRestClientSpotApiAccount : IBitfinexRestClientSpotApiAccount
    {
        private readonly BitfinexRestClientSpotApi _baseClient;

        internal BitfinexRestClientSpotApiAccount(BitfinexRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }


        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexWallet>>> GetBalancesAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexWallet>>(_baseClient.GetUrl("auth/r/wallets", "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexMarginBase>(_baseClient.GetUrl("auth/r/info/margin/base", "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol, CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexMarginSymbol>(_baseClient.GetUrl($"auth/r/info/margin/{symbol}", "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexMovement>>> GetMovementsAsync(string? asset = null, IEnumerable<long>? ids = null, string? address = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("id", ids);
            parameters.AddOptionalParameter("address", address);
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var url = _baseClient.GetUrl(asset == null ? "auth/r/movements/hist" : $"auth/r/movements/{asset}/hist", "2");
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexMovement>>(url, HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMovementDetails>> GetMovementsDetailsAsync(long id, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "id", id }
            };

            return await _baseClient.SendRequestAsync<BitfinexMovementDetails>(_baseClient.GetUrl("auth/r/movements/info", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexAlert>>> GetAlertListAsync(CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "type", "price" }
            };

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexAlert>>(_baseClient.GetUrl("auth/r/alerts", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "type", "price" },
                { "symbol", symbol },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };

            return await _baseClient.SendRequestAsync<BitfinexAlert>(_baseClient.GetUrl("auth/w/alert/set", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price, CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexSuccessResult>(_baseClient.GetUrl($"auth/w/alert/price:{symbol}:{price.ToString(CultureInfo.InvariantCulture)}/del", "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal price, WalletType type, decimal? leverage = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "dir", side == OrderSide.Buy ? 1: -1 },
                { "rate", price.ToString(CultureInfo.InvariantCulture) },
                { "type", JsonConvert.SerializeObject(type, new WalletTypeConverter(false)).ToUpper() }
            };
            parameters.AddOptionalParameter("lev", leverage?.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestAsync<BitfinexAvailableBalance>(_baseClient.GetUrl("auth/calc/order/avail", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexLedgerEntry>>> GetLedgerEntriesAsync(string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? category = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 500);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("category", category);
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            var url = string.IsNullOrEmpty(asset)
                ? "auth/r/ledgers/hist" : $"auth/r/ledgers/{asset}/hist";

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexLedgerEntry>>(_baseClient.GetUrl(url, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexUserInfo>> GetUserInfoAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexUserInfo>(_baseClient.GetUrl("auth/r/info/user", "2"), HttpMethod.Post, ct, signed: true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexSummary>> Get30DaySummaryAndFeesAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexSummary>(_baseClient.GetUrl("auth/r/summary", "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexDepositAddress>>> GetDepositAddressAsync(string method, WithdrawWallet toWallet, bool? forceNew = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "method", method },
                { "wallet", JsonConvert.SerializeObject(toWallet, new WithdrawWalletConverter(false)) }
            };
            parameters.AddOptionalParameter("op_renew", forceNew == null ? null : forceNew == true ? 1 : 0);

            return await _baseClient.SendRequestAsync<BitfinexWriteResult<BitfinexDepositAddress>>(_baseClient.GetUrl("auth/w/deposit/address", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWriteResult<BitfinexTransfer>>> WalletTransferAsync(string asset, decimal quantity, WithdrawWallet fromWallet, WithdrawWallet toWallet, string? toAsset = null, string? emailDestination = null, long? userIdDestination = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Dictionary<string, object>
            {
                { "currency", asset },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) },
                { "from", JsonConvert.SerializeObject(fromWallet, new WithdrawWalletConverter(false)) },
                { "to", JsonConvert.SerializeObject(toWallet, new WithdrawWalletConverter(false)) },
            };
            parameters.AddOptionalParameter("currency_to", toAsset);
            parameters.AddOptionalParameter("email_dst", emailDestination);
            parameters.AddOptionalParameter("user_id_dst", userIdDestination);

            return await _baseClient.SendRequestAsync<BitfinexWriteResult<BitfinexTransfer>>(_baseClient.GetUrl("auth/w/transfer", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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
            var parameters = new Dictionary<string, object>
            {
                { "withdraw_type", withdrawType },
                { "walletselected", JsonConvert.SerializeObject(wallet, new WithdrawWalletConverter(false)) },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
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
            parameters.AddOptionalParameter("expressWire", expressWire == null ? null : JsonConvert.SerializeObject(expressWire, new BoolToIntConverter(false)));
            parameters.AddOptionalParameter("intermediary_bank_name", intermediaryBankName);
            parameters.AddOptionalParameter("intermediary_bank_address", intermediaryBankAddress);
            parameters.AddOptionalParameter("intermediary_bank_city", intermediaryBankCity);
            parameters.AddOptionalParameter("intermediary_bank_country", intermediaryBankCountry);
            parameters.AddOptionalParameter("intermediary_bank_account", intermediaryBankAccount);
            parameters.AddOptionalParameter("intermediary_bank_swift", intermediaryBankSwift);

            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexWithdrawalResult>>(_baseClient.GetUrl("withdraw", "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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
            var parameters = new Dictionary<string, object>
            {
                { "method", method },
                { "wallet", JsonConvert.SerializeObject(wallet, new WithdrawWalletConverter(false)) },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("address", address);
            parameters.AddOptionalParameter("payment_id", paymentId);
            parameters.AddOptionalParameter("invoice", invoice);
            parameters.AddOptionalParameter("note", note);
            parameters.AddOptionalParameter("fee_deduct", feeFromWithdrawalAmount == null ? null : feeFromWithdrawalAmount == true ? 1: 0);

            return await _baseClient.SendRequestAsync<BitfinexWithdrawalResultV2>(_baseClient.GetUrl("auth/w/withdraw", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexLogin>>> GetLoginHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 250);

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("start", DateTimeConverter.ConvertToMilliseconds(startTime));
            parameters.AddOptionalParameter("end", DateTimeConverter.ConvertToMilliseconds(endTime));

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexLogin>>(_baseClient.GetUrl("auth/r/logins/hist", "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexPermission>>> GetApiKeyPermissionsAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexPermission>>(_baseClient.GetUrl("auth/r/permissions", "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexChangeLog>>> GetAccountChangeLogAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexChangeLog>>(_baseClient.GetUrl("auth/r/audit/hist", "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }
    }
}
