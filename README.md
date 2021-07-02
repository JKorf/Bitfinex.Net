# Bitfinex.Net
![Build status](https://travis-ci.org/JKorf/Bitfinex.Net.svg?branch=master) ![Nuget version](https://img.shields.io/nuget/v/bitfinex.net.svg)  ![Nuget downloads](https://img.shields.io/nuget/dt/Bitfinex.Net.svg)

Bitfinex.Net is a wrapper around the Bitfinex API as described on [Bitfinex](https://docs.bitfinex.com/docs), including all features the API provides using clear and readable objects, both for the REST  as the websocket API's.

**If you think something is broken, something is missing or have any questions, please open an [Issue](https://github.com/JKorf/Bitfinex.Net/issues)**

## CryptoExchange.Net
This library is build upon the CryptoExchange.Net library, make sure to check out the documentation on that for basic usage: [docs](https://github.com/JKorf/CryptoExchange.Net)

## Donations
I develop and maintain this package on my own for free in my spare time. Donations are greatly appreciated. If you prefer to donate any other currency please contact me.

**Btc**:  12KwZk3r2Y3JZ2uMULcjqqBvXmpDwjhhQS  
**Eth**:  0x069176ca1a4b1d6e0b7901a6bc0dbf3bb0bf5cc2  
**Nano**: xrb_1ocs3hbp561ef76eoctjwg85w5ugr8wgimkj8mfhoyqbx4s1pbc74zggw7gs  

## Discord
A Discord server is available [here](https://discord.gg/MSpeEtSY8t). Feel free to join for discussion and/or questions around the CryptoExchange.Net and implementation libraries.

## Getting started
Make sure you have installed the Bitfinex.Net [Nuget](https://www.nuget.org/packages/Bitfinex.Net/) package and add `using Bitfinex.Net` to your usings.  You now have access to 2 clients:  
**BitfinexClient**  
The client to interact with the Bitfinex REST API. Getting prices:
````C#
var client = new BitfinexClient(new BitfinexClientOptions(){
 // Specify options for the client
});
var callResult = await client.GetTickerAsync();
// Make sure to check if the call was successful
if(!callResult.Success)
{
  // Call failed, check callResult.Error for more info
}
else
{
  // Call succeeded, callResult.Data will have the resulting data
}
````

Placing an order:
````C#
var client = new BitfinexClient(new BitfinexClientOptions(){
 // Specify options for the client
 ApiCredentials = new ApiCredentials("Key", "Secret")
});
var callResult = await client.PlaceOrderAsync("BTCUSDT", OrderSide.Buy, OrderType.Limit, 50, 10);
// Make sure to check if the call was successful
if(!callResult.Success)
{
  // Call failed, check callResult.Error for more info
}
else
{
  // Call succeeded, callResult.Data will have the resulting data
}
````

**BitfinexSocketClient**  
The client to interact with the Bitfinex websocket API. Basic usage:
````C#
var client = new BitfinexSocketClient(new BitfinexSocketClientOptions()
{
  // Specify options for the client
});
var subscribeResult = client.SubscribeToTickerUpdatesAsync("tETHBTC", data => {
  // Handle data when it is received
});
// Make sure to check if the subscritpion was successful
if(!subscribeResult.Success)
{
  // Subscription failed, check callResult.Error for more info
}
else
{
  // Subscription succeeded, the handler will start receiving data when it is available
}
````

## Client options
For the basic client options see also the CryptoExchange.Net [docs](https://github.com/JKorf/CryptoExchange.Net#client-options). The here listed options are the options specific for Bitfinex.Net.
**BitfinexClientOptions**
| Property | Description | Default |
| ----------- | ----------- | ---------|
|`AffiliateCode`|The affiliate code to use for requests|kCCe-CNBO

**BitfinexSocketClientOptions**  
| Property | Description | Default |
| ----------- | ----------- | ---------|
|`AffiliateCode`|The affiliate code to use for requests|kCCe-CNBO

## Release notes
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

* Version 3.4.6 - 28 apr 2021
    * Inverted bid/ask when requesting order book for funding symbol
    * Updated CryptoExchange.Net

* Version 3.4.5 - 20 apr 2021
    * Fixed NullReference exception when unsubscribing auth subscription on the socket client

* Version 3.4.4 - 19 apr 2021
    * Updated CryptoExchange.Net

* Version 3.4.3 - 08 apr 2021
    * Allow GetMovements without parameters

* Version 3.4.2 - 30 mrt 2021
    * Updated CryptoExchange.Net

* Version 3.4.1 - 01 mrt 2021
    * Added Nuget SymbolPackage

* Version 3.4.0 - 01 mrt 2021
    * Added config for deterministic build
    * Updated CryptoExchange.Net

* Version 3.3.1 - 24 feb 2021
    * Added category filter to GetLedgerEntries

* Version 3.3.0 - 09 feb 2021
    * Fixed cancelationDate hours serialization on PlaceOrder
    * Made symbol optional for GetTradeHistory, GetLedgerHistory and GetOrderHistory

* Version 3.2.0 - 03 feb 2021
    * Updated PlaceOrder method to V2

* Version 3.2.0 - 03 feb 2021
    * Updated PlaceOrder method to V2

* Version 3.1.2 - 22 jan 2021
    * Updated for ICommonKline

* Version 3.1.1 - 14 jan 2021
    * Updated CryptoExchange.Net

* Version 3.1.0 - 21 dec 2020
    * Update CryptoExchange.Net
    * Updated to latest IExchangeClient

* Version 3.0.18 - 11 dec 2020
    * Updated CryptoExchange.Net
    * Implemented IExchangeClient

* Version 3.0.17 - 19 nov 2020
    * Updated CryptoExchange.Net

* Version 3.0.16 - 08 Oct 2020
    * Updated CryptoExchange.Net

* Version 3.0.15 - 28 Aug 2020
    * Updated CryptoExchange.Net

* Version 3.0.14 - 12 Aug 2020
    * Updated CryptoExchange.Net

* Version 3.0.13 - 20 Jul 2020
    * Added affiliate code to models and new socket order
    * Fix price serialization new rest order

* Version 3.0.12 - 20 Jun 2020
	* Added GetRawOrderBook
	* Fix for FillOrKill parsing
	* Added affiliate code support on placing order

* Version 3.0.11 - 16 Jun 2020
    * Fix for BitfinexSymbolOrderBook

* Version 3.0.10 - 07 Jun 2020
    * Fixed issue where the BitfinexSymbolOrderBook desynced 

* Version 3.0.9 - 03 Mar 2020
    * Fixed symbol regex allowing numbers

* Version 3.0.8 - 03 Mar 2020
    * Updated CryptoExchange

* Version 3.0.7 - 05 Feb 2020
    * Fixed reconnection on 20051 status sometimes losing operations

* Version 3.0.6 - 27 Jan 2020
    * Updated CryptoExchange.Net

* Version 3.0.5 - 19 Nov 2019
    * Fixed symbol validation for dusk

* Version 3.0.4 - 19 Nov 2019
    * Fixed authenticated websocket subscriptions

* Version 3.0.3 - 15 Nov 2019
    * Fixed confirmations websocket order placing/updating/cancelation
	
* Version 3.0.2 - 12 Nov 2019
    * Fixed Trailing-Stop order type, fixed ClaimPosition parameter

* Version 3.0.1 - 24 Oct 2019
	* Fixed validation in PlaceOrder

* Version 3.0.0 - 23 Oct 2019
	* See CryptoExchange.Net 3.0 release notes
	* Added input validation
	* Added CancellationToken support to all requests
	* Now using IEnumerable<> for collections
	* Renamed Candle -> Kline
	* Renamed Market -> Symbol
	* Renamed GetWallets -> GetBalances

* Version 2.1.6 - 07 Aug 2019
    * Fixed threadsafety in subscription response mapping

* Version 2.1.5 - 06 Aug 2019
    * Fixed missing events when placing orders via websocket
    * Added code docs

* Version 2.1.4 - 05 Aug 2019
    * Fix offer serialization
    * Added missing xml file for code docs

* Version 2.1.3 - 09 jul 2019
	* Updated BitfinexSymbolOrderBook

* Version 2.1.2 - 08 jul 2019
	* Added option to pass client to BitfinexSymbolOrderBook to combine subscriptions on sockets

* Version 2.1.1 - 01 jul 2019
	* Fixed position parsing
	* Added stopLimitPrice parameter to PlaceOrder Rest call for StopLimitOrders	

* Version 2.1.0 - 27 jun 2019
	* Added multiple missing REST calls
	* Added property documentations
	* Fixed parallel subscribing on websocket
	* Fixed GetAvailableBalance call
	* Fixed a bug in error parsing

* Version 2.0.12 - 14 may 2019
	* Added an order book implementation for easily keeping an updated order book
	* Added additional constructor to ApiCredentials to be able to read from file

* Version 2.0.11 - 01 may 2019
	* Updated to latest CryptoExchange.Net
		* Adds response header to REST call result
		* Added rate limiter per API key
		* Unified socket client workings	

* Version 2.0.10 - 18 mar 2019
	* Fix for order status parsing

* Version 2.0.9 - 08 mar 2019
	* Changed rest client returns from CallResult to WebCallResult
	* Revert checkin of experimental code resulting in api key invalid errors

* Version 2.0.8 - 07 mar 2019
	* Updated CryptoExchange.Net

* Version 2.0.7 - 18 feb 2019
	* Updated CryptoExchange.Net
	* Adjusted Nonce calculation

* Version 2.0.6 - 01 feb 2019
	* Fixed result parsing of placed trailing stop orders
	* Fixed invalid cid when placing order with socket client
	* Fixed parameters in update order

* Version 2.0.5 - 09 jan 2019
	* Changed withdraw type from enum to string
	* Updated CryptoExchange.net

* Version 2.0.4 - 17 dec 2018
	* Fixed status fields when serializing
	* Fixed reconnecting sometimes throwing an error
	* Fixed error after event timeout in socket client

* Version 2.0.3 - 10 dec 2018
	* Fix for status field serialization in BitfinexOrder object

* Version 2.0.2 - 06 dec 2018
	* Fix CryptoExchange reference

* Version 2.0.1 - 06 dec 2018
	* Fixed freezes if called from the UI thread	

* Version 2.0.0 - 05 dec 2018
	* Updated to CryptoExchange.Net version 2
		* Libraries now use the same standard functionalities
		* Objects returned by socket subscriptions standardized across libraries

* Version 1.2.6 - 30 okt 2018
	* Fixed error message handling during subscription
	* Fixed reconnect trying to resubscribe failed subscriptions

* Version 1.2.5 - 18 okt 2018
	* Fixed handling of confirmation preventing missed messages

* Version 1.2.4 - 21 sep 2018
	* Updated CryptoExchange.Net

* Version 1.2.3 - 19 sep 2018
	* Fix for exception during reconnecting if socket hangs in connecting state

* Version 1.2.2 - 07 sep 2018
	* Fix for reconnecting
	* Fix for resubscribing always reporting successful when socket disconnects in the mean time
	* Added error message for duplicate subscriptions

* Version 1.2.1 - 21 aug 2018
	* Changed SubscribeToTradeUpdates update data to BitfinexSocketEvent<BitfinexTradeSimple[]> to be able to distinguish execution and update messages

* Version 1.2.0 - 21 aug 2018
	* Refactored connecting/reconnecting
	* Fixed bug disposing default credentials

* Version 1.1.8 - 20 aug 2018
	* Updated CryptoExchange.Net to fix disposed socket issue

* Version 1.1.7 - 16 aug 2018
	* Added client interfaces
	* Fixed some resharper warnings
	* Updated CryptoExchange.Net

* Version 1.1.6 - 15 aug 2018
	* Fixed subscribe calls always reporting success
	* Made resubscribing after lost connection more robust
	* Added reconnect interval setting

* Version 1.1.5 - 13 aug 2018
	* Added CancelOrders methods to socket client
	* Updated CryptoExchange.Net

* Version 1.1.4 - 30 jul 2018
	* Changed order confirmation matching to use ClientOrderId instead of trying to match based on amount/price/symbol and type

* Version 1.1.3 - 24 jul 2018
	* Fix for mixup in TradeUpdate for user trades and for symbol trades
	* Fix for small decimal parsing

* Version 1.1.2 - 20 jul 2018
	* Fix for resubscribing after lost connection

* Version 1.1.1 - 18 jul 2018
	* Fix for subscription data handling

* Version 1.1.0 - 16 jul 2018
	* Refactored multiple subscriptions to expose the EventType that caused the update

* Version 1.0.19 - 12 jul 2018
	* Fixed limit parameters in various rest calls
	* Fixed response handling of canceled FillOrKill orders

* Version 1.0.18 - 03 jul 2018
	* Added StopLimit margin ordertype
	* Fixed bug restarting socketclient after stopping

* Version 1.0.17 - 28 jun 2018
	* Fixed margin order confirmation

* Version 1.0.16 - 26 jun 2018
	* Fix for OrderStatus mapping

* Version 1.0.15 - 26 jun 2018
	* Improvided OrderStatus mapping
	* Added OrderStatusString for original order status string returned by server

* Version 1.0.14 - 25 jun 2018
	* Fix for error parsing nullable ordertype
	* Fix for subscriptions not getting executed if called before Start in socket client

* Version 1.0.13 - 22 jun 2018
	* Added symbol mapping for GetTrades call
	* Refactored socketclient to be more robust
	* Added additional logging

* Version 1.0.12 - 11 jun 2018
	* Fix for subscribing to trades updates
	* Fix for PlaceOrder not returning the placed order in socket client
	* Added retry to (re)subscribing

* Version 1.0.11 - 06 jun 2018
	* Added timeout options
	* Added locking for threadsafety
	* Improved subscribe symbol interpretation

* Version 1.0.10 - 05 jun 2018
	* Fix for TradeDetails mapping

* Version 1.0.9 - 04 jun 2018
	* Added GetSymbols and GetSymbolDetails calls
	* Added GetAvailableBalance call
	* Added UpdateOrder call
	* Added ConfigureAwaits to prevent freezes from UI thread
	* Added waiting for authentication in socket client
	
* Version 1.0.8 - 12 may 2018
	* Fix for new order parameter in socket client

* Version 1.0.7 - 12 may 2018
	* Added StopLimitOrder ordertype

* Version 1.0.6 - 08 may 2018
	* Fixed bug with orderstatus parsing

* Version 1.0.5 - 07 may 2018
	* Fix for logging issue

* Version 1.0.4 - 07 may 2018
	* Added optional parameters to place order

* Version 1.0.3 - 04 may 2018
	* Fix for freezing from UI-thread with authenticated requests
	* Fix for position deserialization

* Version 1.0.2 - 20 apr 2018
	* Fix for ticker stream data deserialization

* Version 1.0.1 - 20 apr 2018
	* Fix for OrderStatus parsing
	* Fix for int's being used when should be long
	* Fix swapped parameters in socket order
	
* Version 1.0.0 - 19 apr 2018
	* Added raw orderbook subscription
	* Fix for GetCandles limit parameter
	* Fix for V1 Api error parsing
	* Fix for nonce for authenticated requests

* Version 0.0.8 - 23 mar 2018
	* Updated socket dispose
	* Updated base

* Version 0.0.7 - 23 mar 2018
	* Added notification message handling resulting in faster error message when failing to place/cancel orders via websocket
	* Fix for unsubsribing exception

* Version 0.0.6 - 22 mar 2018
	* Fix for order status parsing

* Version 0.0.5 - 21 mar 2018
	* Can add multiple log writers
	* Added some code docs

* Version 0.0.4 - 20 mar 2018
	* Fix for precision serialization

* Version 0.0.3 - 12 mar 2018
	* Fixes for freezes when calling from UI thread

* Version 0.0.2 - 09 mar 2018 
	* Auto reconnecting socket client
	* Info messages send from server processing
	* Socket client status events added

* Version 0.0.1 - 08 mar 2018
	* Initial release
