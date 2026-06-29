// 05-error-handling.cs
//
// Demonstrates: HttpResult, WebSocketResult, ExchangeCallResult, retry logic,
// and common error scenarios.
//
// Setup: dotnet add package JKorf.Bitfinex.Net

using Bitfinex.Net;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using CryptoExchange.Net.Objects;

var client = new BitfinexRestClient(options =>
{
    options.ApiCredentials = new BitfinexCredentials("API_KEY", "API_SECRET");
});

// ---- 1. THE BASIC PATTERN ----
// Every REST method returns HttpResult<T> or HttpResult.
// WebSocket subscriptions return WebSocketResult<UpdateSubscription>.
// Shared non-I/O symbol/cache helpers return ExchangeCallResult<T>.
// .Success is true/false. .Data is valid only when .Success is true.
// .Error contains structured error info when .Success is false.

var result = await client.ExchangeApi.ExchangeData.GetTickerAsync("tBTCUSD");

if (result.Success)
{
    Console.WriteLine($"Price: {result.Data.LastPrice}");
}
else
{
    Console.WriteLine($"Code:      {result.Error?.Code}");
    Console.WriteLine($"Message:   {result.Error?.Message}");
    Console.WriteLine($"Type:      {result.Error?.ErrorType}");
    Console.WriteLine($"Transient: {result.Error?.IsTransient}");
}

// ---- 2. SIMPLE RETRY WITH BACKOFF ----
// Retry only on transient errors such as rate limits, network issues, or server overload.
// Do not retry validation errors or insufficient balance errors.

async Task<HttpResult<T>> WithRetry<T>(
    Func<Task<HttpResult<T>>> call,
    int maxAttempts = 3)
{
    HttpResult<T> last = default!;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        last = await call();
        if (last.Success) return last;
        if (last.Error?.IsTransient != true) return last;

        await Task.Delay(TimeSpan.FromMilliseconds(250 * Math.Pow(2, attempt)));
    }
    return last;
}

var ticker = await WithRetry(
    () => client.ExchangeApi.ExchangeData.GetTickerAsync("tBTCUSD"));

if (!ticker.Success)
{
    Console.WriteLine($"Ticker still failed after retry: {ticker.Error}");
}

// ---- 3. ORDER PLACEMENT WITH RESULT CATEGORIZATION ----
var order = await client.ExchangeApi.Trading.PlaceOrderAsync(
    symbol: "tBTCUSD",
    side: OrderSide.Buy,
    type: OrderType.ExchangeLimit,
    quantity: 0.001m,
    price: 1m);

if (!order.Success)
{
    var category = order.Error?.IsTransient == true
        ? "Transient - retry may be appropriate"
        : "Permanent - surface to user";

    Console.WriteLine($"{category}: {order.Error?.Code} {order.Error?.Message}");
}

// ---- 4. EXCEPTIONS VS ERROR RESULTS ----
// Bitfinex.Net returns API/network/rate-limit errors via HttpResult.Error.
// Exceptions are for misconfiguration, disposal, cancellation, or programmer errors.

// Common variations:
//   With CancellationToken:       pass `ct: cancellationToken` to any method
//   With timeout per request:     options.RequestTimeout = TimeSpan.FromSeconds(10)
//   Margin positions:            client.ExchangeApi.Trading.GetPositionsAsync()
//   Funding management:          client.GeneralApi.Funding.*
