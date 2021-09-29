using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.RestV1Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Interfaces
{
    /// <summary>
    /// Interface for the Bitfinex client
    /// </summary>
    public interface IBitfinexClient: IRestClient
    {
        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        /// <param name="nonceProvider">Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that</param>
        void SetApiCredentials(string apiKey, string apiSecret, INonceProvider? nonceProvider = null);

        /// <summary>
        /// Gets the platform status
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Whether Bitfinex platform is running normally or not</returns>
        Task<WebCallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a list of supported currencies
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexCurrency>>> GetCurrenciesAsync(CancellationToken ct = default);

        /// <summary>
        /// Returns basic market data for the provided symbols
        /// </summary>
        /// <param name="symbol">The symbol to get data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Symbol data</returns>
        Task<WebCallResult<IEnumerable<BitfinexSymbolOverview>>> GetTickerAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Returns basic market data for the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols to get data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Symbol data</returns>
        Task<WebCallResult<IEnumerable<BitfinexSymbolOverview>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Get recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get trades for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time to return trades for</param>
        /// <param name="endTime">The end time to return trades for</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Trades for the symbol</returns>
        Task<WebCallResult<IEnumerable<BitfinexTradeSimple>>> GetTradeHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="precision">The precision of the data</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The order book for the symbol</returns>
        Task<WebCallResult<IEnumerable<BitfinexOrderBookEntry>>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get various stats for the symbol
        /// </summary>
        /// <param name="symbol">The symbol to request stats for</param>
        /// <param name="key">The type of stats</param>
        /// <param name="side">Side of the stats</param>
        /// <param name="section">Section of the stats</param>
        /// <param name="sorting">The way the result should be sorted</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats>> GetStatsAsync(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Get the last kline for a symbol
        /// </summary>
        /// <param name="timeFrame">The time frame of the kline</param>
        /// <param name="symbol">The symbol to get the kline for</param>
        /// <param name="fundingPeriod">The Funding period. Only required for funding candles. Enter after the symbol (trade:1m:fUSD:p30/hist).</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The last kline for the symbol</returns>
        Task<WebCallResult<BitfinexKline>> GetLastKlineAsync(TimeFrame timeFrame, string symbol, string? fundingPeriod = null, CancellationToken ct = default);

        /// <summary>
        /// Gets klines for a symbol
        /// </summary>
        /// <param name="timeFrame">The time frame of the klines</param>
        /// <param name="symbol">The symbol to get the klines for</param>
        /// <param name="fundingPeriod">The Funding period. Only required for funding candles. Enter after the symbol (trade:1m:fUSD:p30/hist).</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time of the klines</param>
        /// <param name="endTime">The end time of the klines</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexKline>>> GetKlinesAsync(TimeFrame timeFrame, string symbol, string? fundingPeriod = null, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Calculate the average execution price
        /// </summary>
        /// <param name="symbol">The symbol to calculate for</param>
        /// <param name="amount">The amount to execute</param>
        /// <param name="rateLimit">Limit to price</param>
        /// <param name="period">Maximum period for margin funding</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The average price at which the execution would happen</returns>
        Task<WebCallResult<BitfinexAveragePrice>> GetAveragePriceAsync(string symbol, decimal amount, decimal? rateLimit = null, int? period = null, CancellationToken ct = default);

        /// <summary>
        /// Returns the exchange rate for the currencies
        /// </summary>
        /// <param name="currency1">The first currency</param>
        /// <param name="currency2">The second currency</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange rate</returns>
        Task<WebCallResult<BitfinexForeignExchangeRate>> GetForeignExchangeRateAsync(string currency1, string currency2, CancellationToken ct = default);

        /// <summary>
        /// Get all funds
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexWallet>>> GetBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the active orders
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetActiveOrdersAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the order history for a symbol for this account
        /// </summary>
        /// <param name="symbol">The symbol to get the history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetOrdersAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the individual trades for an order
        /// </summary>
        /// <param name="symbol">The symbol of the order</param>
        /// <param name="orderId">The order Id</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexTradeDetails>>> GetOrderTradesAsync(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Get the trade history for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexTradeDetails>>> GetUserTradesAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the active positions
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetActivePositionsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get a list of historical positions
        /// </summary>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexPositionExtended>>> GetPositionHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get positions by id
        /// </summary>
        /// <param name="ids">The id's of positions to return</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexPositionExtended>>> GetPositionsByIdAsync(IEnumerable<string> ids, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the active funding offers
        /// </summary>
        /// <param name="symbol">The symbol to return the funding offer for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexFundingOffer>>> GetActiveFundingOffersAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get the funding offer history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexFundingOffer>>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Submit a new funding offer.
        /// </summary>
        /// <param name="fundingOrderType">Order Type (LIMIT, FRRDELTAVAR, FRRDELTAFIX).</param>
        /// <param name="symbol">Symbol for desired pair (fUSD, fBTC, etc..).</param>
        /// <param name="amount">Amount (positive for offer, negative for bid).</param>
        /// <param name="rate">Daily rate.</param>
        /// <param name="period">Time period of offer. Minimum 2 days. Maximum 120 days.</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResult<BitfinexFundingOffer>>> SubmitFundingOfferAsync(FundingOrderType fundingOrderType, string symbol, decimal amount, decimal rate, int period, CancellationToken ct = default);

        /// <summary>
        /// Cancels an existing Funding Offer based on the offer ID entered.
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingOffer>> CancelFundingOfferAsync(long offerId, CancellationToken ct = default);

        /// <summary>
        /// Get the funding loans
        /// </summary>
        /// <param name="symbol">The symbol to get the funding loans for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexFunding>>> GetFundingLoansAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get the funding loan history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexFunding>>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the funding credits
        /// </summary>
        /// <param name="symbol">The symbol to get the funding credits for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexFundingCredit>>> GetFundingCreditsAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get the funding credits history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexFundingCredit>>> GetFundingCreditsHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the funding trades history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexFundingTrade>>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the base margin info
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the margin info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get funding info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get the withdrawal/deposit history
        /// </summary>
        /// <param name="symbol">Symbol to get history for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexMovement>>> GetMovementsAsync(string? symbol, CancellationToken ct = default);

        /// <summary>
        /// Daily performance
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPerformance>> GetDailyPerformanceAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the list of alerts
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexAlert>>> GetAlertListAsync(CancellationToken ct = default);

        /// <summary>
        /// Set an alert
        /// </summary>
        /// <param name="symbol">The symbol to set the alert for</param>
        /// <param name="price">The price to set the alert for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price, CancellationToken ct = default);

        /// <summary>
        /// Delete an existing alert
        /// </summary>
        /// <param name="symbol">The symbol of the alert to delete</param>
        /// <param name="price">The price of the alert to delete</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price, CancellationToken ct = default);

        /// <summary>
        /// Calculates the available balance for a symbol at a specific rate
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="side">Buy or sell</param>
        /// <param name="rate">The rate/price</param>
        /// <param name="type">The wallet type</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal rate, WalletType type, CancellationToken ct = default);

        /// <summary>
        /// Get changes in your balance for a currency
        /// </summary>
        /// <param name="currency">The currency to check the ledger for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="category">Filter by category, see https://docs.bitfinex.com/reference#rest-auth-ledgers</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexLedgerEntry>>> GetLedgerEntriesAsync(string? currency = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, int? category = null, CancellationToken ct = default);

        /// <summary>
        /// Gets information about the user associated with the api key/secret
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexUserInfo>> GetUserInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the margin funding book
        /// </summary>
        /// <param name="currency">Currency to get the book for</param>
        /// <param name="limit">Limit of the results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingBook>> GetFundingBookAsync(string currency, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the most recent lends
        /// </summary>
        /// <param name="currency">Currency to get the book for</param>
        /// <param name="startTime">Return data after this time</param>
        /// <param name="limit">Limit of the results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexLend>>> GetLendsAsync(string currency, DateTime? startTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets details of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexSymbolDetails>>> GetSymbolDetailsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get information about your account
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAccountInfo>> GetAccountInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Get withdrawal fees for this account
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWithdrawalFees>> GetWithdrawalFeesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get 30-day summary on trading volume and margin funding
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Bitfinex30DaySummary>> Get30DaySummaryAsync(CancellationToken ct = default);

        /// <summary>
        /// Place a new order
        /// </summary>
        /// <param name="symbol">Symbol to place order for</param>
        /// <param name="side">Side of the order</param>
        /// <param name="type">Type of the order</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price for the order</param>
        /// <param name="affiliateCode">Affiliate code for the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <param name="flags"></param>
        /// <param name="leverage">Set the leverage for a derivative order, supported by derivative symbol orders only. The value should be between 1 and 100 inclusive. The field is optional, if omitted the default leverage value of 10 will be used.</param>
        /// <param name="groupId">Group id</param>
        /// <param name="clientOrderId">Client order id</param>
        /// <param name="priceTrailing">The trailing price for a trailing stop order</param>
        /// <param name="priceAuxLimit">Auxiliary Limit price (for STOP LIMIT)</param>
        /// <param name="priceOcoStop">OCO stop price</param>
        /// <param name="cancelTime">datetime for automatic order cancellation</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType type,
            decimal price,
            decimal amount,
            int? flags = null,
            int? leverage = null,
            int? groupId = null,
            int? clientOrderId = null,
            decimal? priceTrailing = null,
            decimal? priceAuxLimit = null,
            decimal? priceOcoStop = null,
            DateTime? cancelTime = null,
            string? affiliateCode = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel a specific order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="clientOrderId">The client order id of the order to cancel</param>
        /// <param name="clientOrderIdDate">The date of the client order (year month and day)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> CancelOrderAsync(long? orderId = null, long? clientOrderId = null, DateTime? clientOrderIdDate = null, CancellationToken ct = default);

        /// <summary>
        /// Cancels all open orders
        /// </summary>
        /// <param name="ct">Cancellation token</param><returns></returns>
        Task<WebCallResult<BitfinexResult>> CancelAllOrdersAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the status of a specific order
        /// </summary>
        /// <param name="orderId">The order id of the order to get</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPlacedOrder>> GetOrderAsync(long orderId, CancellationToken ct = default);

        /// <summary>
        /// Gets a deposit address for a currency
        /// </summary>
        /// <param name="currency">The currency to get address for</param>
        /// <param name="toWallet">The type of wallet the deposit is for</param>
        /// <param name="forceNew">If true a new address will be generated (previous addresses will still be valid)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexDepositAddress>> GetDepositAddressAsync(string currency, WithdrawWallet toWallet, bool? forceNew = null, CancellationToken ct = default);

        /// <summary>
        /// Transfers funds from one wallet to another
        /// </summary>
        /// <param name="currency">The currency to transfer</param>
        /// <param name="fromWallet">The wallet to remove funds from</param>
        /// <param name="toWallet">The wallet to add funds to</param>
        /// <param name="amount">The amount to transfer</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexTransferResult>> WalletTransferAsync(string currency, decimal amount, WithdrawWallet fromWallet, WithdrawWallet toWallet, CancellationToken ct = default);
        
        /// <summary>
        /// Withdraw funds from Bitfinex, either to a crypto currency address or a bank account
        /// All withdrawals need the withdrawType, wallet and amount parameters
        /// CryptoCurrency withdrawals need the address parameters, the paymentId can be used for Monero as payment id and for Ripple as tag
        /// Wire withdrawals need the bank parameters. In some cases your bank will require the use of an intermediary bank, if this is the case, please supply those fields as well.
        /// </summary>
        /// <param name="withdrawType">The type of funds to withdraw</param>
        /// <param name="wallet">The wallet to withdraw from</param>
        /// <param name="amount">The amount to withdraw</param>
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
            decimal amount,
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

        /// <summary>
        /// Claim a position
        /// </summary>
        /// <param name="id">The id of the position to claim</param>
        /// <param name="amount">The (partial) amount to be claimed</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexDepositAddress>> ClaimPositionAsync(long id, decimal amount, CancellationToken ct = default);

        /// <summary>
        /// Submit a new order
        /// </summary>
        /// <param name="currency">The currency</param>
        /// <param name="amount">The amount</param>
        /// <param name="rate">Rate to lend or borrow at in percent per 365 days (0 for FRR)</param>
        /// <param name="period">Number of days</param>
        /// <param name="direction">Direction of the offer</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOffer>> NewOfferAsync(string currency, decimal amount, decimal rate, int period, FundingType direction, CancellationToken ct = default);

        /// <summary>
        /// Cancel an offer
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOffer>> CancelOfferAsync(long offerId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an offer
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOffer>> GetOfferAsync(long offerId, CancellationToken ct = default);

        /// <summary>
        /// Close margin funding
        /// </summary>
        /// <param name="swapId">The id to close</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingContract>> CloseMarginFundingAsync(long swapId, CancellationToken ct = default);

        /// <summary>
        /// Close a position
        /// </summary>
        /// <param name="positionId">The id to close</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexClosePositionResult>> ClosePositionAsync(long positionId, CancellationToken ct = default);
    }
}