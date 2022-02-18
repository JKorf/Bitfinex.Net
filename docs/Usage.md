---
title: Getting started
nav_order: 2
---

## Creating client
There are 2 clients available to interact with the Bitfinex API, the `BitfinexClient` and `BitfinexSocketClient`.

*Create a new rest client*
```csharp
var bitfinexClient = new BitfinexClient(new BitfinexClientOptions()
{
	// Set options here for this client
});
```

*Create a new socket client*
```csharp
var bitfinexSocketClient = new BitfinexSocketClient(new BitfinexSocketClientOptions()
{
	// Set options here for this client
});
```

Different options are available to set on the clients, see this example
```csharp
var bitfinexClient = new BitfinexClient(new BitfinexClientOptions()
{
	ApiCredentials = new ApiCredentials("API-KEY", "API-SECRET"),
	LogLevel = LogLevel.Trace,
	RequestTimeout = TimeSpan.FromSeconds(60)
});
```
Alternatively, options can be provided before creating clients by using `SetDefaultOptions`:
```csharp
BitfinexClient.SetDefaultOptions(new BitfinexClientOptions{
	// Set options here for all new clients
});
var bitfinexClient = new BitfinexClient();
```
More info on the specific options can be found in the [CryptoExchange.Net documentation](https://jkorf.github.io/CryptoExchange.Net/Options.html)

### Dependency injection
See [CryptoExchange.Net documentation](https://jkorf.github.io/CryptoExchange.Net/Clients.html#dependency-injection)