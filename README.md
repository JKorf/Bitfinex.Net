# ![.Bitfinex.Net](https://github.com/JKorf/Bitfinex.Net/blob/master/Bitfinex.Net/Icon/icon.png?raw=true) Bitfinex.Net

[![.NET](https://img.shields.io/github/actions/workflow/status/JKorf/Bitfinex.Net/dotnet.yml?style=for-the-badge)](https://github.com/JKorf/Bitfinex.Net/actions/workflows/dotnet.yml) ![License](https://img.shields.io/github/license/JKorf/Bitfinex.Net?style=for-the-badge)

Bitfinex.Net is a strongly typed client library for accessing the [Bitfinex REST and Websocket API](https://docs.bitfinex.com/docs). All data is mapped to readable models and enum values. Additional features include an implementation for maintaining a client side order book, easy integration with other exchange client libraries and more.

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

## Get the library
[![Nuget version](https://img.shields.io/nuget/v/bitfinex.net.svg?style=for-the-badge)](https://www.nuget.org/packages/Bitfinex.Net)  [![Nuget downloads](https://img.shields.io/nuget/dt/Bitfinex.Net.svg?style=for-the-badge)](https://www.nuget.org/packages/Bitfinex.Net)

	dotnet add package Bitfinex.Net

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

CryptoExchange.Net also allows for [easy access to different exchange API's](https://jkorf.github.io/CryptoExchange.Net#idocs_common).

|Exchange|Repository|Nuget|
|--|--|--|
|Binance|[JKorf/Binance.Net](https://github.com/JKorf/Binance.Net)|[![Nuget version](https://img.shields.io/nuget/v/Binance.net.svg?style=flat-square)](https://www.nuget.org/packages/Binance.Net)|
|BingX|[JKorf/BingX.Net](https://github.com/JKorf/BingX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.BingX.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.BingX.Net)|
|Bitget|[JKorf/Bitget.Net](https://github.com/JKorf/Bitget.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.Bitget.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.Bitget.Net)|
|Bybit|[JKorf/Bybit.Net](https://github.com/JKorf/Bybit.Net)|[![Nuget version](https://img.shields.io/nuget/v/Bybit.net.svg?style=flat-square)](https://www.nuget.org/packages/Bybit.Net)|
|CoinEx|[JKorf/CoinEx.Net](https://github.com/JKorf/CoinEx.Net)|[![Nuget version](https://img.shields.io/nuget/v/CoinEx.net.svg?style=flat-square)](https://www.nuget.org/packages/CoinEx.Net)|
|CoinGecko|[JKorf/CoinGecko.Net](https://github.com/JKorf/CoinGecko.Net)|[![Nuget version](https://img.shields.io/nuget/v/CoinGecko.net.svg?style=flat-square)](https://www.nuget.org/packages/CoinGecko.Net)|
|Huobi/HTX|[JKorf/Huobi.Net](https://github.com/JKorf/Huobi.Net)|[![Nuget version](https://img.shields.io/nuget/v/Huobi.net.svg?style=flat-square)](https://www.nuget.org/packages/Huobi.Net)|
|Kraken|[JKorf/Kraken.Net](https://github.com/JKorf/Kraken.Net)|[![Nuget version](https://img.shields.io/nuget/v/KrakenExchange.net.svg?style=flat-square)](https://www.nuget.org/packages/KrakenExchange.Net)|
|Kucoin|[JKorf/Kucoin.Net](https://github.com/JKorf/Kucoin.Net)|[![Nuget version](https://img.shields.io/nuget/v/Kucoin.net.svg?style=flat-square)](https://www.nuget.org/packages/Kucoin.Net)|
|Mexc|[JKorf/Mexc.Net](https://github.com/JKorf/Mexc.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.Mexc.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.Mexc.Net)|
|OKX|[JKorf/OKX.Net](https://github.com/JKorf/OKX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.OKX.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.OKX.Net)|

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
I develop and maintain this package on my own for free in my spare time, any support is greatly appreciated.

### Donate
Make a one time donation in a crypto currency of your choice. If you prefer to donate a currency not listed here please contact me.

**Btc**:  bc1q277a5n54s2l2mzlu778ef7lpkwhjhyvghuv8qf  
**Eth**:  0xcb1b63aCF9fef2755eBf4a0506250074496Ad5b7   
**USDT (TRX)**  TKigKeJPXZYyMVDgMyXxMf17MWYia92Rjd

### Sponsor
Alternatively, sponsor me on Github using [Github Sponsors](https://github.com/sponsors/JKorf).

## Release notes
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

