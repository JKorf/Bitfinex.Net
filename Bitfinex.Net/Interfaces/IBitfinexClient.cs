using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.RestV1Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace Bitfinex.Net.Interfaces
{
    public interface IBitfinexClient: IRestClient
    {
        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        void SetApiCredentials(string apiKey, string apiSecret);

        /// <summary>
        /// Gets the platform status
        /// </summary>
        /// <returns>Whether Bitfinex platform is running normally or not</returns>
        WebCallResult<BitfinexPlatformStatus> GetPlatformStatus();

        /// <summary>
        /// Gets the platform status
        /// </summary>
        /// <returns>Whether Bitfinex platform is running normally or not</returns>
        Task<WebCallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync();

        /// <summary>
        /// Gets a list of supported currencies
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexCurrency[]> GetCurrencies();

        /// <summary>
        /// Gets a list of supported currencies
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexCurrency[]>> GetCurrenciesAsync(params string[] symbols);

        /// <summary>
        /// Returns basic market data for the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols to get data for</param>
        /// <returns>Market data</returns>
        WebCallResult<BitfinexMarketOverviewRest[]> GetTicker(params string[] symbols);

        /// <summary>
        /// Returns basic market data for the provided symbols
        /// </summary>
        /// <param name="symbols">The symbols to get data for</param>
        /// <returns>Market data</returns>
        Task<WebCallResult<BitfinexMarketOverviewRest[]>> GetTickerAsync(params string[] symbols);

        /// <summary>
        /// Get recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get trades for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time to return trades for</param>
        /// <param name="endTime">The end time to return trades for</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <returns>Trades for the symbol</returns>
        WebCallResult<BitfinexTradeSimple[]> GetTrades(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null);

        /// <summary>
        /// Get recent trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get trades for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time to return trades for</param>
        /// <param name="endTime">The end time to return trades for</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <returns>Trades for the symbol</returns>
        Task<WebCallResult<BitfinexTradeSimple[]>> GetTradesAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null);

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="precision">The precision of the data</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <returns>The order book for the symbol</returns>
        WebCallResult<BitfinexOrderBookEntry[]> GetOrderBook(string symbol, Precision precision, int? limit = null);

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="precision">The precision of the data</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <returns>The order book for the symbol</returns>
        Task<WebCallResult<BitfinexOrderBookEntry[]>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null);

        /// <summary>
        /// Get various stats for the symbol
        /// </summary>
        /// <param name="symbol">The symbol to request stats for</param>
        /// <param name="key">The type of stats</param>
        /// <param name="side">Side of the stats</param>
        /// <param name="section">Section of the stats</param>
        /// <param name="sorting">The way the result should be sorted</param>
        /// <returns></returns>
        WebCallResult<BitfinexStats> GetStats(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting = null);

        /// <summary>
        /// Get various stats for the symbol
        /// </summary>
        /// <param name="symbol">The symbol to request stats for</param>
        /// <param name="key">The type of stats</param>
        /// <param name="side">Side of the stats</param>
        /// <param name="section">Section of the stats</param>
        /// <param name="sorting">The way the result should be sorted</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats>> GetStatsAsync(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting);

        /// <summary>
        /// Get the last candle for a symbol
        /// </summary>
        /// <param name="timeFrame">The time frame of the candle</param>
        /// <param name="symbol">The symbol to get the candle for</param>
        /// <returns>The last candle for the symbol</returns>
        WebCallResult<BitfinexCandle> GetLastCandle(TimeFrame timeFrame, string symbol);

        /// <summary>
        /// Get the last candle for a symbol
        /// </summary>
        /// <param name="timeFrame">The time frame of the candle</param>
        /// <param name="symbol">The symbol to get the candle for</param>
        /// <returns>The last candle for the symbol</returns>
        Task<WebCallResult<BitfinexCandle>> GetLastCandleAsync(TimeFrame timeFrame, string symbol);

        /// <summary>
        /// Gets candles for a symbol
        /// </summary>
        /// <param name="timeFrame">The time frame of the candles</param>
        /// <param name="symbol">The symbol to get the candles for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time of the candles</param>
        /// <param name="endTime">The end time of the candles</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <returns></returns>
        WebCallResult<BitfinexCandle[]> GetCandles(TimeFrame timeFrame, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null);

        /// <summary>
        /// Gets candles for a symbol
        /// </summary>
        /// <param name="timeFrame">The time frame of the candles</param>
        /// <param name="symbol">The symbol to get the candles for</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time of the candles</param>
        /// <param name="endTime">The end time of the candles</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexCandle[]>> GetCandlesAsync(TimeFrame timeFrame, string symbol,int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null);

        /// <summary>
        /// Calculate the average execution price
        /// </summary>
        /// <param name="symbol">The symbol to calculate for</param>
        /// <param name="amount">The amount to execute</param>
        /// <param name="rateLimit">Limit to price</param>
        /// <param name="period">Maximum period for margin funding</param>
        /// <returns>The average price at which the execution would happen</returns>
        WebCallResult<BitfinexMarketAveragePrice> GetMarketAveragePrice(string symbol, decimal amount, decimal rateLimit, int? period = null);

        /// <summary>
        /// Calculate the average execution price
        /// </summary>
        /// <param name="symbol">The symbol to calculate for</param>
        /// <param name="amount">The amount to execute</param>
        /// <param name="rateLimit">Limit to price</param>
        /// <param name="period">Maximum period for margin funding</param>
        /// <returns>The average price at which the execution would happen</returns>
        Task<WebCallResult<BitfinexMarketAveragePrice>> GetMarketAveragePriceAsync(string symbol, decimal amount, decimal? rateLimit = null, int? period = null);

        /// <summary>
        /// Returns the exchange rate for the currencies
        /// </summary>
        /// <param name="currency1">The first currency</param>
        /// <param name="currency2">The second currency</param>
        WebCallResult<BitfinexForeignExchangeRate> GetForeignExchangeRate(string currency1, string currency2);

        /// <summary>
        /// Returns the exchange rate for the currencies
        /// </summary>
        /// <param name="currency1">The first currency</param>
        /// <param name="currency2">The second currency</param>
        Task<WebCallResult<BitfinexForeignExchangeRate>> GetForeignExchangeRateAsync(string currency1, string currency2);

        /// <summary>
        /// Get all funds
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexWallet[]> GetWallets();

        /// <summary>
        /// Get all funds
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWallet[]>> GetWalletsAsync();

        /// <summary>
        /// Get the active orders
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexOrder[]> GetActiveOrders();

        /// <summary>
        /// Get the active orders
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOrder[]>> GetActiveOrdersAsync();

        /// <summary>
        /// Get the order history for a symbol for this account
        /// </summary>
        /// <param name="symbol">The symbol to get the history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        WebCallResult<BitfinexOrder[]> GetOrderHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the order history for a symbol for this account
        /// </summary>
        /// <param name="symbol">The symbol to get the history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOrder[]>> GetOrderHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the individual trades for an order
        /// </summary>
        /// <param name="symbol">The symbol of the order</param>
        /// <param name="orderId">The order Id</param>
        /// <returns></returns>
        WebCallResult<BitfinexTradeDetails[]> GetTradesForOrder(string symbol, long orderId);

        /// <summary>
        /// Get the individual trades for an order
        /// </summary>
        /// <param name="symbol">The symbol of the order</param>
        /// <param name="orderId">The order Id</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexTradeDetails[]>> GetTradesForOrderAsync(string symbol, long orderId);

        /// <summary>
        /// Get the trade history for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        WebCallResult<BitfinexTradeDetails[]> GetTradeHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the trade history for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexTradeDetails[]>> GetTradeHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the active positions
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexPosition[]> GetActivePositions();

        /// <summary>
        /// Get the active positions
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPosition[]>> GetActivePositionsAsync();

        /// <summary>
        /// Get a list of historical positions
        /// </summary>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        WebCallResult<BitfinexPositionExtended[]> GetPositionHistory(DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get a list of historical positions
        /// </summary>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPositionExtended[]>> GetPositionHistoryAsync(DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get positions by id
        /// </summary>
        /// <param name="ids">The id's of positions to return</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        WebCallResult<BitfinexPositionExtended[]> GetPositionsById(string[] ids, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get positions by id
        /// </summary>
        /// <param name="ids">The id's of positions to return</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPositionExtended[]>> GetPositionsByIdAsync(string[] ids, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the active funding offers
        /// </summary>
        /// <param name="symbol">The symbol to return the funding offer for</param>
        /// <returns></returns>
        WebCallResult<BitfinexFundingOffer[]> GetActiveFundingOffers(string symbol);

        /// <summary>
        /// Get the active funding offers
        /// </summary>
        /// <param name="symbol">The symbol to return the funding offer for</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingOffer[]>> GetActiveFundingOffersAsync(string symbol);

        /// <summary>
        /// Get the funding offer history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        WebCallResult<BitfinexFundingOffer[]> GetFundingOfferHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding offer history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingOffer[]>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding loans
        /// </summary>
        /// <param name="symbol">The symbol to get the funding loans for</param>
        /// <returns></returns>
        WebCallResult<BitfinexFunding[]> GetFundingLoans(string symbol);

        /// <summary>
        /// Get the funding loans
        /// </summary>
        /// <param name="symbol">The symbol to get the funding loans for</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFunding[]>> GetFundingLoansAsync(string symbol);

        /// <summary>
        /// Get the funding loan history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        WebCallResult<BitfinexFunding[]> GetFundingLoansHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding loan history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFunding[]>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding credits
        /// </summary>
        /// <param name="symbol">The symbol to get the funding credits for</param>
        /// <returns></returns>
        WebCallResult<BitfinexFundingCredit[]> GetFundingCredits(string symbol);

        /// <summary>
        /// Get the funding credits
        /// </summary>
        /// <param name="symbol">The symbol to get the funding credits for</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingCredit[]>> GetFundingCreditsAsync(string symbol);

        /// <summary>
        /// Get the funding credits history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        WebCallResult<BitfinexFundingCredit[]> GetFundingCreditsHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding credits history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingCredit[]>> GetFundingCreditsHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding trades history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        WebCallResult<BitfinexFundingTrade[]> GetFundingTradesHistory(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the funding trades history
        /// </summary>
        /// <param name="symbol">The symbol to get history for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingTrade[]>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get the base margin info
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexMarginBase> GetBaseMarginInfo();

        /// <summary>
        /// Get the base margin info
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync();

        /// <summary>
        /// Get the margin info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <returns></returns>
        WebCallResult<BitfinexMarginSymbol> GetSymbolMarginInfo(string symbol);

        /// <summary>
        /// Get the margin info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol);

        /// <summary>
        /// Get funding info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <returns></returns>
        WebCallResult<BitfinexFundingInfo> GetFundingInfo(string symbol);

        /// <summary>
        /// Get funding info for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to get the info for</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol);

        /// <summary>
        /// Get the withdrawal/deposit history
        /// </summary>
        /// <param name="symbol">Symbol to get history for</param>
        /// <returns></returns>
        WebCallResult<BitfinexMovement[]> GetMovements(string symbol);

        /// <summary>
        /// Get the withdrawal/deposit history
        /// </summary>
        /// <param name="symbol">Symbol to get history for</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMovement[]>> GetMovementsAsync(string symbol);

        WebCallResult<BitfinexPerformance> GetDailyPerformance();
        Task<WebCallResult<BitfinexPerformance>> GetDailyPerformanceAsync();

        /// <summary>
        /// Get the list of alerts
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexAlert[]> GetAlertList();

        /// <summary>
        /// Get the list of alerts
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAlert[]>> GetAlertListAsync();

        /// <summary>
        /// Set an alert
        /// </summary>
        /// <param name="symbol">The symbol to set the alert for</param>
        /// <param name="price">The price to set the alert for</param>
        /// <returns></returns>
        WebCallResult<BitfinexAlert> SetAlert(string symbol, decimal price);

        /// <summary>
        /// Set an alert
        /// </summary>
        /// <param name="symbol">The symbol to set the alert for</param>
        /// <param name="price">The price to set the alert for</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price);

        /// <summary>
        /// Delete an existing alert
        /// </summary>
        /// <param name="symbol">The symbol of the alert to delete</param>
        /// <param name="price">The price of the alert to delete</param>
        /// <returns></returns>
        WebCallResult<BitfinexSuccessResult> DeleteAlert(string symbol, decimal price);

        /// <summary>
        /// Delete an existing alert
        /// </summary>
        /// <param name="symbol">The symbol of the alert to delete</param>
        /// <param name="price">The price of the alert to delete</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price);

        /// <summary>
        /// Calculates the available balance for a symbol at a specific rate
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="side">Buy or sell</param>
        /// <param name="rate">The rate/price</param>
        /// <param name="type">The wallet type</param>
        /// <returns></returns>
        WebCallResult<BitfinexAvailableBalance> GetAvailableBalance(string symbol, OrderSide side, decimal rate, WalletType type);

        /// <summary>
        /// Calculates the available balance for a symbol at a specific rate
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="side">Buy or sell</param>
        /// <param name="rate">The rate/price</param>
        /// <param name="type">The wallet type</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal rate, WalletType type);

        /// <summary>
        /// Get changes in your balance for a currency
        /// </summary>
        /// <param name="currency">The currency to check the ledger for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        WebCallResult<BitfinexLedgerEntry[]> GetLedgerEntries(string currency, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Get changes in your balance for a currency
        /// </summary>
        /// <param name="currency">The currency to check the ledger for</param>
        /// <param name="startTime">Start time of the data to return</param>
        /// <param name="endTime">End time of the data to return</param>
        /// <param name="limit">Max amount of results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexLedgerEntry[]>> GetLedgerEntriesAsync(string currency, DateTime? startTime = null, DateTime? endTime = null, int? limit = null);

        /// <summary>
        /// Gets information about the user associated with the api key/secret
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexUserInfo> GetUserInfo();

        /// <summary>
        /// Gets information about the user associated with the api key/secret
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexUserInfo>> GetUserInfoAsync();

        /// <summary>
        /// Gets the margin funding book
        /// </summary>
        /// <param name="currency">Currency to get the book for</param>
        /// <param name="limit">Limit of the results</param>
        /// <returns></returns>
        WebCallResult<BitfinexFundingBook> GetFundingBook(string currency, int? limit = null);

        /// <summary>
        /// Gets the margin funding book
        /// </summary>
        /// <param name="currency">Currency to get the book for</param>
        /// <param name="limit">Limit of the results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingBook>> GetFundingBookAsync(string currency, int? limit = null);

        /// <summary>
        /// Gets the most recent lends
        /// </summary>
        /// <param name="currency">Currency to get the book for</param>
        /// <param name="startTime">Return data after this time</param>
        /// <param name="limit">Limit of the results</param>
        /// <returns></returns>
        WebCallResult<BitfinexLend[]> GetLends(string currency, DateTime? startTime = null, int? limit = null);

        /// <summary>
        /// Gets the most recent lends
        /// </summary>
        /// <param name="currency">Currency to get the book for</param>
        /// <param name="startTime">Return data after this time</param>
        /// <param name="limit">Limit of the results</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexLend[]>> GetLendsAsync(string currency, DateTime? startTime = null, int ? limit = null);

        /// <summary>
        /// Gets a list of all symbols
        /// </summary>
        /// <returns></returns>
        WebCallResult<string[]> GetSymbols();

        /// <summary>
        /// Gets a list of all symbols
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<string[]>> GetSymbolsAsync();

        /// <summary>
        /// Gets details of all symbols
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexSymbolDetails[]> GetSymbolDetails();

        /// <summary>
        /// Gets details of all symbols
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexSymbolDetails[]>> GetSymbolDetailsAsync();

        /// <summary>
        /// Get information about your account
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexAccountInfo> GetAccountInfo();

        /// <summary>
        /// Get information about your account
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAccountInfo>> GetAccountInfoAsync();

        /// <summary>
        /// Get withdrawal fees for this account
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexWithdrawalFees> GetWithdrawalFees();

        /// <summary>
        /// Get withdrawal fees for this account
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexWithdrawalFees>> GetWithdrawalFeesAsync();

        /// <summary>
        /// Get 30-day summary on trading volume and margin funding
        /// </summary>
        /// <returns></returns>
        WebCallResult<Bitfinex30DaySummary> Get30DaySummary();

        /// <summary>
        /// Get 30-day summary on trading volume and margin funding
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<Bitfinex30DaySummary>> Get30DaySummaryAsync();

        /// <summary>
        /// Place a new order
        /// </summary>
        /// <param name="symbol">Symbol to place order for</param>
        /// <param name="side">Side of the order</param>
        /// <param name="type">Type of the order</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price for the order</param>
        /// <param name="hidden">If the order should be placed as hidden</param>
        /// <param name="postOnly">If the only should only be placed if it isn't immediately filled</param>
        /// <param name="useAllAvailable">If all available funds should be used</param>
        /// <param name="stopLimitPrice">The stop price if a stop limit order is placed</param>
        /// <param name="ocoOrder">If the order is a one-cancels-other order</param>
        /// <param name="ocoBuyPrice">The one-cancels-other buy price</param>
        /// <param name="ocoSellPrice">The one-cancels-other sell price</param>
        /// <returns></returns>
        WebCallResult<BitfinexPlacedOrder> PlaceOrder(
            string symbol,
            OrderSide side,
            OrderTypeV1 type,
            decimal amount, 
            decimal price, 
            bool? hidden = null, 
            bool? postOnly = null, 
            bool? useAllAvailable = null,
            decimal? stopLimitPrice = null,
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
        /// <param name="postOnly">If the only should only be placed if it isn't immediately filled</param>
        /// <param name="useAllAvailable">If all available funds should be used</param>
        /// <param name="stopLimitPrice">The stop price if a stop limit order is placed</param>
        /// <param name="ocoOrder">If the order is a one-cancels-other order</param>
        /// <param name="ocoBuyPrice">The one-cancels-other buy price</param>
        /// <param name="ocoSellPrice">The one-cancels-other sell price</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPlacedOrder>> PlaceOrderAsync(
            string symbol, 
            OrderSide side, 
            OrderTypeV1 type, 
            decimal amount, 
            decimal price, 
            bool? hidden = null,
            bool? postOnly = null, 
            bool? useAllAvailable = null,
            decimal? stopLimitPrice = null,
            bool? ocoOrder = null, 
            decimal? ocoBuyPrice = null,
            decimal? ocoSellPrice = null);

        /// <summary>
        /// Cancel a specific order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        WebCallResult<BitfinexPlacedOrder> CancelOrder(long orderId);

        /// <summary>
        /// Cancel a specific order
        /// </summary>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPlacedOrder>> CancelOrderAsync(long orderId);

        /// <summary>
        /// Cancels all open orders
        /// </summary>
        /// <returns></returns>
        WebCallResult<BitfinexResult> CancelAllOrders();

        /// <summary>
        /// Cancels all open orders
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<BitfinexResult>> CancelAllOrdersAsync();

        /// <summary>
        /// Get the status of a specific order
        /// </summary>
        /// <param name="orderId">The order id of the order to get</param>
        /// <returns></returns>
        WebCallResult<BitfinexPlacedOrder> GetOrder(long orderId);

        /// <summary>
        /// Get the status of a specific order
        /// </summary>
        /// <param name="orderId">The order id of the order to get</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexPlacedOrder>> GetOrderAsync(long orderId);

        /// <summary>
        /// Gets a deposit address for a currency
        /// </summary>
        /// <param name="currency">The currency to get address for</param>
        /// <param name="toWallet">The type of wallet the deposit is for</param>
        /// <param name="forceNew">If true a new address will be generated (previous addresses will still be valid)</param>
        /// <returns></returns>
        WebCallResult<BitfinexDepositAddress> GetDepositAddress(string currency, WithdrawWallet toWallet, bool? forceNew = null);

        /// <summary>
        /// Gets a deposit address for a currency
        /// </summary>
        /// <param name="currency">The currency to get address for</param>
        /// <param name="toWallet">The type of wallet the deposit is for</param>
        /// <param name="forceNew">If true a new address will be generated (previous addresses will still be valid)</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexDepositAddress>> GetDepositAddressAsync(string currency, WithdrawWallet toWallet, bool? forceNew = null);

        /// <summary>
        /// Transfers funds from one wallet to another
        /// </summary>
        /// <param name="currency">The currency to transfer</param>
        /// <param name="fromWallet">The wallet to remove funds from</param>
        /// <param name="toWallet">The wallet to add funds to</param>
        /// <param name="amount">The amount to transfer</param>
        /// <returns></returns>
        WebCallResult<BitfinexTransferResult> WalletTransfer(string currency, decimal amount, WithdrawWallet fromWallet, WithdrawWallet toWallet);

        /// <summary>
        /// Transfers funds from one wallet to another
        /// </summary>
        /// <param name="currency">The currency to transfer</param>
        /// <param name="fromWallet">The wallet to remove funds from</param>
        /// <param name="toWallet">The wallet to add funds to</param>
        /// <param name="amount">The amount to transfer</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexTransferResult>> WalletTransferAsync(string currency, decimal amount, WithdrawWallet fromWallet, WithdrawWallet toWallet);

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
        /// <returns></returns>
        WebCallResult<BitfinexWithdrawalResult> Withdraw(string withdrawType,
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
        /// <returns></returns>
        Task<WebCallResult<BitfinexWithdrawalResult>> WithdrawAsync(string withdrawType, 
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
        /// Claim a position
        /// </summary>
        /// <param name="id">The id of the position to claim</param>
        /// <param name="amount">The (partial) amount to be claimed</param>
        /// <returns></returns>
        WebCallResult<BitfinexDepositAddress> ClaimPosition(long id, decimal amount);

        /// <summary>
        /// Claim a position
        /// </summary>
        /// <param name="id">The id of the position to claim</param>
        /// <param name="amount">The (partial) amount to be claimed</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexDepositAddress>> ClaimPositionAsync(long id, decimal amount);

        /// <summary>
        /// Submit a new order
        /// </summary>
        /// <param name="currency">The currency</param>
        /// <param name="amount">The amount</param>
        /// <param name="rate">Rate to lend or borrow at in percent per 365 days (0 for FRR)</param>
        /// <param name="period">Number of days</param>
        /// <param name="direction">Direction of the offer</param>
        /// <returns></returns>
        WebCallResult<BitfinexOffer> NewOffer(string currency, decimal amount, decimal rate, int period, FundingType direction);

        /// <summary>
        /// Submit a new order
        /// </summary>
        /// <param name="currency">The currency</param>
        /// <param name="amount">The amount</param>
        /// <param name="rate">Rate to lend or borrow at in percent per 365 days (0 for FRR)</param>
        /// <param name="period">Number of days</param>
        /// <param name="direction">Direction of the offer</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOffer>> NewOfferAsync(string currency, decimal amount, decimal rate, int period, FundingType direction);

        /// <summary>
        /// Cancel an offer
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <returns></returns>
        WebCallResult<BitfinexOffer> CancelOffer(long offerId);

        /// <summary>
        /// Cancel an offer
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOffer>> CancelOfferAsync(long offerId);

        /// <summary>
        /// Cancel an offer
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <returns></returns>
        WebCallResult<BitfinexOffer> GetOffer(long offerId);

        /// <summary>
        /// Cancel an offer
        /// </summary>
        /// <param name="offerId">The id of the offer to cancel</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOffer>> GetOfferAsync(long offerId);

        /// <summary>
        /// Close margin funding
        /// </summary>
        /// <param name="swapId">The id to close</param>
        /// <returns></returns>
        WebCallResult<BitfinexFundingContract> CloseMarginFunding(long swapId);

        /// <summary>
        /// Close margin funding
        /// </summary>
        /// <param name="swapId">The id to close</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingContract>> CloseMarginFundingAsync(long swapId);

        /// <summary>
        /// Close a position
        /// </summary>
        /// <param name="positionId">The id to close</param>
        /// <returns></returns>
        WebCallResult<BitfinexClosePositionResult> ClosePosition(long positionId);

        /// <summary>
        /// Close a position
        /// </summary>
        /// <param name="positionId">The id to close</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexClosePositionResult>> ClosePositionAsync(long positionId);
    }
}