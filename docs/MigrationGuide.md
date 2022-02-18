---
title: Migrate V4 to V5
nav_order: 4
---

## Migrate from version V4.x.x to V5.x.x

There are a decent amount of breaking changes when moving from version 4.x.x to version 5.x.x. Although the interface has changed, the available endpoints/information have not, so there should be no need to completely rewrite your program.
Most endpoints are now available under a slightly different name or path, and most data models have remained the same, barring a few renames.
In this document most changes will be described. If you have any other questions or issues when updating, feel free to open an issue.

Changes related to `IExchangeClient`, options and client structure are also (partially) covered in the [CryptoExchange.Net Migration Guide](https://jkorf.github.io/CryptoExchange.Net/Migration%20Guide.html)

### Namespaces
There are a few namespace changes:  

|Type|Old|New|
|----|---|---|
|Enums|`Bitfinex.Net.Objects`|`Bitfinex.Net.Enums`  |
|Clients|`Bitfinex.Net`|`Bitfinex.Net.Clients`  |
|Client interfaces|`Bitfinex.Net.Interfaces`|`Bitfinex.Net.Interfaces.Clients`  |
|Objects|`Bitfinex.Net.Objects`|`Bitfinex.Net.Objects.Models`  |
|SymbolOrderBook|`Bitfinex.Net`|`Bitfinex.Net.SymbolOrderBooks`|

### Client options
The BaseAddress and rate limiting options are now under the `SpotApiOptions`.  
*V4*
```csharp
var bitfinexClient = new BitfinexClient(new BitfinexClientOptions
{
	ApiCredentials = new ApiCredentials("API-KEY", "API-SECRET"),
	BaseAddress = "ADDRESS",
	RateLimitingBehaviour = RateLimitingBehaviour.Fail
});
```

*V5*
```csharp
var bitfinexClient = new BitfinexClient(new BitfinexClientOptions
{
	ApiCredentials = new ApiCredentials("API-KEY", "API-SECRET"),
	SpotApiOptions = new RestApiClientOptions
	{
		BaseAddress = "ADDRESS",
		RateLimitingBehaviour = RateLimitingBehaviour.Fail
	}
});
```

### Client structure
Version 5 adds the `GeneralApi` and `SpotApi` Api clients under the `BitfinexClient`, and a topic underneath that. This is done to keep the same client structure as other exchange implementations, more info on this [here](https://jkorf.github.io/CryptoExchange.Net/Clients.html).
In the BitfinexSocketClient a `SpotStreams` Api client is added. This means all calls will have changed, though most will only need to add `SpotApi.[Topic].XXX`/`SpotStreams.XXX`:

*V4*
```csharp
var balances = await bitfinexClient.GetBalancesAsync();
var movements = await bitfinexClient.GetMovementsAsync();

var tickers = await bitfinexClient.GetTickersAsync();
var symbols = await bitfinexClient.GetSymbolsAsync();

var order = await bitfinexClient.PlaceOrderAsync();
var trades = await bitfinexClient.GetUserTradesAsync();

var sub = bitfinexSocket.SubscribeToTickerUpdatesAsync("tBTCUSD", DataHandler);
```

*V5*  
```csharp
var balances = await bitfinexClient.SpotApi.Account.GetBalancesAsync();
var movements = await bitfinexClient.SpotApi.Account.GetMovementsAsync();

var tickers = await bitfinexClient.SpotApi.ExchangeData.GetTickersAsync();
var symbols = await bitfinexClient.SpotApi.ExchangeData.GetSymbolsAsync();

var order = await bitfinexClient.SpotApi.Trading.PlaceOrderAsync();
var trades = await bitfinexClient.SpotApi.Trading.GetUserTradesAsync();

var sub = bitfinexSocket.SpotStreams.SubscribeToTickerUpdatesAsync("tBTCUSD", DataHandler);
```

### Definitions
Some names have been changed to a common definition. This includes where the name is part of a bigger name  

|Old|New||
|----|---|---|
|`Currency`|`Asset`|`GetCurrenciesAsync` -> `GetAssetsAsync`|
|`TimeFrame`|`KlineInterval`||
|`Size/Amount`|`Quantity`|`WithdrawMinSize` -> `WithdrawMinQuantity`|
|`Open/High/Low/Close`|`OpenPrice/HighPrice/LowPrice/ClosePrice` ||
|`BestAsk/BestBid`|`BestAskPrice/BestBidPrice`||
|`GetActiveOrdersAsync`|`GetOpenOrdersAsync`||

### BitfinexSymbolOrderBook
The constructor for the `BitfinexSymbolOrderBook` no longer requires a precisionLevel and limit parameter. They are still available to set via the `BitfinexOrderBookOptions`. If not set in the options the default values will be `PrecisionLevel0` and `25`.
*V4*
```csharp
var book = new BitfinexSymbolOrderBook("tBTCUST", Precision.PrecisionLevel0, 25);
```

*V5*
```csharp
var book = new BitfinexSymbolOrderBook("tBTCUST", new BitfinexOrderBookOptions {
	Precision = Precision.PrecisionLevel0,
	Limit = 25,
});
```

### Changed methods
The spot PlaceOrderAsync call has had the price and quantity parameter swapped(!) so the parameters are the same as for the PlaceOrderAsync calls in other CryptoExchange.Net implementations:  
*V4*  
```csharp
Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType type,
            decimal price,	// <--
            decimal amount,	// <--
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
```
*V5*  
```csharp
Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType type,
            decimal quantity,	// <--
            decimal price,		// <--
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
```