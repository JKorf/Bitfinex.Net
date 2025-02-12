# ![.Bitfinex.Net](https://github.com/JKorf/Bitfinex.Net/blob/master/Bitfinex.Net/Icon/icon.png?raw=true) Bitfinex.Net

[![.NET](https://img.shields.io/github/actions/workflow/status/JKorf/Bitfinex.Net/dotnet.yml?style=for-the-badge)](https://github.com/JKorf/Bitfinex.Net/actions/workflows/dotnet.yml) ![License](https://img.shields.io/github/license/JKorf/Bitfinex.Net?style=for-the-badge)

Bitfinex.Net is a strongly typed client library for accessing the [Bitfinex REST and Websocket API](https://docs.bitfinex.com/docs).

## Features
* Response data is mapped to descriptive models
* Input parameters and response values are mapped to discriptive enum values where possible
* Automatic websocket (re)connection management 
* Client side order book implementation
* Extensive logging
* Support for different environments
* Easy integration with other exchange client based on the CryptoExchange.Net base library

## Supported Frameworks
The library is targeting both `.NET Standard 2.0` and `.NET Standard 2.1` for optimal compatibility

|.NET implementation|Version Support|
|--|--|
|.NET Core|`2.0` and higher|
|.NET Framework|`4.6.1` and higher|
|Mono|`5.4` and higher|
|Xamarin.iOS|`10.14` and higher|
|Xamarin.Android|`8.0` and higher|
|UWP|`10.0.16299` and higher|
|Unity|`2018.1` and higher|

## Install the library

### NuGet 
[![NuGet version](https://img.shields.io/nuget/v/Bitfinex.net.svg?style=for-the-badge)](https://www.nuget.org/packages/Bitfinex.Net)  [![Nuget downloads](https://img.shields.io/nuget/dt/Bitfinex.Net.svg?style=for-the-badge)](https://www.nuget.org/packages/Bitfinex.Net)

	dotnet add package Bitfinex.Net
	
### GitHub packages
Bitfinex.Net is available on [GitHub packages](https://github.com/JKorf/Bitfinex.Net/pkgs/nuget/Bitfinex.Net). You'll need to add `https://nuget.pkg.github.com/JKorf/index.json` as a NuGet package source.

### Download release
[![GitHub Release](https://img.shields.io/github/v/release/JKorf/Bitfinex.Net?style=for-the-badge&label=GitHub)](https://github.com/JKorf/Bitfinex.Net/releases)

The NuGet package files are added along side the source with the latest GitHub release which can found [here](https://github.com/JKorf/Bitfinex.Net/releases).

## How to use
*REST Endpoints*  

```csharp
// Get the ETH/USDT ticker via rest request
var restClient = new BitfinexRestClient();
var tickerResult = await restClient.SpotApi.ExchangeData.GetTickerAsync("tETHUST");
var lastPrice = tickerResult.Data.LastPrice;
```

*Websocket streams*  

```csharp
// Subscribe to ETH/USDT ticker updates via the websocket API
var socketClient = new BitfinexSocketClient();
var tickerSubscriptionResult = socketClient.SpotApi.SubscribeToTickerUpdatesAsync("tETHUST", (update) => 
{
  var lastPrice = update.Data.LastPrice;
});
```

For information on the clients, dependency injection, response processing and more see the [Bitfinex.Net documentation](https://jkorf.github.io/Bitfinex.Net), [CryptoExchange.Net documentation](https://jkorf.github.io/CryptoExchange.Net), or have a look at the examples [here](https://github.com/JKorf/Bitfinex.Net/tree/master/Examples) or [here](https://github.com/JKorf/CryptoExchange.Net/tree/master/Examples).

## CryptoExchange.Net
Bitfinex.Net is based on the [CryptoExchange.Net](https://github.com/JKorf/CryptoExchange.Net) base library. Other exchange API implementations based on the CryptoExchange.Net base library are available and follow the same logic.

CryptoExchange.Net also allows for [easy access to different exchange API's](https://jkorf.github.io/CryptoExchange.Net#idocs_shared).

|Exchange|Repository|Nuget|
|--|--|--|
|Binance|[JKorf/Binance.Net](https://github.com/JKorf/Binance.Net)|[![Nuget version](https://img.shields.io/nuget/v/Binance.net.svg?style=flat-square)](https://www.nuget.org/packages/Binance.Net)|
|BingX|[JKorf/BingX.Net](https://github.com/JKorf/BingX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.BingX.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.BingX.Net)|
|Bitget|[JKorf/Bitget.Net](https://github.com/JKorf/Bitget.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.Bitget.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.Bitget.Net)|
|BitMart|[JKorf/BitMart.Net](https://github.com/JKorf/BitMart.Net)|[![Nuget version](https://img.shields.io/nuget/v/BitMart.net.svg?style=flat-square)](https://www.nuget.org/packages/BitMart.Net)|
|BitMEX|[JKorf/BitMEX.Net](https://github.com/JKorf/BitMEX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.BitMEX.net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.BitMEX.Net)|
|Bybit|[JKorf/Bybit.Net](https://github.com/JKorf/Bybit.Net)|[![Nuget version](https://img.shields.io/nuget/v/Bybit.net.svg?style=flat-square)](https://www.nuget.org/packages/Bybit.Net)|
|Coinbase|[JKorf/Coinbase.Net](https://github.com/JKorf/Coinbase.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.Coinbase.Net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.Coinbase.Net)|
|CoinEx|[JKorf/CoinEx.Net](https://github.com/JKorf/CoinEx.Net)|[![Nuget version](https://img.shields.io/nuget/v/CoinEx.net.svg?style=flat-square)](https://www.nuget.org/packages/CoinEx.Net)|
|CoinGecko|[JKorf/CoinGecko.Net](https://github.com/JKorf/CoinGecko.Net)|[![Nuget version](https://img.shields.io/nuget/v/CoinGecko.net.svg?style=flat-square)](https://www.nuget.org/packages/CoinGecko.Net)|
|Crypto.com|[JKorf/CryptoCom.Net](https://github.com/JKorf/CryptoCom.Net)|[![Nuget version](https://img.shields.io/nuget/v/CryptoCom.net.svg?style=flat-square)](https://www.nuget.org/packages/CryptoCom.Net)|
|Gate.io|[JKorf/GateIo.Net](https://github.com/JKorf/GateIo.Net)|[![Nuget version](https://img.shields.io/nuget/v/GateIo.net.svg?style=flat-square)](https://www.nuget.org/packages/GateIo.Net)|
|HTX|[JKorf/HTX.Net](https://github.com/JKorf/HTX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.HTX.Net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.HTX.Net)|
|HyperLiquid|[JKorf/HyperLiquid.Net](https://github.com/JKorf/HyperLiquid.Net)|[![Nuget version](https://img.shields.io/nuget/v/HyperLiquid.Net.svg?style=flat-square)](https://www.nuget.org/packages/HyperLiquid.Net)|
|Kraken|[JKorf/Kraken.Net](https://github.com/JKorf/Kraken.Net)|[![Nuget version](https://img.shields.io/nuget/v/KrakenExchange.net.svg?style=flat-square)](https://www.nuget.org/packages/KrakenExchange.Net)|
|Kucoin|[JKorf/Kucoin.Net](https://github.com/JKorf/Kucoin.Net)|[![Nuget version](https://img.shields.io/nuget/v/Kucoin.net.svg?style=flat-square)](https://www.nuget.org/packages/Kucoin.Net)|
|Mexc|[JKorf/Mexc.Net](https://github.com/JKorf/Mexc.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.Mexc.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.Mexc.Net)|
|OKX|[JKorf/OKX.Net](https://github.com/JKorf/OKX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.OKX.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.OKX.Net)|
|WhiteBit|[JKorf/WhiteBit.Net](https://github.com/JKorf/WhiteBit.Net)|[![Nuget version](https://img.shields.io/nuget/v/WhiteBit.net.svg?style=flat-square)](https://www.nuget.org/packages/WhiteBit.Net)|
|XT|[JKorf/XT.Net](https://github.com/JKorf/XT.Net)|[![Nuget version](https://img.shields.io/nuget/v/XT.net.svg?style=flat-square)](https://www.nuget.org/packages/XT.Net)|

## Discord
[![Nuget version](https://img.shields.io/discord/847020490588422145?style=for-the-badge)](https://discord.gg/MSpeEtSY8t)  
A Discord server is available [here](https://discord.gg/MSpeEtSY8t). Feel free to join for discussion and/or questions around the CryptoExchange.Net and implementation libraries.

## Supported functionality
|API|Supported|Location|
|--|--:|--|
|Rest Public Endpoints|✓|`restClient.SpotApi.ExchangeData`|
|Rest Public Pulse Endpoints|X||
|Calculation Endpoints|✓|`restClient.SpotApi.ExchangeData`|
|Rest Authenticated Endpoints|✓|`restClient.SpotApi.Account` / `restClient.SpotApi.Trading`|
|Rest Authenticated Pulse Endpoints|X||
|Rest Authenticated Merchant Endpoints|X||
|Websocket Public Channels|✓|`socketClient.SpotApi`|
|Websocket Authenticated Channels|✓|`socketClient.SpotApi`|
|Websocket Authenticated Inputs|✓|`socketClient.SpotApi`|

## Support the project
Any support is greatly appreciated.

### Donate
Make a one time donation in a crypto currency of your choice. If you prefer to donate a currency not listed here please contact me.

**Btc**:  bc1q277a5n54s2l2mzlu778ef7lpkwhjhyvghuv8qf  
**Eth**:  0xcb1b63aCF9fef2755eBf4a0506250074496Ad5b7   
**USDT (TRX)**  TKigKeJPXZYyMVDgMyXxMf17MWYia92Rjd

### Sponsor
Alternatively, sponsor me on Github using [Github Sponsors](https://github.com/sponsors/JKorf).

## Release notes
* Version 8.1.1 - 12 Feb 2025
    * Fixed SharedSymbol formatting for assets with more than 3 characters in the name
    * Fixed unnecessary unsubscribe call when subscribe is never confirmed

* Version 8.1.0 - 11 Feb 2025
    * Updated CryptoExchange.Net to version 8.8.0, see https://github.com/JKorf/CryptoExchange.Net/releases/
    * Added support for more SharedKlineInterval values
    * Added setting of DataTime value on websocket DataEvent updates
    * Fix Mono runtime exception on rest client construction using DI

* Version 8.0.2 - 06 Feb 2025
    * Fixed deserialization error in BitfinexOrder response model

* Version 8.0.1 - 05 Feb 2025
    * Updated CryptoExchange.Net to version 8.7.3 to fix serialization error, see https://github.com/JKorf/CryptoExchange.Net/releases/

* Version 8.0.0 - 27 Jan 2025
    * Added client side ratelimit implementation
    * Updated library to System.Text.Json from Newtonsoft.Json for json (de)serialization
    * Updated CryptoExchange.Net to version 8.7.2, see https://github.com/JKorf/CryptoExchange.Net/releases/
    * Updated public websocket subscriptions to use the URI specifically for public data
    * Updated websocket to use bulk updates for order book updates (configurable)
    * Updated Enum conversions to use EnumConverter

* Version 7.13.1 - 07 Jan 2025
    * Updated CryptoExchange.Net version
    * Added Type property to BitfinexExchange class

* Version 7.13.0 - 23 Dec 2024
    * Updated CryptoExchange.Net to version 8.5.0, see https://github.com/JKorf/CryptoExchange.Net/releases/
    * Added SetOptions methods on Rest and Socket clients
    * Added setting of DefaultProxyCredentials to CredentialCache.DefaultCredentials on the DI http client

* Version 7.12.2 - 05 Dec 2024
    * Fixed Shared balance subscription also sending funding balances

* Version 7.12.1 - 03 Dec 2024
    * Updated CryptoExchange.Net to version 8.4.3, see https://github.com/JKorf/CryptoExchange.Net/releases/
    * Fixed orderbook creation via BitfinexOrderBookFactory

* Version 7.12.0 - 28 Nov 2024
    * Updated CryptoExchange.Net to version 8.4.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.4.0
    * Added GetFeesAsync Shared REST client implementations
    * Updated BitfinexOptions to LibraryOptions implementation
    * Updated test and analyzer package versions

* Version 7.11.0 - 19 Nov 2024
    * Updated CryptoExchange.Net to version 8.3.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.3.0
    * Added support for loading client settings from IConfiguration
    * Added DI registration method for configuring Rest and Socket options at the same time
    * Added DisplayName and ImageUrl properties to BitfinexExchange class
    * Updated client constructors to accept IOptions from DI
    * Removed redundant BitfinexSocketClient constructor

* Version 7.10.0 - 06 Nov 2024
    * Updated CryptoExchange.Net to version 8.2.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.2.0

* Version 7.9.0 - 28 Oct 2024
    * Updated CryptoExchange.Net to version 8.1.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.1.0
    * Moved FormatSymbol to BitfinexExchange class
    * Added support Side setting on SharedTrade model
    * Added BitfinexTrackerFactory for creating trackers
    * Added overload to Create method on BitfinexOrderBookFactory support SharedSymbol parameter
    * Added filtering of TradeUpdate messages in Shared rest trade socket subscription (Trade execution messages are still processed)

* Version 7.8.2 - 14 Oct 2024
    * Updated CryptoExchange.Net to version 8.0.3, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.0.3
    * Fixed TypeLoadException during initialization

* Version 7.8.1 - 08 Oct 2024
    * Limit shared interface GetBalancesAsync result to Exchange balances to prevent duplicate asset balances

* Version 7.8.0 - 27 Sep 2024
    * Updated CryptoExchange.Net to version 8.0.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.0.0
    * Added Shared client interfaces implementation for Spot Rest and Socket clients
    * Added SpotApi.Account.WithdrawV2Async endpoint
    * Updated SpotApi.ExchangeData.GetTradeHistoryAsync limit parameter max value from 5000 to 10000
    * Updated Sourcelink package version
    * Marked ISpotClient references as deprecated

* Version 7.7.0 - 07 Aug 2024
    * Updated CryptoExchange.Net to version 7.11.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.11.0
    * Updated XML code comments
    * Fixed BitfinexPosition model Type enum being nullable

* Version 7.6.0 - 27 Jul 2024
    * Updated CryptoExchange.Net to version 7.10.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.10.0
    * Fixed stats endpoints last stats by splitting endpoints into Last and History variants

* Version 7.5.0 - 16 Jul 2024
    * Updated CryptoExchange.Net to version 7.9.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.9.0
    * Updated internal classes to internal access modifier
    * Added check allowing subscribing to the same websocket stream multiple times

* Version 7.4.3 - 02 Jul 2024
    * Updated CryptoExchange.Net to V7.8.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.8.0

* Version 7.4.2 - 26 Jun 2024
    * Updated CryptoExchange.Net to 7.7.3, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.7.3
    * Fixed SpotApi.Account.GetMovementsDetailsAsync deserialization
    * Fixed SpotApi.SubscribeToDerivativesUpdatesAsync subscription
    * Fixed funding info subscription

* Version 7.4.1 - 25 Jun 2024
    * Updated CryptoExchange.Net to 7.7.2, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.7.2
    * Fixed SpotApi.ExchangeData.GetLiquidationsAsync deserializations

* Version 7.4.0 - 23 Jun 2024
    * Updated CryptoExchange.Net to version 7.7.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.7.0
    * Updated response models from classes to records
    * Fixed exception during order status parsing

* Version 7.3.0 - 11 Jun 2024
    * Updated CryptoExchange.Net to v7.6.0, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.2.8 - 07 May 2024
    * Updated CryptoExchange.Net to v7.5.2, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.2.7 - 01 May 2024
    * Updated CryptoExchange.Net to v7.5.0, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.2.6 - 28 Apr 2024
    * Added BitfinexExchange static info class
    * Added BitfinexOrderBookFactory book creation method
    * Fixed BitfinexOrderBookFactory injection issue
    * Updated CryptoExchange.Net to v7.4.0, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.2.5 - 23 Apr 2024
    * Updated CryptoExchange.Net to 7.3.3, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.2.4 - 18 Apr 2024
    * Updated CryptoExchange.Net to 7.3.1, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.2.3 - 03 Apr 2024
    * Updated string comparison for improved performance
    * Removed pre-send symbol validation

* Version 7.2.2 - 25 Mar 2024
    * Fixed parameter serialization websocket queries
    * Added missing info websocket user data updates

* Version 7.2.1 - 24 Mar 2024
	* Updated CryptoExchange.Net to 7.2.0, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes
	* Fixed Topic not set in socket client SubscribeToKlineUpdatesAsync updates

* Version 7.2.0 - 16 Mar 2024
    * Updated CryptoExchange.Net to 7.1.0, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes
	
* Version 7.1.0 - 25 Feb 2024
    * Updated CryptoExchange.Net and implemented reworked websocket message handling. For release notes for the CryptoExchange.Net base library see: https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes
    * Combined multiple private websocket subscriptions into single subscription
    * Fixed issue in DI registration causing http client to not be correctly injected
    * Removed redundant BitfinexRestClient constructor overload
    * Removed UpdateType from BitfinexTradeSimple model in favor of the UpdateType in the DataEvent wrapper
    * Updated some namespaces

* Version 7.0.5 - 03 Dec 2023
    * Updated CryptoExchange.Net

* Version 7.0.4 - 24 Oct 2023
    * Updated CryptoExchange.Net

* Version 7.0.3 - 09 Oct 2023
    * Updated CryptoExchange.Net version

* Version 7.0.2 - 21 Sep 2023
    * Fixed incorrect url SpotApi.Account.WalletTransferAsync

* Version 7.0.1 - 19 Sep 2023
    * Fixed socket ticker stream model

* Version 7.0.0 - 17 Sep 2023
    * Updated all endpoints to V2 API (expect for Account.WithdrawAsync)
    * Updated all parameters and response models
    * Created endpoints for all possible requests for the Configs and Stats endpoints
    * Split the endpoints and subscriptions for trading symbols and funding symbols where necesarry
    * Added Account.GetMovementsDetailsAsync endpoint
    * Added Account.GetLoginHistoryAsync endpoint
    * Added Account.GetApiKeyPermissionsAsync endpoint
    * Added Account.GetAccountChangeLogAsync endpoint
    * Added ExchangeData.GetDerivativesStatusAsync endpoint
    * Added ExchangeData.GetLiquidationsAsync endpoint
    * Added ExchangeData.GetFundingStatisticsAsync endpoint
    * Added Trading.IncreasePositionAsync endpoint
    * Added Trading.GetIncreasePositionInfoAsync endpoint
    * Added Trading.GetPositionSnapshotsAsync endpoint
    * Added SubscribeToLiquidationUpdatesAsync stream
    * Added SubscribeToDerivativesUpdatesAsync stream
    * Added SubmitFundingOfferAsync socket request
    * Added CancelFundingOfferAsync socket request

* Version 6.0.1 - 25 Aug 2023
    * Updated CryptoExchange.Net

* Version 6.0.0 - 25 Jun 2023
    * Updated CryptoExchange.Net to version 6.0.0
    * Renamed BitfinexClient to BitfinexRestClient
    * Renamed SpotStreams to SpotApi on the BitfinexSocketClient
    * Updated endpoints to consistently use a base url without any path as basis to make switching environments/base urls clearer
    * Added IBitfinexOrderBookFactory and implementation for creating order books
    * Updated dependency injection register method (AddBitfinex)

* Version 5.2.0 - 16 Apr 2023
    * Added GetTickerHistoryAsync endpoint
    * Updated socket PlaceOrderAsync parameters
    * Added missing order flag values

* Version 5.1.2 - 18 Mar 2023
    * Updated CryptoExchange.Net

* Version 5.1.1 - 14 Feb 2023
    * Updated CryptoExchange.Net

* Version 5.1.0 - 17 Nov 2022
    * Updated CryptoExchange.Net

* Version 5.0.16 - 25 Sep 2022
    * Fixed SpotApi.ExchangeData.GetOrderBookAsync() endpoint Bids/Asks being reversed

* Version 5.0.15 - 18 Jul 2022
    * Updated CryptoExchange.Net

* Version 5.0.14 - 16 Jul 2022
    * Updated CryptoExchange.Net

* Version 5.0.13 - 10 Jul 2022
    * Fixed WalletTransferAsync success checking
    * Updated CryptoExchange.Net

* Version 5.0.12 - 12 Jun 2022
    * Updated CryptoExchange.Net

* Version 5.0.11 - 24 May 2022
    * Updated CryptoExchange.Net

* Version 5.0.10 - 22 May 2022
    * Updated CryptoExchange.Net

* Version 5.0.9 - 08 May 2022
    * Updated CryptoExchange.Net

* Version 5.0.8 - 01 May 2022
    * Updated CryptoExchange.Net which fixed an timing related issue in the websocket reconnection logic
    * Added seconds representation to KlineInterval enum
    * Added allowed order book levels 1 and 250

* Version 5.0.7 - 14 Apr 2022
    * Fixed deserialization error in ClosePositionAsync
    * Fixed WalletTransferAsync serialization issue
    * Updated CryptoExchange.Net

* Version 5.0.6 - 14 Mar 2022
    * Added SubmitFundingAutoRenewAsync endpoint
    * Added GetFundingAutoRenewStatusAsync endpoint

* Version 5.0.5 - 10 Mar 2022
    * Updated CryptoExchange.Net

* Version 5.0.4 - 08 Mar 2022
    * Updated CryptoExchange.Net

* Version 5.0.3 - 01 Mar 2022
    * Updated CryptoExchange.Net improving the websocket reconnection robustness

* Version 5.0.2 - 27 Feb 2022
    * Updated CryptoExchange.Net

* Version 5.0.1 - 24 Feb 2022
    * Updated CryptoExchange.Net

* Version 5.0.0 - 18 Feb 2022
    * Added Github.io page for documentation: https://jkorf.github.io/Bitfinex.Net/
    * Added unit tests for parsing the returned JSON for each endpoint and subscription
    * Added AddBitfinex extension method on IServiceCollection for easy dependency injection
    * Added URL reference to API endpoint documentation for each endpoint
    
	* Refactored client structure to be consistent across exchange implementations
    * Renamed various properties to be consistent across exchange implementations

    * Cleaned up project structure
    * Fixed various models

    * Updated CryptoExchange.Net, see https://github.com/JKorf/CryptoExchange.Net#release-notes
    * See https://jkorf.github.io/Bitfinex.Net/MigrationGuide.html for additional notes for updating from V4 to V5

* Version 4.2.4 - 03 Nov 2021
    * Fixed raw order book stream not accounting for checksum updates

* Version 4.2.3 - 08 Oct 2021
    * Updated CryptoExchange.Net to fix some socket issues

* Version 4.2.2 - 06 Oct 2021
    * Updated CryptoExchange.Net, fixing socket issue when calling from .Net Framework

* Version 4.2.1 - 05 Oct 2021
    * Updated CryptoExchange.Net

* Version 4.2.0 - 29 Sep 2021
    * Split GetTickerAsync in GetTickerAsync and GetTickersAsync, changed `params` to `IEnumerable`
    * Updated CryptoExchange.Net

* Version 4.1.2 - 22 Sep 2021
    * Fixed nonce provider when running multiple program instances

* Version 4.1.1 - 21 Sep 2021
    * Fix for nonce provider not working correctly in combination with other exchanges

* Version 4.1.0 - 20 Sep 2021
    * Added custom nonce provider support

* Version 4.0.5 - 15 Sep 2021
    * Updated CryptoExchange.Net

* Version 4.0.4 - 02 Sep 2021
    * Fix for disposing order book closing socket even if there are other connections

* Version 4.0.3 - 26 Aug 2021
    * Updated CryptoExchange.Net

* Version 4.0.2 - 24 Aug 2021
    * Updated CryptoExchange.Net, improving websocket and SymbolOrderBook performance
    * Fixes for checksum validation BitfinexSymbolOrderBook

* Version 4.0.1 - 13 Aug 2021
    * Fix for OperationCancelledException being thrown when closing a socket from a .net framework project

* Version 4.0.0 - 12 Aug 2021
	* Release version with new CryptoExchange.Net version 4.0.0
		* Multiple changes regarding logging and socket connection, see [CryptoExchange.Net release notes](https://github.com/JKorf/CryptoExchange.Net#release-notes)

* Version 4.0.0-beta3 - 09 Aug 2021
    * Renamed GetTradesForOrderAsync to GetOrderTradesAsync
    * Renamed GetTradesAsync to GetTradeHistoryAsync
    * Renamed GetTradeHistoryAsync to GetUserTradesAsync
    * Renamed GetOrderHistoryAsync to GetOrdersAsync

* Version 4.0.0-beta2 - 26 Jul 2021
    * Updated CryptoExchange.Net

* Version 4.0.0-beta1 - 09 Jul 2021
    * Added Async postfix for async methods
    * Updated CryptoExchange.Net

* Version 3.5.0-beta4 - 07 Jun 2021
    * Fixed IExchangeClient PlaceOrder OrderType
    * Fixed WalletTransferAsync amount parameter serialization
    * Updated CryptoExchange.Net

* Version 3.5.0-beta3 - 26 May 2021
    * Removed non-async calls
    * Updated to CryptoExchange.Net changes

* Version 3.5.0-beta2 - 06 mei 2021
    * Updated CryptoExchange.Net

* Version 3.5.0-beta1 - 30 apr 2021
    * Updated to CryptoExchange.Net 4.0.0-beta1, new websocket implementation

