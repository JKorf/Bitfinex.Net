# Bitfinex.Net usage

These snippets use the imports below. Account, trading, and funding-management snippets assume an authenticated `client`. See [API surfaces](api-surfaces.md) for routing and [operational considerations](safety.md) for wallet and retry decisions.

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using Bitfinex.Net;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Interfaces.Clients;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.DependencyInjection;
```

## Authentication

Public calls can use `new BitfinexRestClient()`. Private calls require HMAC API-key credentials:

```csharp
using var client = new BitfinexRestClient(options =>
{
    options.ApiCredentials = new BitfinexCredentials("API_KEY", "API_SECRET");
});
```

The strings are placeholders. For permissions and storage, see [credentials](safety.md#credentials).

## REST results

```csharp
using var client = new BitfinexRestClient();
var ticker = await client.ExchangeApi.ExchangeData.GetTickerAsync("tBTCUSD");
if (!ticker.Success)
{
    Console.WriteLine(ticker.Error);
    return;
}

Console.WriteLine(ticker.Data.LastPrice);
```

On failure, diagnostic fields include `ErrorCode`, `Message`, `ErrorType`, and `IsTransient`. See [retry decisions](safety.md#retry-decisions).

## Wallet balances

```csharp
var wallets = await client.ExchangeApi.Account.GetBalancesAsync();
if (!wallets.Success)
{
    Console.WriteLine(wallets.Error);
    return;
}

foreach (var wallet in wallets.Data.Where(x => x.Total > 0))
    Console.WriteLine($"{wallet.Type} {wallet.Asset}: total={wallet.Total}, available={wallet.Available}");
```

## Exchange-wallet spot orders

```csharp
var order = await client.ExchangeApi.Trading.PlaceOrderAsync(
    symbol: "tBTCUSD",
    side: OrderSide.Buy,
    type: OrderType.ExchangeLimit,
    quantity: 0.001m,
    price: 50000m);

if (!order.Success)
{
    Console.WriteLine(order.Error);
    return;
}

Console.WriteLine(order.Data.Data.Id);
```

The outer `Data` is a write notification; its `Data` contains the changed order. The REST message handler converts non-success notification status to an error result. Cancel through `CancelOrderAsync(orderId: ...)`; inspect the cancellation result as well.

## Margin and derivatives

Use non-exchange order types such as `OrderType.Limit` for margin orders. Derivative symbols such as `tBTCF0:USTF0` use the same `ExchangeApi.Trading` surface; `PlaceOrderAsync` accepts `leverage`. It also requires a `price` argument, so inspect its signature even when selecting a market order type.

Read current positions with `ExchangeApi.Trading.GetPositionsAsync()` and position history with `GetPositionHistoryAsync(...)`. See [trading and wallet selection](safety.md#trading-and-wallet-selection) before adapting the exchange-wallet example.

## Funding

Public funding data is under `ExchangeApi.ExchangeData`:

```csharp
var ticker = await client.ExchangeApi.ExchangeData.GetFundingTickerAsync("fUSD");
if (!ticker.Success)
{
    Console.WriteLine(ticker.Error);
    return;
}

Console.WriteLine(ticker.Data.LastPrice);
```

Authenticated funding management is under `GeneralApi.Funding`:

```csharp
var offer = await client.GeneralApi.Funding.SubmitFundingOfferAsync(
    fundingOrderType: FundingOrderType.Limit,
    symbol: "fUSD",
    quantity: 100m,
    rate: 0.0002m,
    period: 2);

if (!offer.Success)
{
    Console.WriteLine(offer.Error);
    return;
}

Console.WriteLine(offer.Data.Data.Id);
var cancel = await client.GeneralApi.Funding.CancelFundingOfferAsync(offer.Data.Data.Id);
if (!cancel.Success)
    Console.WriteLine(cancel.Error);
```

The example submits an offer and then requests cancellation; an offer may already have been matched. See [funding offers](safety.md#funding-offers).

## WebSocket lifecycle

```csharp
using var socket = new BitfinexSocketClient();
var subscription = await socket.ExchangeApi.SubscribeToTickerUpdatesAsync(
    "tBTCUSD",
    update => Console.WriteLine(update.Data.LastPrice));

if (!subscription.Success)
{
    Console.WriteLine(subscription.Error);
    return;
}

try
{
    Console.ReadLine(); // Replace with the application's lifetime.
}
finally
{
    await socket.UnsubscribeAsync(subscription.Data);
}
```

Kline callbacks deliver a collection in `update.Data`. With socket credentials configured, `SubscribeToUserUpdatesAsync(walletHandler: ...)` provides authenticated wallet updates; other callbacks cover orders, trades, positions, and funding events. There is no listen-key setup step for this subscription. Socket order/funding commands return `QueryResult<T>` rather than subscription handles.

## Shared API V2

```csharp
using var client = new BitfinexRestClient();
IGetTickerRest ticker = client.ExchangeApi.SharedApi;
var result = await ticker.GetTickerAsync(
    new GetTickerRequest(new SharedSymbol(TradingMode.Spot, "BTC", "USD")));

if (!result.Success)
{
    Console.WriteLine(result.Error);
    return;
}

Console.WriteLine(result.Data.LastPrice);
```

For aggregate properties and runtime capability lookup, see [Shared APIs](api-surfaces.md#shared-api-v2).

## Dependency injection

```csharp
var services = new ServiceCollection();
services.AddBitfinex();
```

For private calls, configure `options.ApiCredentials` in the `AddBitfinex(options => ...)` callback with the credentials shown in [authentication](#authentication). Inject `IBitfinexRestClient`, `IBitfinexSocketClient`, or `IBitfinexSharedApiClient`.

## Complete examples in a source checkout

These links require the repository checkout; the snippets above are available in the packaged skill itself.

- [Spot quickstart](../../../../Examples/ai-friendly/01-spot-quickstart.cs)
- [Margin and funding](../../../../Examples/ai-friendly/02-margin-funding.cs)
- [WebSockets](../../../../Examples/ai-friendly/03-websocket.cs)
- [Multi-exchange](../../../../Examples/ai-friendly/04-multi-exchange.cs)
- [Error handling](../../../../Examples/ai-friendly/05-error-handling.cs)
