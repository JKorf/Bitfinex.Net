using Bitfinex.Net.Clients;

// REST
var restClient = new BitfinexRestClient();
var ticker = await restClient.SpotApi.ExchangeData.GetTickersAsync(new[] { "tETHUST" });
if (!ticker.Success)
{
    Console.WriteLine($"Failed to get ticker: {ticker.Error}");
    return;
}

Console.WriteLine($"Rest client ticker price for ETH-USDT: {ticker.Data.First().LastPrice}");

Console.WriteLine();
Console.WriteLine("Press enter to start websocket subscription");
Console.ReadLine();

// Websocket
var socketClient = new BitfinexSocketClient();
var subscription = await socketClient.SpotApi.SubscribeToTickerUpdatesAsync("tETHUST", update =>
{
    Console.WriteLine($"Websocket client ticker price for ETHUSDT: {update.Data.LastPrice}");
});

if (!subscription.Success)
{
    Console.WriteLine($"Failed to subscribe to ticker updates: {subscription.Error}");
    return;
}

Console.ReadLine();
