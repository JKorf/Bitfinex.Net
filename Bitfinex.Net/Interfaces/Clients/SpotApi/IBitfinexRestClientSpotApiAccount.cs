using Bitfinex.Net.Enums;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.V1;

namespace Bitfinex.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Bitfinex account endpoints. Account endpoints include balance info, withdraw/deposit info and requesting and account settings
    /// </summary>
    public interface IBitfinexRestClientSpotApiAccount
    {
        /// <summary>
        /// Get all balances
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-wallets" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWallet[]>> GetBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the base margin info
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-info-margin" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the margin info for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-info-margin" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the info for, for example `tETHUSD`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get the withdrawal/deposit history
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-movements" /></para>
        /// </summary>
        /// <param name="asset">Asset to get history for, for example `ETH`</param>
        /// <param name="ids">Filter by ids</param>
        /// <param name="address">Filter by deposit address</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMovement[]>> GetMovementsAsync(string? asset = null, IEnumerable<long>? ids = null, string? address = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get detailed information about a deposit/withdrawal
        /// <para><a href="https://docs.bitfinex.com/reference/movement-info" /></para>
        /// </summary>
        /// <param name="id">Id of the movement</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMovementDetails>> GetMovementsDetailsAsync(long id, CancellationToken ct = default);

        /// <summary>
        /// Get the list of alerts
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-alerts" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAlert[]>> GetAlertListAsync(CancellationToken ct = default);

        /// <summary>
        /// Set an alert
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-alert-set" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to set the alert for, for example `tETHUSD`</param>
        /// <param name="price">The price to set the alert for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price, CancellationToken ct = default);

        /// <summary>
        /// Delete an existing alert
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-alert-del" /></para>
        /// </summary>
        /// <param name="symbol">The symbol of the alert to delete, for example `tETHUSD`</param>
        /// <param name="price">The price of the alert to delete</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price, CancellationToken ct = default);

        /// <summary>
        /// Calculates the available balance for a symbol at a specific rate
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-calc-order-avail" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="side">Buy or sell</param>
        /// <param name="rate">The rate/price</param>
        /// <param name="type">The wallet type</param>
        /// <param name="leverage">Leverage that you want to use in calculating the max order amount (DERIV only)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal rate, WalletType type, decimal? leverage = null, CancellationToken ct = default);

        /// <summary>
        /// Get changes in your balance for an asset
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-ledgers" /></para>
        /// </summary>
        /// <param name="asset">The asset to check the ledger for, for example `ETH`</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="category">Filter by category, see https://docs.bitfinex.com/reference#rest-auth-ledgers</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexLedgerEntry[]>> GetLedgerEntriesAsync(string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? category = null, CancellationToken ct = default);

        /// <summary>
        /// Gets information about the user associated with the api key/secret
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-info-user" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexUserInfo>> GetUserInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Provides an overview of the different fee rates for the account as well as the LEO discount level and the average amount of LEO held over the last 30 days.
        /// <para><a href="https://docs.bitfinex.com/reference/rest-auth-summary" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexSummary>> Get30DaySummaryAndFeesAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a deposit address for an asset
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-deposit" /></para>
        /// </summary>
        /// <param name="method">The method to get address for. Methods can be retrieved via ExchangeData.GetAssetDepositWithdrawalMethodsAsync</param>
        /// <param name="toWallet">The type of wallet the deposit is for</param>
        /// <param name="forceNew">If true a new address will be generated (previous addresses will still be valid)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResultDepositAddress>> GetDepositAddressAsync(string method, WithdrawWallet toWallet, bool? forceNew = null, CancellationToken ct = default);

        /// <summary>
        /// Transfers funds from one wallet to another
        /// <para><a href="https://docs.bitfinex.com/reference/rest-auth-transfer" /></para>
        /// </summary>
        /// <param name="asset">The asset to transfer, for example `ETH`</param>
        /// <param name="fromWallet">The wallet to remove funds from</param>
        /// <param name="toWallet">The wallet to add funds to</param>
        /// <param name="quantity">The quantity to transfer</param>
        /// <param name="toAsset">The asset that you would like to exchange to (USTF0 === USDT for derivatives pairs)</param>
        /// <param name="emailDestination">Allows transfer of funds to a sub- or master-account identified by the associated email address.</param>
        /// <param name="userIdDestination">Allows transfer of funds to a sub- or master-account identified by the associated user id.</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResultTransfer>> WalletTransferAsync(string asset, decimal quantity, WithdrawWallet fromWallet, WithdrawWallet toWallet, string? toAsset = null, string? emailDestination = null, long? userIdDestination = null, CancellationToken ct = default);

        /// <summary>
        /// Withdraw funds from Bitfinex, either to a crypto currency address or a bank account
        /// All withdrawals need the withdrawType, wallet and quantity parameters
        /// CryptoCurrency withdrawals need the address parameters, the paymentId can be used for Monero as payment id and for Ripple as tag
        /// Wire withdrawals need the bank parameters. In some cases your bank will require the use of an intermediary bank, if this is the case, please supply those fields as well.
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-withdrawal" /></para>
        /// </summary>
        /// <param name="withdrawType">The type of funds to withdraw</param>
        /// <param name="wallet">The wallet to withdraw from</param>
        /// <param name="quantity">The quantity to withdraw</param>
        /// <param name="address">The destination of the withdrawal</param>
        /// <param name="accountNumber">The account number</param>
        /// <param name="bankSwift">The SWIFT code of the bank</param>
        /// <param name="bankName">The bank name</param>
        /// <param name="bankAddress">The bank address</param>
        /// <param name="bankCity">The bank city</param>
        /// <param name="bankCountry">The bank country</param>
        /// <param name="paymentDetails">Message for the receiver</param>
        /// <param name="expressWire">Whether it is an express wire withdrawal</param>
        /// <param name="intermediaryBankName">Intermediary bank name</param>
        /// <param name="intermediaryBankAddress">Intermediary bank address</param>
        /// <param name="intermediaryBankCity">Intermediary bank city</param>
        /// <param name="intermediaryBankCountry">Intermediary bank country</param>
        /// <param name="intermediaryBankAccount">Intermediary bank account</param>
        /// <param name="intermediaryBankSwift">Intermediary bank SWIFT code</param>
        /// <param name="accountName">The name of the account</param>
        /// <param name="paymentId">Hex string for Monero transaction</param>
        /// <param name="travelRuleTos">Flag to voluntarily send travel rule details for withdrawal</param>
        /// <param name="vaspDid">Virtual asset provider identifier, optional info for travel rule purpose. DID values can be found on https://api-pub.bitfinex.com/v2/ext/vasps endpoint.</param>
        /// <param name="vaspName">Virtual asset provider name, optional info for travel rule purpose, if self custody ignore the field</param>
        /// <param name="beneficiarySelf">Set to 'true' to extract destination data from your KYC data. (If 'true', dest_firstname, dest_lastname, or dest_corp_name do not need to be supplied)</param>
        /// <param name="destFirstname">Destination entity first name for travel rule purpose (mandatory if dest_lastname is supplied, not required if beneficiary_self = true)</param>
        /// <param name="destLastname">Destination entity last name for travel rule purpose (mandatory if dest_firstname is supplied, not required if beneficiary_self = true)</param>
        /// <param name="destCorpName">Destination entity corporate name for travel rule purpose. (use either dest_firstname + dest_lastname or dest_corp_name, not required if beneficiary_self = true)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWithdrawalResult>> WithdrawAsync(string withdrawType,
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
            CancellationToken ct = default);

        /// <summary>
        /// Withdraw funds
        /// <para><a href="https://docs.bitfinex.com/reference/rest-auth-withdraw" /></para>
        /// </summary>
        /// <param name="method">Method of withdrawal, methods can be retrieved with <see cref="IBitfinexRestClientSpotApiExchangeData.GetAssetDepositWithdrawalMethodsAsync">ExchangeData.GetAssetDepositWithdrawalMethodsAsync</see></param>
        /// <param name="wallet">Wallet type</param>
        /// <param name="quantity">Quantity to withdraw</param>
        /// <param name="address">Withdrawal address</param>
        /// <param name="invoice">Invoice (for lightning withdrawals)</param>
        /// <param name="paymentId">Payment id (tag/memo)</param>
        /// <param name="feeFromWithdrawalAmount">When true the fee will be deducted from the withdrawal quantity</param>
        /// <param name="note">Note</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWithdrawalResultV2>> WithdrawV2Async(string method,
                                                                         WithdrawWallet wallet,
                                                                         decimal quantity,
                                                                         string? address = null,
                                                                         string? invoice = null,
                                                                         string? paymentId = null,
                                                                         bool? feeFromWithdrawalAmount = null,
                                                                         string? note = null,
                                                                         CancellationToken ct = default);
        /// <summary>
        /// Get login history
        /// <para><a href="https://docs.bitfinex.com/reference/rest-auth-logins-hist" /></para>
        /// </summary>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexLogin[]>> GetLoginHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get api key permissions
        /// <para><a href="https://docs.bitfinex.com/reference/key-permissions" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPermission[]>> GetApiKeyPermissionsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get account change log
        /// <para><a href="https://docs.bitfinex.com/reference/rest-auth-audit-hist" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexChangeLog[]>> GetAccountChangeLogAsync(CancellationToken ct = default);
    }
}
