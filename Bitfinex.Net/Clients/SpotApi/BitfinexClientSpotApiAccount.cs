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

namespace Bitfinex.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class BitfinexClientSpotApiAccount : IBitfinexClientSpotApiAccount
    {
        private const string WalletsEndpoint = "auth/r/wallets";
        private const string CalcAvailableBalanceEndpoint = "auth/calc/order/avail";
        private const string UserInfoEndpoint = "auth/r/info/user";
        private const string LedgerEntriesSingleEndpoint = "auth/r/ledgers/hist";
        private const string LedgerEntriesEndpoint = "auth/r/ledgers/{}/hist";
        private const string AllMovementsEndpoint = "auth/r/movements/hist";
        private const string MovementsEndpoint = "auth/r/movements/{}/hist";
        private const string DailyPerformanceEndpoint = "auth/r/stats/perf:1D/hist";
        private const string AlertListEndpoint = "auth/r/alerts";
        private const string SetAlertEndpoint = "auth/w/alert/set";
        private const string DeleteAlertEndpoint = "auth/w/alert/price:{}:{}/del";
        private const string AccountInfoEndpoint = "account_infos";
        private const string DepositAddressEndpoint = "deposit/new";
        private const string TransferEndpoint = "transfer";
        private const string WithdrawEndpoint = "withdraw";
        private const string MarginInfoBaseEndpoint = "auth/r/info/margin/base";
        private const string MarginInfoSymbolEndpoint = "auth/r/info/margin/{}";
        private const string SummaryEndpoint = "summary";
        private const string WithdrawalFeeEndpoint = "account_fees";

        private readonly BitfinexClientSpotApi _baseClient;

        internal BitfinexClientSpotApiAccount(BitfinexClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }


        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexWallet>>> GetBalancesAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexWallet>>(_baseClient.GetUrl(WalletsEndpoint, "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexMarginBase>(_baseClient.GetUrl(MarginInfoBaseEndpoint, "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            return await _baseClient.SendRequestAsync<BitfinexMarginSymbol>(_baseClient.GetUrl(MarginInfoSymbolEndpoint.FillPathParameters(symbol), "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexMovement>>> GetMovementsAsync(string? symbol = null, CancellationToken ct = default)
        {
            var url = _baseClient.GetUrl(symbol == null ? AllMovementsEndpoint : MovementsEndpoint.FillPathParameters(symbol), "2");
            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexMovement>>(url, HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<BitfinexAlert>>> GetAlertListAsync(CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "type", "price" }
            };

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexAlert>>(_baseClient.GetUrl(AlertListEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            var parameters = new Dictionary<string, object>
            {
                { "type", "price" },
                { "symbol", symbol },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };

            return await _baseClient.SendRequestAsync<BitfinexAlert>(_baseClient.GetUrl(SetAlertEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();

            return await _baseClient.SendRequestAsync<BitfinexSuccessResult>(_baseClient.GetUrl(DeleteAlertEndpoint.FillPathParameters(symbol, price.ToString(CultureInfo.InvariantCulture)), "2"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal price, WalletType type, CancellationToken ct = default)
        {
            symbol.ValidateBitfinexSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "dir", side == OrderSide.Buy ? 1: -1 },
                { "rate", price.ToString(CultureInfo.InvariantCulture) },
                { "type", JsonConvert.SerializeObject(type, new WalletTypeConverter(false)).ToUpper() }
            };

            return await _baseClient.SendRequestAsync<BitfinexAvailableBalance>(_baseClient.GetUrl(CalcAvailableBalanceEndpoint, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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
                ? LedgerEntriesSingleEndpoint : LedgerEntriesEndpoint.FillPathParameters(asset!);

            return await _baseClient.SendRequestAsync<IEnumerable<BitfinexLedgerEntry>>(_baseClient.GetUrl(url, "2"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexUserInfo>> GetUserInfoAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexUserInfo>(_baseClient.GetUrl(UserInfoEndpoint, "2"), HttpMethod.Post, ct, signed: true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexAccountInfo>> GetAccountInfoAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexAccountInfo>>(_baseClient.GetUrl(AccountInfoEndpoint, "1"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
            return result ? result.As(result.Data.First()) : result.As<BitfinexAccountInfo>(default);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Bitfinex30DaySummary>> Get30DaySummaryAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<Bitfinex30DaySummary>(_baseClient.GetUrl(SummaryEndpoint, "1"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexDepositAddress>> GetDepositAddressAsync(string asset, WithdrawWallet toWallet, bool? forceNew = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Dictionary<string, object>
            {
                { "method", asset },
                { "wallet_name", JsonConvert.SerializeObject(toWallet, new WithdrawWalletConverter(false)) }
            };
            parameters.AddOptionalParameter("renew", forceNew.HasValue ? JsonConvert.SerializeObject(toWallet, new BoolToIntConverter(false)) : null);

            return await _baseClient.SendRequestAsync<BitfinexDepositAddress>(_baseClient.GetUrl(DepositAddressEndpoint, "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexTransferResult>> WalletTransferAsync(string asset, decimal quantity, WithdrawWallet fromWallet, WithdrawWallet toWallet, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            var parameters = new Dictionary<string, object>
            {
                { "asset", asset },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) },
                { "walletfrom", JsonConvert.SerializeObject(fromWallet, new WithdrawWalletConverter(false)) },
                { "walletto", JsonConvert.SerializeObject(toWallet, new WithdrawWalletConverter(false)) },
            };
            var result = await _baseClient.SendRequestAsync<BitfinexTransferResult[]>(_baseClient.GetUrl(TransferEndpoint, "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (!result)
                return result.As((BitfinexTransferResult)null!);

            return result.As(result.Data.First());
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

            var result = await _baseClient.SendRequestAsync<IEnumerable<BitfinexWithdrawalResult>>(_baseClient.GetUrl(WithdrawEndpoint, "1"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (!result)
                return result.As<BitfinexWithdrawalResult>(default);

            var data = result.Data.First();
            if (!data.Success)
                return result.AsError<BitfinexWithdrawalResult>(new ServerError(data.Message));
            return result.As(data);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<BitfinexWithdrawalFees>> GetWithdrawalFeesAsync(CancellationToken ct = default)
        {
            return await _baseClient.SendRequestAsync<BitfinexWithdrawalFees>(_baseClient.GetUrl(WithdrawalFeeEndpoint, "1"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }
    }
}
