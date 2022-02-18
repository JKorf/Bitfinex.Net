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
    public interface IBitfinexClientSpotApiAccount
    {
        /// <summary>
        /// Get all balances
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-wallets" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexWallet>>> GetBalancesAsync(CancellationToken ct = default);

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
        /// <param name="symbol">The symbol to get the info for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get the withdrawal/deposit history
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-movements" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to get history for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexMovement>>> GetMovementsAsync(string? symbol = null, CancellationToken ct = default);

        /// <summary>
        /// Get the list of alerts
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-alerts" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexAlert>>> GetAlertListAsync(CancellationToken ct = default);

        /// <summary>
        /// Set an alert
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-alert-set" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to set the alert for</param>
        /// <param name="price">The price to set the alert for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price, CancellationToken ct = default);

        /// <summary>
        /// Delete an existing alert
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-alert-del" /></para>
        /// </summary>
        /// <param name="symbol">The symbol of the alert to delete</param>
        /// <param name="price">The price of the alert to delete</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price, CancellationToken ct = default);

        /// <summary>
        /// Calculates the available balance for a symbol at a specific rate
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-calc-order-avail" /></para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="side">Buy or sell</param>
        /// <param name="rate">The rate/price</param>
        /// <param name="type">The wallet type</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal rate, WalletType type, CancellationToken ct = default);

        /// <summary>
        /// Get changes in your balance for an asset
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-ledgers" /></para>
        /// </summary>
        /// <param name="asset">The asset to check the ledger for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="category">Filter by category, see https://docs.bitfinex.com/reference#rest-auth-ledgers</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexLedgerEntry>>> GetLedgerEntriesAsync(string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? category = null, CancellationToken ct = default);

        /// <summary>
        /// Gets information about the user associated with the api key/secret
        /// <para><a href="https://docs.bitfinex.com/reference#rest-auth-info-user" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexUserInfo>> GetUserInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Get information about your account
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-account-info" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAccountInfo>> GetAccountInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Get withdrawal fees for this account
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-fees" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWithdrawalFees>> GetWithdrawalFeesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get 30-day summary on trading volume and margin funding
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-summary" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Bitfinex30DaySummary>> Get30DaySummaryAsync(CancellationToken ct = default);


        /// <summary>
        /// Gets a deposit address for an asset
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-deposit" /></para>
        /// </summary>
        /// <param name="asset">The asset to get address for</param>
        /// <param name="toWallet">The type of wallet the deposit is for</param>
        /// <param name="forceNew">If true a new address will be generated (previous addresses will still be valid)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexDepositAddress>> GetDepositAddressAsync(string asset, WithdrawWallet toWallet, bool? forceNew = null, CancellationToken ct = default);

        /// <summary>
        /// Transfers funds from one wallet to another
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-auth-transfer-between-wallets" /></para>
        /// </summary>
        /// <param name="asset">The asset to transfer</param>
        /// <param name="fromWallet">The wallet to remove funds from</param>
        /// <param name="toWallet">The wallet to add funds to</param>
        /// <param name="quantity">The quantity to transfer</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexTransferResult>> WalletTransferAsync(string asset, decimal quantity, WithdrawWallet fromWallet, WithdrawWallet toWallet, CancellationToken ct = default);

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
            CancellationToken ct = default);
    }
}
