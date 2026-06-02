using Bitfinex.Net;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;

const string symbol = "tETHUST";

// Replace with valid credentials or order placement will always fail
var apiKey = "APIKEY";
var apiSecret = "APISECRET";

Console.WriteLine("Bitfinex.Net order placement example");
Console.WriteLine();
Console.WriteLine("This example can place a real order when valid credentials are configured.");
Console.WriteLine();

var client = new BitfinexRestClient(options =>
{
    options.ApiCredentials = new BitfinexCredentials(apiKey, apiSecret);
});

await PlaceSpotLimitOrderAsync(client);

static async Task PlaceSpotLimitOrderAsync(BitfinexRestClient client)
{
    Console.WriteLine($"Placing spot limit buy order for {symbol}...");

    var ticker = await client.SpotApi.ExchangeData.GetTickersAsync(new[] { symbol });
    if (!ticker.Success)
    {
        Console.WriteLine($"Failed to get ticker: {ticker.Error}");
        return;
    }

    var safePrice = Math.Round(ticker.Data.Single().LastPrice * 0.95m, 2);
    var order = await client.SpotApi.Trading.PlaceOrderAsync(
        symbol: symbol,
        side: OrderSide.Buy,
        type: OrderType.ExchangeLimit,
        quantity: 0.01m,
        price: safePrice);

    if (!order.Success)
    {
        Console.WriteLine($"Failed to place order: {order.Error}");
        return;
    }

    Console.WriteLine($"Placed order {order.Data.Data.Id}, status: {order.Data.Data.Status}");

    var openOrders = await client.SpotApi.Trading.GetOpenOrdersAsync(symbol, new[] { order.Data.Data.Id });
    if (openOrders.Success)
        Console.WriteLine($"Open order status: {openOrders.Data.SingleOrDefault()?.StatusString ?? "not open"}");
    else
        Console.WriteLine($"Failed to query open order: {openOrders.Error}");

    var cancel = await client.SpotApi.Trading.CancelOrderAsync(order.Data.Data.Id);
    Console.WriteLine(cancel.Success
        ? $"Cancelled order {order.Data.Data.Id}"
        : $"Failed to cancel order: {cancel.Error}");
}
