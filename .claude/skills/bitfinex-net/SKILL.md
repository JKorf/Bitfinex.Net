---
name: bitfinex-net
description: Build C#/.NET integrations using Bitfinex.Net for Bitfinex spot, margin, derivatives, funding, REST, WebSockets, authentication, account operations, trading, and Shared APIs. Use when writing or updating code that consumes the library.
---

# Bitfinex.Net

Use `BitfinexRestClient` and `BitfinexSocketClient` for Bitfinex exchange access. They provide signing, rate limiting, result handling, and subscription management.

## Setup

```sh
dotnet add package Bitfinex.Net
```

Use the API provided by the installed package. Native market, account, and trading calls use `ExchangeApi`; authenticated REST funding uses `GeneralApi.Funding`. Do not invent `SpotApi`, `FuturesApi`, or top-level `FundingApi` roots.

## Consumer conventions

- Public market data needs no credentials. Private calls use `BitfinexCredentials` with an API key and secret; see [authentication](references/usage.md#authentication).
- Native trading symbols have a `t` prefix, such as `tBTCUSD`; funding symbols use `f`, such as `fUSD`. Derivatives use symbols such as `tBTCF0:USTF0`.
- Spot exchange-wallet orders use `OrderType.ExchangeLimit` or `ExchangeMarket`; non-exchange types such as `Limit` or `Market` are margin orders. Derivative leverage is an order parameter.
- REST calls return `HttpResult<T>` or `HttpResult`. Subscriptions return `WebSocketResult<UpdateSubscription>`; socket commands return `QueryResult<T>`. Shared non-I/O helpers can return `ExchangeCallResult<T>`. Check `Success` before accessing `Data`.
- REST write notifications often wrap the entity under `result.Data.Data`, unlike ticker or socket-command results; inspect the declared payload type.
- Reuse clients, await asynchronous methods, and pass cancellation tokens through the workflow. Keep socket callbacks short and unsubscribe through the concrete socket client on shutdown.
- Use symbol metadata to choose valid quantities and prices. Let the library generate client order IDs unless external correlation is needed.
- For portable multi-exchange code, use `ExchangeApi.SharedApi` and narrow capability interfaces. Use `IBitfinexSharedApiClient` for dynamic capability lookup across REST and socket implementations.

## Task references

Read only the references needed for the task:

- [API surfaces](references/api-surfaces.md): Client roots, symbol formats, endpoint routing, Shared API capabilities, and symbol catalogs.
- [Usage](references/usage.md): Authentication, results, wallets, trading, funding, subscriptions, and dependency injection examples.
- [Operational considerations](references/safety.md): Key permissions, wallet/order distinctions, funding offers, ambiguous outcomes, and retry decisions.
