---
title: IBitfinexRestClientSpotApiExchangeData
has_children: false
parent: IBitfinexRestClientSpotApi
grand_parent: Rest API documentation
---
*[generated documentation]*  
`BitfinexRestClient > SpotApi > ExchangeData`  
*Bitfinex exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.*
  

***

## GetAssetBlockExplorerUrlsAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get mapping of assets to their block explorer urls*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAssetBlockExplorerUrlsAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, IEnumerable<string>>>> GetAssetBlockExplorerUrlsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAssetDepositWithdrawalMethodsAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get mapping of assets to their withdrwal methods*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAssetDepositWithdrawalMethodsAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, IEnumerable<string>>>> GetAssetDepositWithdrawalMethodsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAssetFullNamesAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get mapping of assets to their full name*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAssetFullNamesAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, string>>> GetAssetFullNamesAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAssetNamesAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get asset names*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAssetNamesAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<string>>> GetAssetNamesAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAssetNetworksAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get mapping of assets to the network they operate on*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAssetNetworksAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, string>>> GetAssetNetworksAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAssetsListAsync  

[https://docs.bitfinex.com/reference#rest-public-conf](https://docs.bitfinex.com/reference#rest-public-conf)  
<p>

*Gets a list of supported assets*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAssetsListAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexAsset>>> GetAssetsListAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAssetSymbolsAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get mapping of assets to their API symbol*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAssetSymbolsAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, string>>> GetAssetSymbolsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAssetUnderlyingsAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get mapping of derivative assets to their underlying asset*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAssetUnderlyingsAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, string>>> GetAssetUnderlyingsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAssetUnitsAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get mapping of assets to their unit of measure where applicable*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAssetUnitsAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, string>>> GetAssetUnitsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAssetWithdrawalFeesAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get mapping of assets to their withdrawal fees*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAssetWithdrawalFeesAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, IEnumerable<decimal>>>> GetAssetWithdrawalFeesAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAveragePriceAsync  

[https://docs.bitfinex.com/reference#rest-public-calc-market-average-price](https://docs.bitfinex.com/reference#rest-public-calc-market-average-price)  
<p>

*Calculate the average execution price*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetAveragePriceAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexAveragePrice>> GetAveragePriceAsync(string symbol, decimal quantity, decimal? rateLimit = default, int? period = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to calculate for|
|quantity|The quantity to execute|
|_[Optional]_ rateLimit|Limit to price|
|_[Optional]_ period|Maximum period for margin funding|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetCreditSizeAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get total funding used in positions in specified asset*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetCreditSizeAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexStats>>> GetCreditSizeAsync(string asset, StatSection section, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset|The asset|
|section|Section of data|
|_[Optional]_ limit|Max number of results|
|_[Optional]_ startTime|Filter by start time|
|_[Optional]_ endTime|Filter by end time|
|_[Optional]_ sorting|Sorting|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetCreditSizeAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get total funding used in positions on a specific symbol in specified asset*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetCreditSizeAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexStats>>> GetCreditSizeAsync(string asset, string symbol, StatSection section, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset|The asset|
|symbol|The symbol|
|section|Section of data|
|_[Optional]_ limit|Max number of results|
|_[Optional]_ startTime|Filter by start time|
|_[Optional]_ endTime|Filter by end time|
|_[Optional]_ sorting|Sorting|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetDepositWithdrawalStatusAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get deposit/withdrawal status info for assets*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetDepositWithdrawalStatusAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexAssetInfo>>> GetDepositWithdrawalStatusAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetDerivativesFeesAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get derivatives fees config*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetDerivativesFeesAsync();  
```  

```csharp  
Task<WebCallResult<BitfinexDerivativesFees>> GetDerivativesFeesAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetDerivativesStatusAsync  

[https://docs.bitfinex.com/reference/rest-public-derivatives-status](https://docs.bitfinex.com/reference/rest-public-derivatives-status)  
<p>

*Get derivatives status info*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetDerivativesStatusAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexDerivativesStatus>>> GetDerivativesStatusAsync(IEnumerable<string>? symbols = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbols|Filter symbols|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetDerivativesStatusHistoryAsync  

[https://docs.bitfinex.com/reference/rest-public-derivatives-status-history](https://docs.bitfinex.com/reference/rest-public-derivatives-status-history)  
<p>

*Get derivatives status info history*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetDerivativesStatusHistoryAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexDerivativesStatus>>> GetDerivativesStatusHistoryAsync(string symbol, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol|
|_[Optional]_ limit|The amount of results|
|_[Optional]_ startTime|The start time of the data|
|_[Optional]_ endTime|The end time of the data|
|_[Optional]_ sorting|The way the result is sorted|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetForeignExchangeRateAsync  

[https://docs.bitfinex.com/reference#rest-public-calc-foreign-exchange-rate](https://docs.bitfinex.com/reference#rest-public-calc-foreign-exchange-rate)  
<p>

*Returns the exchange rate for the assets*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetForeignExchangeRateAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexForeignExchangeRate>> GetForeignExchangeRateAsync(string asset1, string asset2, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset1|The first asset|
|asset2|The second asset|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingOrderBookAsync  

[https://docs.bitfinex.com/reference#rest-public-book](https://docs.bitfinex.com/reference#rest-public-book)  
<p>

*Gets the order book for a funding symbol*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetFundingOrderBookAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexFundingOrderBook>> GetFundingOrderBookAsync(string symbol, Precision precision, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the order book for|
|precision|The precision of the data|
|_[Optional]_ limit|The amount of results in the book|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingSizeAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get total active funding in specified asset*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetFundingSizeAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexStats>>> GetFundingSizeAsync(string asset, StatSection section, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset|The asset|
|section|Section of data|
|_[Optional]_ limit|Max number of results|
|_[Optional]_ startTime|Filter by start time|
|_[Optional]_ endTime|Filter by end time|
|_[Optional]_ sorting|Sorting|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingStatisticsAsync  

[https://docs.bitfinex.com/reference/rest-public-funding-stats](https://docs.bitfinex.com/reference/rest-public-funding-stats)  
<p>

*Get a list of the most recent funding data for the given asset: FRR, average period, total amount provided, total amount used*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetFundingStatisticsAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexFundingStats>>> GetFundingStatisticsAsync(string symbol, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol||
|_[Optional]_ limit||
|_[Optional]_ startTime||
|_[Optional]_ endTime||
|_[Optional]_ ct||

</p>

***

## GetFundingTickerAsync  

[https://docs.bitfinex.com/reference#rest-public-ticker](https://docs.bitfinex.com/reference#rest-public-ticker)  
<p>

*Returns basic market data for the provided funding symbols*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetFundingTickerAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexFundingTicker>> GetFundingTickerAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get data for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingTickersAsync  

[https://docs.bitfinex.com/reference#rest-public-tickers](https://docs.bitfinex.com/reference#rest-public-tickers)  
<p>

*Returns basic market data for the provided funding symbols*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetFundingTickersAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexFundingTicker>>> GetFundingTickersAsync(IEnumerable<string>? symbols = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbols|The symbols to get data for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFuturesSymbolsAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get list of market information for each derivative trading pair*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetFuturesSymbolsAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, BitfinexSymbolInfo>>> GetFuturesSymbolsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetKlinesAsync  

[https://docs.bitfinex.com/reference#rest-public-candles](https://docs.bitfinex.com/reference#rest-public-candles)  
<p>

*Gets klines for a symbol*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetKlinesAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexKline>>> GetKlinesAsync(string symbol, KlineInterval interval, string? fundingPeriod = default, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the klines for|
|interval|The time frame of the klines|
|_[Optional]_ fundingPeriod|The Funding period. Only required for funding candles. Enter after the symbol (trade:1m:fUSD:p30/hist).|
|_[Optional]_ limit|The amount of results|
|_[Optional]_ startTime|The start time of the klines|
|_[Optional]_ endTime|The end time of the klines|
|_[Optional]_ sorting|The way the result is sorted|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetLastKlineAsync  

[https://docs.bitfinex.com/reference#rest-public-candles](https://docs.bitfinex.com/reference#rest-public-candles)  
<p>

*Get the last kline for a symbol*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetLastKlineAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexKline>> GetLastKlineAsync(string symbol, KlineInterval interval, string? fundingPeriod = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the kline for|
|interval|The time frame of the kline|
|_[Optional]_ fundingPeriod|The Funding period. Only required for funding candles. Enter after the symbol (trade:1m:fUSD:p30/hist).|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetLiquidationsAsync  

[https://docs.bitfinex.com/reference/rest-public-liquidations](https://docs.bitfinex.com/reference/rest-public-liquidations)  
<p>

*Get liquidation history*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetLiquidationsAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexLiquidation>>> GetLiquidationsAsync(int? limit = default, DateTime? startTime = default, DateTime? endTime = default, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ limit|The amount of results|
|_[Optional]_ startTime|The start time of the data|
|_[Optional]_ endTime|The end time of the data|
|_[Optional]_ sorting|The way the result is sorted|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetLongsShortsTotalsAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get total longs/shorts in base currency (i.e. BTC for tBTCUSD)*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetLongsShortsTotalsAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexStats>>> GetLongsShortsTotalsAsync(string symbol, StatSide side, StatSection section, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol|
|side|Position side|
|section|Section of data|
|_[Optional]_ limit|Max number of results|
|_[Optional]_ startTime|Filter by start time|
|_[Optional]_ endTime|Filter by end time|
|_[Optional]_ sorting|Sorting|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetMarginInfoAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get lists of active haircuts and risk coefficients on margin pairs*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetMarginInfoAsync();  
```  

```csharp  
Task<WebCallResult<BitfinexMarginInfo>> GetMarginInfoAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOrderBookAsync  

[https://docs.bitfinex.com/reference#rest-public-book](https://docs.bitfinex.com/reference#rest-public-book)  
<p>

*Gets the order book for a trading symbol*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetOrderBookAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexOrderBook>> GetOrderBookAsync(string symbol, Precision precision, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the order book for|
|precision|The precision of the data|
|_[Optional]_ limit|The amount of results in the book|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetPlatformStatusAsync  

[https://docs.bitfinex.com/reference#rest-public-platform-status](https://docs.bitfinex.com/reference#rest-public-platform-status)  
<p>

*Gets the platform status*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetPlatformStatusAsync();  
```  

```csharp  
Task<WebCallResult<BitfinexPlatformStatus>> GetPlatformStatusAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetRawFundingOrderBookAsync  

[https://docs.bitfinex.com/reference#rest-public-book](https://docs.bitfinex.com/reference#rest-public-book)  
<p>

*Get the raw order book for a funding symbol*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetRawFundingOrderBookAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexRawFundingOrderBook>> GetRawFundingOrderBookAsync(string symbol, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol|
|_[Optional]_ limit|The amount of results in the book|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetRawOrderBookAsync  

[https://docs.bitfinex.com/reference#rest-public-book](https://docs.bitfinex.com/reference#rest-public-book)  
<p>

*Get the raw order book for a trading symbol*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetRawOrderBookAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexRawOrderBook>> GetRawOrderBookAsync(string symbol, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol|
|_[Optional]_ limit|The amount of results in the book|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetSymbolNamesAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get symbol names*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetSymbolNamesAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<string>>> GetSymbolNamesAsync(SymbolType type, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|type|The types of symbol|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetSymbolsAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get list of market information for each trading pair*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetSymbolsAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, BitfinexSymbolInfo>>> GetSymbolsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetTickerAsync  

[https://docs.bitfinex.com/reference#rest-public-ticker](https://docs.bitfinex.com/reference#rest-public-ticker)  
<p>

*Returns basic market data for the provided symbols*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetTickerAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexTicker>> GetTickerAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get data for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetTickerHistoryAsync  

<p>

*Get ticker history*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetTickerHistoryAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexTickerHistory>>> GetTickerHistoryAsync(IEnumerable<string>? symbols = default, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbols|Symbols|
|_[Optional]_ limit|The amount of results|
|_[Optional]_ startTime|Filter by start time|
|_[Optional]_ endTime|Filter by end time|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetTickersAsync  

[https://docs.bitfinex.com/reference#rest-public-tickers](https://docs.bitfinex.com/reference#rest-public-tickers)  
<p>

*Returns basic market data for the provided symbols*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetTickersAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexTicker>>> GetTickersAsync(IEnumerable<string>? symbols = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbols|The symbols to get data for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetTradeHistoryAsync  

[https://docs.bitfinex.com/reference#rest-public-trades](https://docs.bitfinex.com/reference#rest-public-trades)  
<p>

*Get recent trades for a symbol*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetTradeHistoryAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexTradeSimple>>> GetTradeHistoryAsync(string symbol, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get trades for|
|_[Optional]_ limit|The amount of results|
|_[Optional]_ startTime|The start time to return trades for|
|_[Optional]_ endTime|The end time to return trades for|
|_[Optional]_ sorting|The way the result is sorted|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetTradingVolumeAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get trading volume on the platform*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetTradingVolumeAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexStats>>> GetTradingVolumeAsync(int period, StatSection section, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|period|The period in days to get the data for. 1, 7 or 30|
|section|Section of data|
|_[Optional]_ limit|Max number of results|
|_[Optional]_ startTime|Filter by start time|
|_[Optional]_ endTime|Filter by end time|
|_[Optional]_ sorting|Sorting|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetVolumeWeightedAveragePriceAsync  

[https://docs.bitfinex.com/reference/rest-public-conf](https://docs.bitfinex.com/reference/rest-public-conf)  
<p>

*Get volume weighted average price for the day*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.ExchangeData.GetVolumeWeightedAveragePriceAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexStats>>> GetVolumeWeightedAveragePriceAsync(string symbol, StatSection section, int? limit = default, DateTime? startTime = default, DateTime? endTime = default, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol|
|section|Section of data|
|_[Optional]_ limit|Max number of results|
|_[Optional]_ startTime|Filter by start time|
|_[Optional]_ endTime|Filter by end time|
|_[Optional]_ sorting|Sorting|
|_[Optional]_ ct|Cancellation token|

</p>
