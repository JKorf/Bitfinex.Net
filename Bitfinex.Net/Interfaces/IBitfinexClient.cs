using System;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.RestV1Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiter;

namespace Bitfinex.Net.Interfaces
{
    public interface IBitfinexClient
    {
        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        void SetApiCredentials(string apiKey, string apiSecret);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetPlatformStatusAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexPlatformStatus> GetPlatformStatus();

        /// <summary>
        /// Gets the platform status
        /// </summary>
        /// <returns>Whether Bitfinex platform is running normally or not</returns>
        Task<CallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetSymbolsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<string[]> GetSymbols();

        /// <summary>
        /// Gets a list of all symbols
        /// </summary>
        /// <returns></returns>
        Task<CallResult<string[]>> GetSymbolsAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetSymbolDetailsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexSymbolDetails[]> GetSymbolDetails();

        /// <summary>
        /// Gets details of all symbols
        /// </summary>
        /// <returns></returns>
        Task<CallResult<BitfinexSymbolDetails[]>> GetSymbolDetailsAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetTickerAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexMarketOverviewRest[]> GetTicker(params string[] symbols);

        /// <summary>
        /// Returns basic market data for the provided smbols
        /// </summary>
        /// <param name="symbols">The symbols to get data for</param>
        /// <returns>Market data</returns>
        Task<CallResult<BitfinexMarketOverviewRest[]>> GetTickerAsync(params string[] symbols);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetTradesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexTradeSimple[]> GetTrades(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null);

        /// <summary>
        /// Get recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get trades for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The starttime to return trades for</param>
        /// <param name="endTime">The endtime to return trades for</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <returns>Trades for the symbol</returns>
        Task<CallResult<BitfinexTradeSimple[]>> GetTradesAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetOrderBookAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexOrderBookEntry[]> GetOrderBook(string symbol, Precision precision, int? limit = null);

        /// <summary>
        /// Gets the orderbook for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="precision">The precision of the data</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <returns>The orderbook for the symbol</returns>
        Task<CallResult<BitfinexOrderBookEntry[]>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetStatsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexStats> GetStats(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting = null);

        /// <summary>
        /// Get various stats for the symbol
        /// </summary>
        /// <param name="symbol">The symbol to request stats for</param>
        /// <param name="key">The type of stats</param>
        /// <param name="side">Side of the stats</param>
        /// <param name="section">Section of the stats</param>
        /// <param name="sorting">The way the result should be sorted</param>
        /// <returns></returns>
        Task<CallResult<BitfinexStats>> GetStatsAsync(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetLastCandleAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexCandle> GetLastCandle(TimeFrame timeFrame, string symbol);

        /// <summary>
        /// Get the last candle for a symbol
        /// </summary>
        /// <param name="timeFrame">The timeframe of the candle</param>
        /// <param name="symbol">The symbol to get the candle for</param>
        /// <returns>The last candle for the symbol</returns>
        Task<CallResult<BitfinexCandle>> GetLastCandleAsync(TimeFrame timeFrame, string symbol);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetCandlesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexCandle[]> GetCandles(TimeFrame timeFrame, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null);

        /// <summary>
        /// Gets candles for a symbol
        /// </summary>
        /// <param name="timeFrame">The timeframe of the candles</param>
        /// <param name="symbol">The symbol to get the candles for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time of the candles</param>
        /// <param name="endTime">The end time of the candles</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <returns></returns>
        Task<CallResult<BitfinexCandle[]>> GetCandlesAsync(TimeFrame timeFrame, string symbol,int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetMarketAveragePriceAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexMarketAveragePrice> GetMarketAveragePrice(string symbol, decimal amount, decimal rateLimit, int? period = null);

        /// <summary>
        /// Calculate the average execution price
        /// </summary>
        /// <param name="symbol">The symbol to calculate for</param>
        /// <param name="amount">The amount to execute</param>
        /// <param name="rateLimit">Limit to price</param>
        /// <param name="period">Maximum period for margin funding</param>
        /// <returns>The average price at which the execution would happen</returns>
        Task<CallResult<BitfinexMarketAveragePrice>> GetMarketAveragePriceAsync(string symbol, decimal amount, decimal? rateLimit = null, int? period = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetWalletsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexWallet[]> GetWallets();

        /// <summary>
        /// Get all funds
        /// </summary>
        /// <returns></returns>
        Task<CallResult<BitfinexWallet[]>> GetWalletsAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetActiveOrdersAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexOrder[]> GetActiveOrders();

        /// <summary>
        /// Get the active orders
        /// </summary>
        /// <returns></returns>
        Task<CallResult<BitfinexOrder[]>> GetActiveOrdersAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetOrderHistoryAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexOrder[]> GetOrderHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the order history for a symbol for this account
        /// </summary>
        /// <param name="symbol">The symbol to get the history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<CallResult<BitfinexOrder[]>> GetOrderHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetTradesForOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexTradeDetails[]> GetTradesForOrder(string symbol, long orderId);

        /// <summary>
        /// Get the individual trades for an order
        /// </summary>
        /// <param name="symbol">The symbol of the order</param>
        /// <param name="orderId">The order Id</param>
        /// <returns></returns>
        Task<CallResult<BitfinexTradeDetails[]>> GetTradesForOrderAsync(string symbol, long orderId);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetTradeHistoryAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexTradeDetails[]> GetTradeHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the trade history for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<CallResult<BitfinexTradeDetails[]>> GetTradeHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetActivePositionsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexPosition[]> GetActivePositions();

        /// <summary>
        /// Get the active positions
        /// </summary>
        /// <returns></returns>
        Task<CallResult<BitfinexPosition[]>> GetActivePositionsAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetActiveFundingOffersAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexFundingOffer[]> GetActiveFundingOffers(string symbol);

        /// <summary>
        /// Get the active funding offers
        /// </summary>
        /// <param name="symbol">The symbol to return the funding offer for</param>
        /// <returns></returns>
        Task<CallResult<BitfinexFundingOffer[]>> GetActiveFundingOffersAsync(string symbol);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetFundingOfferHistoryAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexFundingOffer[]> GetFundingOfferHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding offer history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<CallResult<BitfinexFundingOffer[]>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetFundingLoansAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexFundingLoan[]> GetFundingLoans(string symbol);

        /// <summary>
        /// Get the funding loans
        /// </summary>
        /// <param name="symbol">The symbol to get the funding loans for</param>
        /// <returns></returns>
        Task<CallResult<BitfinexFundingLoan[]>> GetFundingLoansAsync(string symbol);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetFundingLoansHistoryAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexFundingLoan[]> GetFundingLoansHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding loan history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<CallResult<BitfinexFundingLoan[]>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetFundingCreditsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexFundingCredit[]> GetFundingCredits(string symbol);

        /// <summary>
        /// Get the funding credits
        /// </summary>
        /// <param name="symbol">The symbol to get the funding credits for</param>
        /// <returns></returns>
        Task<CallResult<BitfinexFundingCredit[]>> GetFundingCreditsAsync(string symbol);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetFundingCreditsHistoryAsyncTask"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexFundingCredit[]> GetFundingCreditsHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding credits history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<CallResult<BitfinexFundingCredit[]>> GetFundingCreditsHistoryAsyncTask(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetFundingTradesHistoryAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexFundingTrade[]> GetFundingTradesHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding trades history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<CallResult<BitfinexFundingTrade[]>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetBaseMarginInfoAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexMarginBase> GetBaseMarginInfo();

        /// <summary>
        /// Get the base margin info
        /// </summary>
        /// <returns></returns>
        Task<CallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetSymbolMarginInfoAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexMarginSymbol> GetSymbolMarginInfo(string symbol);

        /// <summary>
        /// Get the margin info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <returns></returns>
        Task<CallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetFundingInfoAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexFundingInfo> GetFundingInfo(string symbol);

        /// <summary>
        /// Get funding info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <returns></returns>
        Task<CallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetMovementsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexMovement[]> GetMovements(string symbol);

        /// <summary>
        /// Get the withdrawal/deposit history
        /// </summary>
        /// <param name="symbol">Symbol to get history for</param>
        /// <returns></returns>
        Task<CallResult<BitfinexMovement[]>> GetMovementsAsync(string symbol);

        CallResult<BitfinexPerformance> GetDailyPerformance();
        Task<CallResult<BitfinexPerformance>> GetDailyPerformanceAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetAlertListAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexAlert[]> GetAlertList();

        /// <summary>
        /// Get the list of alerts
        /// </summary>
        /// <returns></returns>
        Task<CallResult<BitfinexAlert[]>> GetAlertListAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.SetAlertAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexAlert> SetAlert(string symbol, decimal price);

        /// <summary>
        /// Set an alert
        /// </summary>
        /// <param name="symbol">The symbol to set the alert for</param>
        /// <param name="price">The price to set the alert for</param>
        /// <returns></returns>
        Task<CallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.DeleteAlertAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexSuccessResult> DeleteAlert(string symbol, decimal price);

        /// <summary>
        /// Delete an existing alert
        /// </summary>
        /// <param name="symbol">The symbol of the alert to delete</param>
        /// <param name="price">The price of the alert to delete</param>
        /// <returns></returns>
        Task<CallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetAccountInfoAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexAccountInfo> GetAccountInfo();

        /// <summary>
        /// Get information about your account
        /// </summary>
        /// <returns></returns>
        Task<CallResult<BitfinexAccountInfo>> GetAccountInfoAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetWithdrawalFeesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexWithdrawalFees> GetWithdrawalFees();

        /// <summary>
        /// Get withdrawal fees for this account
        /// </summary>
        /// <returns></returns>
        Task<CallResult<BitfinexWithdrawalFees>> GetWithdrawalFeesAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.PlaceOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexPlacedOrder> PlaceOrder(
            string symbol,
            OrderSide side,
            OrderTypeV1 type,
            decimal amount, 
            decimal price, 
            bool? hidden = null, 
            bool? postOnly = null, 
            bool? useAllAvailable = null,
            bool? ocoOrder = null, 
            decimal? ocoBuyPrice = null, 
            decimal? ocoSellPrice = null);

        /// <summary>
        /// Place a new order
        /// </summary>
        /// <param name="symbol">Symbol to place order for</param>
        /// <param name="side">Side of the order</param>
        /// <param name="type">Type of the order</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price for the order</param>
        /// <param name="hidden">If the order should be placed as hidden</param>
        /// <param name="postOnly">If the only should only be placed if it isn't immediatly filled</param>
        /// <param name="useAllAvailable">If all available funds should be used</param>
        /// <param name="ocoOrder">If the order is a one-cancels-other order</param>
        /// <param name="ocoBuyPrice">The one-cancels-other buy price</param>
        /// <param name="ocoSellPrice">The one-cancels-other sell price</param>
        /// <returns></returns>
        Task<CallResult<BitfinexPlacedOrder>> PlaceOrderAsync(
            string symbol, 
            OrderSide side, 
            OrderTypeV1 type, 
            decimal amount, 
            decimal price, 
            bool? hidden = null,
            bool? postOnly = null, 
            bool? useAllAvailable = null,
            bool? ocoOrder = null, 
            decimal? ocoBuyPrice = null,
            decimal? ocoSellPrice = null);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.CancelOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexPlacedOrder> CancelOrder(long orderId);

        /// <summary>
        /// Cancel a specific order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        Task<CallResult<BitfinexPlacedOrder>> CancelOrderAsync(long orderId);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.CancelAllOrdersAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexResult> CancelAllOrders();

        /// <summary>
        /// Cancels all open orders
        /// </summary>
        /// <returns></returns>
        Task<CallResult<BitfinexResult>> CancelAllOrdersAsync();

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetOrderAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexPlacedOrder> GetOrder(long orderId);

        /// <summary>
        /// Get the status of a specific order
        /// </summary>
        /// <param name="orderId">The order id of the order to get</param>
        /// <returns></returns>
        Task<CallResult<BitfinexPlacedOrder>> GetOrderAsync(long orderId);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.GetAvailableBalanceAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexAvailableBalance> GetAvailableBalance(string symbol, OrderSide side, decimal rate, WalletType type);

        /// <summary>
        /// Calculates the available balance for a symbol at a specific rate
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="side">Buy or sell</param>
        /// <param name="rate">The rate/price</param>
        /// <param name="type">The wallet type</param>
        /// <returns></returns>
        Task<CallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal rate, WalletType type);

        /// <summary>
        /// Synchronized version of the <see cref="BitfinexClient.WithdrawAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<BitfinexWithdrawalResult> Withdraw(WithdrawalType withdrawType,
            WithdrawWallet wallet,
            decimal amount,
            string address = null,
            string accountNumber = null,
            string bankSwift = null,
            string bankName = null,
            string bankAddress = null,
            string bankCity = null,
            string bankCountry = null,
            string paymentDetails = null,
            bool? expressWire = null,
            string intermediaryBankName = null,
            string intermediaryBankAddress = null,
            string intermediaryBankCity = null,
            string intermediaryBankCountry = null,
            string intermediaryBankAccount = null,
            string intermediaryBankSwift = null,
            string accountName = null,
            string paymentId = null);

        /// <summary>
        /// Withdraw funds from Bitfinex, either to a cryptocurrency address or a bank account
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
        /// <returns></returns>
        Task<CallResult<BitfinexWithdrawalResult>> WithdrawAsync(WithdrawalType withdrawType, 
            WithdrawWallet wallet, 
            decimal amount, 
            string address = null, 
            string accountNumber = null,
            string bankSwift = null, 
            string bankName = null,
            string bankAddress = null,
            string bankCity = null,
            string bankCountry = null,
            string paymentDetails = null,
            bool? expressWire = null,
            string intermediaryBankName = null,
            string intermediaryBankAddress = null,
            string intermediaryBankCity = null,
            string intermediaryBankCountry = null,
            string intermediaryBankAccount = null,
            string intermediaryBankSwift = null,
            string accountName = null, 
            string paymentId = null);

        void AddRateLimiter(IRateLimiter limiter);
        void RemoveRateLimiters();
        CallResult<long> Ping();
        void Dispose();
        Task<CallResult<long>> PingAsync();
        IRequestFactory RequestFactory { get; set; }
    }
}