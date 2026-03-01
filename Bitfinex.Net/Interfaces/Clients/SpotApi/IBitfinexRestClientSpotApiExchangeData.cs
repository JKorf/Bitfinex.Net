using Bitfinex.Net.Enums;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects.Models;

namespace Bitfinex.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Bitfinex exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.
    /// </summary>
    public interface IBitfinexRestClientSpotApiExchangeData
    {
        /// <summary>
        /// Gets the platform status
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-platform-status" /><br />
        /// Endpoint:<br />
        /// GET /v2/platform/status
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Whether Bitfinex platform is running normally or not</returns>
        Task<WebCallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a list of supported assets
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:map:currency:label
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAsset[]>> GetAssetsListAsync(CancellationToken ct = default);

        /// <summary>
        /// Returns basic market data for the provided symbols
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-ticker" /><br />
        /// Endpoint:<br />
        /// GET /v2/tickers
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol to get data for, for example `tETHUSD`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Symbol data</returns>
        Task<WebCallResult<BitfinexTicker>> GetTickerAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Returns basic market data for the provided funding symbols
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-ticker" /><br />
        /// Endpoint:<br />
        /// GET /v2/tickers
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol to get data for, for example `tETHUSD`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Symbol data</returns>
        Task<WebCallResult<BitfinexFundingTicker>> GetFundingTickerAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Returns basic market data for the provided symbols
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-tickers" /><br />
        /// Endpoint:<br />
        /// GET /v2/tickers
        /// </para>
        /// </summary>
        /// <param name="symbols">The symbols to get data for, for example `tETHUSD`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Symbol data</returns>
        Task<WebCallResult<BitfinexTicker[]>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Returns basic market data for the provided funding symbols
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-tickers" /><br />
        /// Endpoint:<br />
        /// GET /v2/tickers
        /// </para>
        /// </summary>
        /// <param name="symbols">The symbols to get data for, for example `tETHUSD`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Symbol data</returns>
        Task<WebCallResult<BitfinexFundingTicker[]>> GetFundingTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Get ticker history
        /// </summary>
        /// <param name="symbols">Symbols, for example `tETHUSD`</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexTickerHistory[]>> GetTickerHistoryAsync(IEnumerable<string>? symbols = null, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default);

        /// <summary>
        /// Get recent trades for a symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-trades" /><br />
        /// Endpoint:<br />
        /// GET /v2/trades/{symbol}/hist
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol to get trades for, for example `tETHUSD`</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time to return trades for</param>
        /// <param name="endTime">The end time to return trades for</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Trades for the symbol</returns>
        Task<WebCallResult<BitfinexTradeSimple[]>> GetTradeHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the order book for a trading symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-book" /><br />
        /// Endpoint:<br />
        /// GET /v2/book/{symbol}/{precision}
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for, for example `tETHUSD`</param>
        /// <param name="precision">The precision of the data</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The order book for the symbol</returns>
        Task<WebCallResult<BitfinexOrderBook>> GetOrderBookAsync(string symbol, Precision precision, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the order book for a funding symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-book" /><br />
        /// Endpoint:<br />
        /// GET /v2/book/{symbol}/{precision}
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for, for example `tETHUSD`</param>
        /// <param name="precision">The precision of the data</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The order book for the symbol</returns>
        Task<WebCallResult<BitfinexFundingOrderBook>> GetFundingOrderBookAsync(string symbol, Precision precision, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the raw order book for a trading symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-book" /><br />
        /// Endpoint:<br />
        /// GET /v2/book/{symbol}/R0
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexRawOrderBook>> GetRawOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the raw order book for a funding symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-book" /><br />
        /// Endpoint:<br />
        /// GET /v2/book/{symbol}/R0
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="limit">The amount of results in the book</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexRawFundingOrderBook>> GetRawFundingOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the last kline for a symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-candles" /><br />
        /// Endpoint:<br />
        /// GET /v2/candles/trade:{interval}:{symbol}/last
        /// </para>
        /// </summary>
        /// <param name="interval">The time frame of the kline</param>
        /// <param name="symbol">The symbol to get the kline for, for example `tETHUSD`</param>
        /// <param name="fundingPeriod">The Funding period. Only required for funding candles. Enter after the symbol (trade:1m:fUSD:p30/hist).</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The last kline for the symbol</returns>
        Task<WebCallResult<BitfinexKline>> GetLastKlineAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, CancellationToken ct = default);

        /// <summary>
        /// Gets klines for a symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-candles" /><br />
        /// Endpoint:<br />
        /// GET /v2/candles/trade:{interval}:{symbol}/hist
        /// </para>
        /// </summary>
        /// <param name="interval">The time frame of the klines</param>
        /// <param name="symbol">The symbol to get the klines for, for example `tETHUSD`</param>
        /// <param name="fundingPeriod">The Funding period. Only required for funding candles. Enter after the symbol (trade:1m:fUSD:p30/hist).</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time of the klines</param>
        /// <param name="endTime">The end time of the klines</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexKline[]>> GetKlinesAsync(string symbol, KlineInterval interval, string? fundingPeriod = null, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Calculate the average execution price
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-calc-market-average-price" /><br />
        /// Endpoint:<br />
        /// POST /v2/calc/trade/avg
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol to calculate for, for example `tETHUSD`</param>
        /// <param name="quantity">The quantity to execute</param>
        /// <param name="rateLimit">Limit to price</param>
        /// <param name="period">Maximum period for margin funding</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The average price at which the execution would happen</returns>
        Task<WebCallResult<BitfinexAveragePrice>> GetAveragePriceAsync(string symbol, decimal quantity, decimal? rateLimit = null, int? period = null, CancellationToken ct = default);

        /// <summary>
        /// Returns the exchange rate for the assets
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference#rest-public-calc-foreign-exchange-rate" /><br />
        /// Endpoint:<br />
        /// POST /v2/calc/fx
        /// </para>
        /// </summary>
        /// <param name="asset1">The first asset, for example `ETH`</param>
        /// <param name="asset2">The second asset, for example `ETH`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Exchange rate</returns>
        Task<WebCallResult<BitfinexForeignExchangeRate>> GetForeignExchangeRateAsync(string asset1, string asset2, CancellationToken ct = default);

        /// <summary>
        /// Get derivatives status info
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-derivatives-status" /><br />
        /// Endpoint:<br />
        /// GET /v2/status/deriv
        /// </para>
        /// </summary>
        /// <param name="symbols">Filter symbols, for example `tETHUSD`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexDerivativesStatus[]>> GetDerivativesStatusAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Get derivatives status info history
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-derivatives-status-history" /><br />
        /// Endpoint:<br />
        /// GET /v2/status/deriv/{symbol}/hist
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time of the data</param>
        /// <param name="endTime">The end time of the data</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexDerivativesStatus[]>> GetDerivativesStatusHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Get liquidation history
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-liquidations" /><br />
        /// Endpoint:<br />
        /// GET /v2/liquidations/hist
        /// </para>
        /// </summary>
        /// <param name="limit">The amount of results</param>
        /// <param name="startTime">The start time of the data</param>
        /// <param name="endTime">The end time of the data</param>
        /// <param name="sorting">The way the result is sorted</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexLiquidation[]>> GetLiquidationsAsync(int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Get a list of the most recent funding data for the given asset: FRR, average period, total amount provided, total amount used
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-funding-stats" /><br />
        /// Endpoint:<br />
        /// GET /v2/funding/stats/{symbol}/hist
        /// </para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `tETHUSD`</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexFundingStats[]>> GetFundingStatisticsAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default);

        /// <summary>
        /// Get total active funding in specified asset
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/funding.size:1m:{asset}/last
        /// </para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats>> GetLastFundingSizeAsync(string asset, CancellationToken ct = default);

        /// <summary>
        /// Get total active funding in specified asset
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/funding.size:1m:{asset}/hist
        /// </para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="sorting">Sorting</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats[]>> GetFundingSizeHistoryAsync(string asset, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Get total funding used in positions in specified asset
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/credits.size:1m:{asset}/last
        /// </para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats>> GetLastCreditSizeAsync(string asset, CancellationToken ct = default);

        /// <summary>
        /// Get total funding used in positions in specified asset
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/credits.size:1m:{asset}/hist
        /// </para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="sorting">Sorting</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats[]>> GetCreditSizeHistoryAsync(string asset, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Get total funding used in positions on a specific symbol in specified asset
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/credits.size.sym:1m:{asset}:{symbol}/last
        /// </para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats>> GetLastCreditSizeAsync(string asset, string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get total funding used in positions on a specific symbol in specified asset
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/credits.size.sym:1m:{asset}:{symbol}/hist
        /// </para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="sorting">Sorting</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats[]>> GetCreditSizeHistoryAsync(string asset, string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Get total longs/shorts in base currency (i.e. BTC for tBTCUSD)
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/pos.size:1m:{symbol}:{side}/last
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="side">Position side</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats>> GetLastLongsShortsTotalsAsync(string symbol, StatSide side, CancellationToken ct = default);

        /// <summary>
        /// Get total longs/shorts in base currency (i.e. BTC for tBTCUSD)
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/pos.size:1m:{symbol}:{side}/hist
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="side">Position side</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="sorting">Sorting</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats[]>> GetLongsShortsTotalsHistoryAsync(string symbol, StatSide side, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Get trading volume on the platform
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/vol.{period}d:30m:BFX/last
        /// </para>
        /// </summary>
        /// <param name="period">The period in days to get the data for. 1, 7 or 30</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats>> GetLastTradingVolumeAsync(int period, CancellationToken ct = default);

        /// <summary>
        /// Get trading volume on the platform
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/vol.{period}d:30m:BFX/hist
        /// </para>
        /// </summary>
        /// <param name="period">The period in days to get the data for. 1, 7 or 30</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="sorting">Sorting</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats[]>> GetTradingVolumeHistoryAsync(int period, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Get volume weighted average price for the day
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/vwap:1d:{symbol}/last
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats>> GetLastVolumeWeightedAveragePriceAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get volume weighted average price for the day
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/stats1/vwap:1d:{symbol}/hist
        /// </para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `tETHUSD`</param>
        /// <param name="limit">Max number of results</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="sorting">Sorting</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexStats[]>> GetVolumeWeightedAveragePriceHistoryAsync(string symbol, int? limit = null, DateTime? startTime = null, DateTime? endTime = null, Sorting? sorting = null, CancellationToken ct = default);

        /// <summary>
        /// Get symbol names
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:list:pair:{type}
        /// </para>
        /// </summary>
        /// <param name="type">The types of symbol</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<string[]>> GetSymbolNamesAsync(SymbolType type, CancellationToken ct = default);

        /// <summary>
        /// Get asset names
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:list:currency
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<string[]>> GetAssetNamesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get mapping of assets to their API symbol
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:map:currency:sym
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, string>>> GetAssetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get mapping of assets to their full name
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:map:currency:label
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, string>>> GetAssetFullNamesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get mapping of assets to their unit of measure where applicable
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:map:currency:unit
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, string>>> GetAssetUnitsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get mapping of derivative assets to their underlying asset
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:map:currency:undl
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, string>>> GetAssetUnderlyingsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get mapping of assets to the network they operate on
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:map:currency:pool
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, string>>> GetAssetNetworksAsync(CancellationToken ct = default);

        /// <summary>
        /// Get mapping of assets to their block explorer urls
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:map:currency:explorer
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, string[]>>> GetAssetBlockExplorerUrlsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get mapping of assets to their withdrawal fees
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:map:currency:tx:fee
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, decimal[]>>> GetAssetWithdrawalFeesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get mapping of assets to their withdrawal methods
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:map:tx:method
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, string[]>>> GetAssetDepositWithdrawalMethodsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get list of market information for each trading pair
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:info:pair
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, BitfinexSymbolInfo>>> GetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get list of market information for each derivative trading pair
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:info:pair:futures
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, BitfinexSymbolInfo>>> GetFuturesSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get deposit/withdrawal status info for assets
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:info:tx:status
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexAssetInfo[]>> GetDepositWithdrawalStatusAsync(CancellationToken ct = default);

        /// <summary>
        /// Get lists of active haircuts and risk coefficients on margin pairs
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:spec:margin
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexMarginInfo>> GetMarginInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Get derivatives fees config
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.bitfinex.com/reference/rest-public-conf" /><br />
        /// Endpoint:<br />
        /// GET /v2/conf/pub:fees
        /// </para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<BitfinexDerivativesFees>> GetDerivativesFeesAsync(CancellationToken ct = default);
    }
}
