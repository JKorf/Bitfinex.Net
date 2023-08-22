# Bitfinex.Net
[![.NET](https://github.com/JKorf/Bitfinex.Net/actions/workflows/dotnet.yml/badge.svg)](https://github.com/JKorf/Bitfinex.Net/actions/workflows/dotnet.yml) ![Nuget version](https://img.shields.io/nuget/v/bitfinex.net.svg)  ![Nuget downloads](https://img.shields.io/nuget/dt/Bitfinex.Net.svg)

Bitfinex.Net is a wrapper around the Bitfinex API as described on [Bitfinex](https://docs.bitfinex.com/docs), including all features the API provides using clear and readable objects, both for the REST  as the websocket API's.

**If you think something is broken, something is missing or have any questions, please open an [Issue](https://github.com/JKorf/Bitfinex.Net/issues)**

[Documentation](https://jkorf.github.io/Bitfinex.Net/)

## Support the project
I develop and maintain this package on my own for free in my spare time, any support is greatly appreciated.

### Referral link
Sign up using the following referral link to pay a small percentage of the trading fees you pay to support the project instead of paying them straight to Bitfinex. This doesn't cost you a thing!
[Link](https://www.bitfinex.com/sign-up?refcode=kCCe-CNBO)

### Donate
Make a one time donation in a crypto currency of your choice. If you prefer to donate a currency not listed here please contact me.

**Btc**:  bc1qz0jv0my7fc60rxeupr23e75x95qmlq6489n8gh  
**Eth**:  0x8E21C4d955975cB645589745ac0c46ECA8FAE504  

### Sponsor
Alternatively, sponsor me on Github using [Github Sponsors](https://github.com/sponsors/JKorf).

## Discord
A Discord server is available [here](https://discord.gg/MSpeEtSY8t). Feel free to join for discussion and/or questions around the CryptoExchange.Net and implementation libraries.

## Release notes
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

