// 04-multi-exchange.cs
//
// Demonstrates: writing exchange-agnostic code using CryptoExchange.Net.SharedApis.
// Same code works against Bitfinex and other exchanges from the CryptoExchange.Net family.
//
// Setup:
//   dotnet add package JKorf.Bitfinex.Net
//   dotnet add package Binance.Net  // optional, for comparison

using Bitfinex.Net.Clients;
using CryptoExchange.Net.SharedApis;

// Bitfinex exposes a SharedClient on SpotApi. Shared symbols do not use Bitfinex t/f prefixes.
var bitfinexRest = new BitfinexRestClient();
ISpotTickerRestClient bitfinexShared = bitfinexRest.SpotApi.SharedClient;

var sharedInfo = bitfinexRest.SpotApi.SharedClient.Discover();
var supportedFeatures = sharedInfo.Features
    .Where(x => x.Supported)
    .Select(x => x.EndpointName);
Console.WriteLine($"{sharedInfo.Exchange} {sharedInfo.TypeName}: {string.Join(", ", supportedFeatures)}");

var btcusd = new SharedSymbol(TradingMode.Spot, "BTC", "USD");

await PrintTicker(bitfinexShared, btcusd);

async Task PrintTicker(ISpotTickerRestClient client, SharedSymbol symbol)
{
    var result = await client.GetSpotTickerAsync(new GetTickerRequest(symbol));
    if (!result.Success)
    {
        Console.WriteLine($"[{client.Exchange}] Failed: {result.Error}");
        return;
    }

    Console.WriteLine($"[{client.Exchange}] {result.Data.Symbol}: {result.Data.LastPrice}");
}

// Common REST shared interfaces:
//   ISpotTickerRestClient, ISpotSymbolRestClient, ISpotOrderRestClient
//   IBalanceRestClient, IPositionRestClient, IFeeRestClient
//   IOrderBookRestClient, IRecentTradeRestClient, IKlineRestClient
// Call SharedClient.Discover() before routing optional shared features.

// ---- WEBSOCKET EXAMPLE - SHARED SUBSCRIPTION ----
var bitfinexSocket = new BitfinexSocketClient();
ITickerSocketClient bitfinexTickerSocket = bitfinexSocket.SpotApi.SharedClient;

var sub = await bitfinexTickerSocket.SubscribeToTickerUpdatesAsync(
    new SubscribeTickerRequest(btcusd),
    update => Console.WriteLine($"[{bitfinexTickerSocket.Exchange}] {update.Data.Symbol}: {update.Data.LastPrice}"));

if (!sub.Success)
{
    Console.WriteLine($"Subscribe failed: {sub.Error}");
    return;
}

Console.WriteLine("Press Enter to exit");
Console.ReadLine();

await bitfinexSocket.UnsubscribeAsync(sub.Data);

// Note: shared socket interfaces do not expose UnsubscribeAsync.
// Keep the concrete socket client and call concreteClient.UnsubscribeAsync(sub.Data).
