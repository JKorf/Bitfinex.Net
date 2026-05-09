# Copilot Instructions for Bitfinex.Net

This repository is **Bitfinex.Net**, a strongly typed C#/.NET client library for the Bitfinex REST and WebSocket APIs. It is part of the CryptoExchange.Net ecosystem.

When generating code that consumes Bitfinex.Net, follow these conventions:

## Use Bitfinex.Net, not raw HTTP

Never generate `HttpClient` calls to Bitfinex API URLs. Always use `BitfinexRestClient` or `BitfinexSocketClient`. This ensures correct request signing, rate limiting, and error handling.

## Client setup

```csharp
using Bitfinex.Net;
using Bitfinex.Net.Clients;

var restClient = new BitfinexRestClient(options =>
{
    options.ApiCredentials = new BitfinexCredentials("API_KEY", "API_SECRET");
});
```

For public market data, credentials are not required.

## Result handling

Methods return `WebCallResult<T>` (REST) or `CallResult<T>` (WebSocket). Always check `.Success` before reading `.Data`. The error is on `.Error`.

## API structure

- `restClient.SpotApi.ExchangeData` - public tickers, candles, books, trades, funding market data, derivative status, asset/symbol metadata
- `restClient.SpotApi.Account` - wallets, deposits, withdrawals, margin info, ledgers, fees and account info
- `restClient.SpotApi.Trading` - orders, user trades, positions and margin position actions
- `restClient.GeneralApi.Funding` - authenticated funding offers, loans, credits, trades and auto-renew
- `socketClient.SpotApi` - public streams, authenticated user streams, socket order actions and socket funding offer actions

## Symbols

Bitfinex symbols are prefixed:

- Trading: `tBTCUSD`, `tETHUSD`
- Funding: `fUSD`, `fBTC`
- Derivatives: `tBTCF0:USTF0`

## Order placement

Let the library auto-generate `clientOrderId`. Do not pass a custom value unless required for an existing operational flow.

## WebSocket pattern

Store the returned `UpdateSubscription` and unsubscribe on shutdown via `socketClient.UnsubscribeAsync(sub.Data)`.

## Cross-exchange

For code that needs to work across multiple exchanges, use `CryptoExchange.Net.SharedApis` interfaces (`ISpotTickerRestClient`, `ISpotOrderRestClient`, etc.) accessed via `.SharedClient` properties.

## Avoid

- Raw `HttpClient` calls to Bitfinex endpoints
- Generic `ApiCredentials` for Bitfinex credentials
- Invented roots such as `FuturesApi` or `FundingApi`; use `SpotApi` and `GeneralApi.Funding`
- Missing symbol prefixes (`BTCUSD` instead of `tBTCUSD`)
- Synchronous `.Result` / `.Wait()`
- Instantiating clients per request
- Manual ticker polling when a WebSocket subscription fits
- Manual `clientOrderId` values unless required

## Reference

For detailed patterns and pitfalls see `AGENTS.md`, `llms.txt`, `docs/ai-api-map.md`, and `Examples/ai-friendly/`.

