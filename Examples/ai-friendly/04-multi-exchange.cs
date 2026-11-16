// 04-multi-exchange.cs
//
// Demonstrates: writing exchange-agnostic code using CryptoExchange.Net.SharedApis.
// Same code works against Bitfinex and other exchanges from the CryptoExchange.Net family.
//
// Setup:
//   dotnet add package Bitfinex.Net
//   dotnet add package Binance.Net  // optional, for comparison

using Bitfinex.Net.Clients;
using CryptoExchange.Net.SharedApis;

// SharedApi exposes capabilities for individual operations.
using var bitfinexRest = new BitfinexRestClient();
IGetTickerRest bitfinexShared = bitfinexRest.ExchangeApi.SharedApi;

var sharedInfo = bitfinexRest.ExchangeApi.SharedApi.Discover();
var supportedFeatures = sharedInfo.Features
    .Where(x => x.Supported)
    .Select(x => x.EndpointName);
Console.WriteLine($"{sharedInfo.Exchange} {sharedInfo.TypeName}: {string.Join(", ", supportedFeatures)}");

var btcusd = new SharedSymbol(TradingMode.Spot, "BTC", "USD");

await PrintTicker(bitfinexShared, btcusd);

async Task PrintTicker(IGetTickerRest client, SharedSymbol symbol)
{
    var result = await client.GetTickerAsync(new GetTickerRequest(symbol));
    if (!result.Success)
    {
        Console.WriteLine($"[{client.Exchange}] Failed: {result.Error}");
        return;
    }

    Console.WriteLine($"[{client.Exchange}] {result.Data.Symbol}: {result.Data.LastPrice}");
}

// Other capabilities include IGetSpotSymbolsRest, IPlaceSpotOrderRest,
// IPlaceFuturesOrderRest, IGetBalancesRest, and IGetOrderBookRest.
// Use the exchange aggregate for runtime-selected capability lookup.

// ---- WEBSOCKET EXAMPLE - SHARED SUBSCRIPTION ----
using var bitfinexSocket = new BitfinexSocketClient();
ISubscribeTickerSocket bitfinexTickerSocket = bitfinexSocket.ExchangeApi.SharedApi;

var sub = await bitfinexTickerSocket.SubscribeToTickerUpdatesAsync(
    new SubscribeTickerRequest(btcusd),
    update => Console.WriteLine($"[{bitfinexTickerSocket.Exchange}] {update.Data.Symbol}: {update.Data.LastPrice}"));

if (!sub.Success)
{
    Console.WriteLine($"Subscribe failed: {sub.Error}");
    return;
}

try
{
    Console.WriteLine("Press Enter to exit");
    Console.ReadLine();
}
finally
{
    await bitfinexSocket.UnsubscribeAsync(sub.Data);
}

// Note: shared socket interfaces do not expose UnsubscribeAsync.
// Keep the concrete socket client and call concreteClient.UnsubscribeAsync(sub.Data).
