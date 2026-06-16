# Bitfinex.Net AI-friendly examples

These examples are compact, copyable console snippets for AI coding assistants and quick onboarding. They are compiled by the unit test suite so API names stay aligned with the project.

Use Bitfinex symbols as Bitfinex expects them:

- Trading pairs use a `t` prefix, for example `tBTCUSD`.
- Funding currencies use an `f` prefix, for example `fUSD`.
- Derivatives symbols keep the Bitfinex format, for example `tBTCF0:USTF0`.

Examples:

- `01-spot-quickstart.cs` - public market data, wallets, open orders and order placement.
- `02-margin-funding.cs` - margin positions plus public and authenticated funding APIs.
- `03-websocket.cs` - ticker and candle subscriptions.
- `04-multi-exchange.cs` - CryptoExchange.Net shared API usage.
- `05-error-handling.cs` - `HttpResult<T>` handling and retry shape.

Most REST calls return `HttpResult<T>`. Always check `.Success` before using `.Data`; use `.Error` for exchange, validation, network and rate-limit failures.
