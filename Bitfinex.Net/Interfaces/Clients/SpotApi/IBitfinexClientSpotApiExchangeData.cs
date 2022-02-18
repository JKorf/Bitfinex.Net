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
    /// Bitfinex exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.
    /// </summary>
    public interface IBitfinexClientSpotApiExchangeData
    {
        /// <summary>
        /// Gets the platform status
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-platform-status" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Whether Bitfinex platform is running normally or not</returns>
        Task<WebCallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a list of supported assets
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-conf" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexAsset>>> GetAssetsAsync(CancellationToken ct = default);

        /// <summary>
        /// Returns basic market data for the provided symbols
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-ticker" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Symbol data</returns>
        Task<WebCallResult<BitfinexSymbolOverview>> GetTickerAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Returns basic market data for the provided symbols
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-tickers" /></para>
        /// </summary>
        /// <param name="symbols">The symbols to get data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Symbol data</returns>
        Task<WebCallResult<IEnumerable<BitfinexSymbolOverview>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Get recent trades for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-trades" /></para>
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
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-book" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="precision">The precision of the data</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The order book for the symbol</returns>
        Task<WebCallResult<BitfinexOrderBook>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the raw order book for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-book" /></para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexOrderBook>> GetRawOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get various stats for the symbol
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-stats1" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to request stats for</param>
        /// <param name="key">The type of stats</param>
        /// <param name="side">Side of the stats</param>
        /// <param name="section">Section of the stats</param>
        /// <param name="sorting">The way the result should be sorted</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexStats>>> GetStatsAsync(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Get the last kline for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-candles" /></para>
        /// </summary>
        /// <param name="interval">The time frame of the kline</param>
        /// <param name="symbol">The symbol to get the kline for</param>
        /// <param name="fundingPeriod">The Funding period. Only required for funding candles. Enter after the symbol (trade:1m:fUSD:p30/hist).</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The last kline for the symbol</returns>
        Task<WebCallResult<BitfinexKline>> GetLastKlineAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, CancellationToken ct = default);

        /// <summary>
        /// Gets klines for a symbol
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-candles" /></para>
        /// </summary>
        /// <param name="interval">The time frame of the klines</param>
        /// <param name="symbol">The symbol to get the klines for</param>
        /// <param name="fundingPeriod">The Funding period. Only required for funding candles. Enter after the symbol (trade:1m:fUSD:p30/hist).</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time of the klines</param>
        /// <param name="endTime">The end time of the klines</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexKline>>> GetKlinesAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Calculate the average execution price
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-calc-market-average-price" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to calculate for</param>
        /// <param name="quantity">The quantity to execute</param>
        /// <param name="rateLimit">Limit to price</param>
        /// <param name="period">Maximum period for margin funding</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The average price at which the execution would happen</returns>
        Task<WebCallResult<BitfinexAveragePrice>> GetAveragePriceAsync(string symbol, decimal quantity, decimal? rateLimit = null, int? period = null, CancellationToken ct = default);

        /// <summary>
        /// Returns the exchange rate for the assets
        /// <para><a href="https://docs.bitfinex.com/reference#rest-public-calc-foreign-exchange-rate" /></para>
        /// </summary>
        /// <param name="asset1">The first asset</param>
        /// <param name="asset2">The second asset</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange rate</returns>
        Task<WebCallResult<BitfinexForeignExchangeRate>> GetForeignExchangeRateAsync(string asset1, string asset2, CancellationToken ct = default);

        /// <summary>
        /// Gets the margin funding book
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-public-fundingbook" /></para>
        /// </summary>
        /// <param name="asset">Asset to get the book for</param>
        /// <param name="limit">Limit of the results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingBook>> GetFundingBookAsync(string asset, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the most recent lends
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-public-lends" /></para>
        /// </summary>
        /// <param name="asset">Asset to get the book for</param>
        /// <param name="startTime">Return data after this time</param>
        /// <param name="limit">Limit of the results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexLend>>> GetLendsAsync(string asset, DateTime? startTime = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of all symbols
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-public-symbols" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets details of all symbols
        /// <para><a href="https://docs.bitfinex.com/v1/reference#rest-public-symbol-details" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<BitfinexSymbolDetails>>> GetSymbolDetailsAsync(CancellationToken ct = default);
    }
}
