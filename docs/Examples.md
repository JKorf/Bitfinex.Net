---
title: Examples
nav_order: 3
---

## Basic operations
Make sure to read the [CryptoExchange.Net documentation](https://jkorf.github.io/CryptoExchange.Net/Clients.html#processing-request-responses) on processing responses.

### Get market data
```csharp
// Getting info on all symbols
var symbolData = await bitfinexClient.SpotApi.ExchangeData.GetSymbolsAsync();

// Getting tickers for all symbols
var tickerData = await bitfinexClient.SpotApi.ExchangeData.GetTickersAsync();

// Getting the order book of a symbol
var orderBookData = await bitfinexClient.SpotApi.ExchangeData.GetOrderBookAsync("tBTCUST", Precision.PrecisionLevel0);

// Getting recent trades of a symbol
var tradeHistoryData = await bitfinexClient.SpotApi.ExchangeData.GetTradeHistoryAsync("tBTCUST");
```

### Requesting balances
```csharp
var accountData = await bitfinexClient.SpotApi.Account.GetBalancesAsync();
```
### Placing order
```csharp
// Placing a buy limit order for 0.001 BTC at a price of 50000USDT each
var symbolData = await bitfinexClient.SpotApi.Trading.PlaceOrderAsync(
                "tBTCUST",
                OrderSide.Buy,
                OrderType.ExchangeLimit,
                0.001m,
                50000);
													
// Place a stop loss order, place a limit order of 0.001 BTC at 39000USDT each when the last trade price drops below 40000USDT
var orderData = await bitfinexClient.SpotApi.Trading.PlaceOrderAsync(
                "tBTCUST",
                OrderSide.Sell,
                OrderType.ExchangeStopLimit,
                0.001m,
                39000,
                priceAuxLimit: 40000);
```

### Requesting a specific order
```csharp
// Request info on order with id `1234`
var orderData = await bitfinexClient.SpotApi.Trading.GetOrderAsync(1234);
```

### Requesting order history
```csharp
// Get all orders conform the parameters
 var ordersData = await bitfinexClient.SpotApi.Trading.GetClosedOrdersAsync();
```

### Cancel order
```csharp
// Cancel order with id `1234`
var orderData = await bitfinexClient.SpotApi.Trading.CancelOrderAsync(1234);
```

### Get user trades
```csharp
var userTradesResult = await bitfinexClient.SpotApi.Trading.GetUserTradesAsync();
```

### Subscribing to market data updates
```csharp
var subscribeResult =  bitfinexSocket.SpotStreams.SubscribeToTickerUpdatesAsync("tBTCUST", data =>
{
	// Handle ticker data
});
```

### Subscribing to order updates
```csharp
// Any handler can be passed `null` if you're not interested in that type of update
var subscribeResult = await bitfinexSocket.SpotStreams.SubscribeToUserTradeUpdatesAsync(
	data =>
	{
	  // Handle order updates
	},
	data =>
	{
	  // Handle trade updates
	}, 
	data =>
	{
	  // Handle position updates
	});
```
