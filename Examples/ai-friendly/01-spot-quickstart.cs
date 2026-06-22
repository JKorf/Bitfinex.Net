// 01-spot-quickstart.cs
//
// Demonstrates: Bitfinex spot market data, authenticated wallets and order flow.
//
// Setup: dotnet add package JKorf.Bitfinex.Net

using Bitfinex.Net;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;

var client = new BitfinexRestClient(options =>
{
    options.ApiCredentials = new BitfinexCredentials("API_KEY", "API_SECRET");
});

const string symbol = "tBTCUSD";

// ---- 1. PUBLIC MARKET DATA ----
var ticker = await client.ExchangeApi.ExchangeData.GetTickerAsync(symbol);
if (!ticker.Success)
{
    Console.WriteLine($"Ticker failed: {ticker.Error}");
    return;
}

Console.WriteLine($"{symbol} last price: {ticker.Data.LastPrice}");
Console.WriteLine($"{symbol} 24h volume: {ticker.Data.Volume}");

// ---- 2. AUTHENTICATED ACCOUNT DATA ----
var wallets = await client.ExchangeApi.Account.GetBalancesAsync();
if (!wallets.Success)
{
    Console.WriteLine($"Wallets failed: {wallets.Error}");
    return;
}

foreach (var wallet in wallets.Data.Where(x => x.Asset is "BTC" or "USD"))
{
    Console.WriteLine($"{wallet.Type} {wallet.Asset}: total={wallet.Total}, available={wallet.Available}");
}

// ---- 3. PLACE A SMALL EXCHANGE LIMIT ORDER ----
// Use ExchangeLimit for spot exchange wallet orders. Use Limit for margin orders.
var safePrice = Math.Max(ticker.Data.LastPrice * 0.5m, 1m);
var order = await client.ExchangeApi.Trading.PlaceOrderAsync(
    symbol: symbol,
    side: OrderSide.Buy,
    type: OrderType.ExchangeLimit,
    quantity: 0.001m,
    price: safePrice);

if (!order.Success)
{
    Console.WriteLine($"Order rejected: {order.Error}");
    return;
}

Console.WriteLine($"Placed order {order.Data.Data.Id}: {order.Data.Data.Status}");

// ---- 4. QUERY AND CANCEL OPEN ORDERS ----
var openOrders = await client.ExchangeApi.Trading.GetOpenOrdersAsync(symbol);
if (openOrders.Success)
{
    var matchingOrder = openOrders.Data.FirstOrDefault(x => x.Id == order.Data.Data.Id);
    Console.WriteLine(matchingOrder == null
        ? "Order is no longer open"
        : $"Open order remaining quantity: {matchingOrder.QuantityRemaining}");
}

var cancel = await client.ExchangeApi.Trading.CancelOrderAsync(orderId: order.Data.Data.Id);
Console.WriteLine(cancel.Success
    ? "Cancel request accepted"
    : $"Cancel failed: {cancel.Error}");
