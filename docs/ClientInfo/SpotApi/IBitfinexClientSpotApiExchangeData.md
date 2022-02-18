---
title: IBitfinexClientSpotApiExchangeData
has_children: false
parent: IBitfinexClientSpotApi
grand_parent: Rest API documentation
---
*[generated documentation]*  
`BitfinexClient > SpotApi > ExchangeData`  
*Bitfinex exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.*
  

***

## GetAssetsAsync  

[https://docs.bitfinex.com/reference#rest-public-conf](https://docs.bitfinex.com/reference#rest-public-conf)  
<p>

*Gets a list of supported assets*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.ExchangeData.GetAssetsAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexAsset>>> GetAssetsAsync(CancellationToken ct = default);  
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
var client = new BitfinexClient();  
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

## GetForeignExchangeRateAsync  

[https://docs.bitfinex.com/reference#rest-public-calc-foreign-exchange-rate](https://docs.bitfinex.com/reference#rest-public-calc-foreign-exchange-rate)  
<p>

*Returns the exchange rate for the assets*  

```csharp  
var client = new BitfinexClient();  
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

## GetFundingBookAsync  

[https://docs.bitfinex.com/v1/reference#rest-public-fundingbook](https://docs.bitfinex.com/v1/reference#rest-public-fundingbook)  
<p>

*Gets the margin funding book*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.ExchangeData.GetFundingBookAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexFundingBook>> GetFundingBookAsync(string asset, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset|Asset to get the book for|
|_[Optional]_ limit|Limit of the results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetKlinesAsync  

[https://docs.bitfinex.com/reference#rest-public-candles](https://docs.bitfinex.com/reference#rest-public-candles)  
<p>

*Gets klines for a symbol*  

```csharp  
var client = new BitfinexClient();  
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
var client = new BitfinexClient();  
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

## GetLendsAsync  

[https://docs.bitfinex.com/v1/reference#rest-public-lends](https://docs.bitfinex.com/v1/reference#rest-public-lends)  
<p>

*Gets the most recent lends*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.ExchangeData.GetLendsAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexLend>>> GetLendsAsync(string asset, DateTime? startTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset|Asset to get the book for|
|_[Optional]_ startTime|Return data after this time|
|_[Optional]_ limit|Limit of the results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOrderBookAsync  

[https://docs.bitfinex.com/reference#rest-public-book](https://docs.bitfinex.com/reference#rest-public-book)  
<p>

*Gets the order book for a symbol*  

```csharp  
var client = new BitfinexClient();  
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
var client = new BitfinexClient();  
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

## GetRawOrderBookAsync  

[https://docs.bitfinex.com/reference#rest-public-book](https://docs.bitfinex.com/reference#rest-public-book)  
<p>

*Get the raw order book for a symbol*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.ExchangeData.GetRawOrderBookAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexOrderBook>> GetRawOrderBookAsync(string symbol, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol|
|_[Optional]_ limit|The amount of results in the book|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetStatsAsync  

[https://docs.bitfinex.com/reference#rest-public-stats1](https://docs.bitfinex.com/reference#rest-public-stats1)  
<p>

*Get various stats for the symbol*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.ExchangeData.GetStatsAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexStats>>> GetStatsAsync(string symbol, StatKey key, StatSide side, StatSection section, Sorting? sorting = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to request stats for|
|key|The type of stats|
|side|Side of the stats|
|section|Section of the stats|
|_[Optional]_ sorting|The way the result should be sorted|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetSymbolDetailsAsync  

[https://docs.bitfinex.com/v1/reference#rest-public-symbol-details](https://docs.bitfinex.com/v1/reference#rest-public-symbol-details)  
<p>

*Gets details of all symbols*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.ExchangeData.GetSymbolDetailsAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexSymbolDetails>>> GetSymbolDetailsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetSymbolsAsync  

[https://docs.bitfinex.com/v1/reference#rest-public-symbols](https://docs.bitfinex.com/v1/reference#rest-public-symbols)  
<p>

*Gets a list of all symbols*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.ExchangeData.GetSymbolsAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default);  
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
var client = new BitfinexClient();  
var result = await client.SpotApi.ExchangeData.GetTickerAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexSymbolOverview>> GetTickerAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get data for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetTickersAsync  

[https://docs.bitfinex.com/reference#rest-public-tickers](https://docs.bitfinex.com/reference#rest-public-tickers)  
<p>

*Returns basic market data for the provided symbols*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.ExchangeData.GetTickersAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexSymbolOverview>>> GetTickersAsync(IEnumerable<string>? symbols = default, CancellationToken ct = default);  
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
var client = new BitfinexClient();  
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
