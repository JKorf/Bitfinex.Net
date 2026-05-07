// 03-websocket.cs
//
// Demonstrates: Bitfinex public websocket subscriptions.
//
// Setup: dotnet add package JKorf.Bitfinex.Net

using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;

var socketClient = new BitfinexSocketClient();

var tickerSubscription = await socketClient.SpotApi.SubscribeToTickerUpdatesAsync(
    "tBTCUSD",
    update =>
    {
        Console.WriteLine($"BTC/USD ticker: {update.Data.LastPrice} volume={update.Data.Volume}");
    });

if (!tickerSubscription.Success)
{
    Console.WriteLine($"Ticker subscription failed: {tickerSubscription.Error}");
    return;
}

var klineSubscription = await socketClient.SpotApi.SubscribeToKlineUpdatesAsync(
    "tETHUSD",
    KlineInterval.OneMinute,
    update =>
    {
        var candle = update.Data.LastOrDefault();
        if (candle == null)
            return;

        Console.WriteLine($"ETH/USD 1m candle: open={candle.OpenPrice}, close={candle.ClosePrice}");
    });

if (!klineSubscription.Success)
{
    Console.WriteLine($"Kline subscription failed: {klineSubscription.Error}");
    await socketClient.UnsubscribeAsync(tickerSubscription.Data);
    return;
}

Console.WriteLine("Listening. Press Enter to unsubscribe.");
Console.ReadLine();

await socketClient.UnsubscribeAsync(tickerSubscription.Data);
await socketClient.UnsubscribeAsync(klineSubscription.Data);
