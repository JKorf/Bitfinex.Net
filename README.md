# Bitfinex.Net
![Build status](https://travis-ci.com/JKorf/Bitfinex.Net.svg?branch=master) ![Nuget version](https://img.shields.io/nuget/v/bitfinex.net.svg)  ![Nuget downloads](https://img.shields.io/nuget/dt/Bitfinex.Net.svg)

Bitfinex.Net is a wrapper around the Bitfinex API as described on [Bitfinex](https://docs.bitfinex.com/docs), including all features the API provides using clear and readable objects, both for the REST  as the websocket API's.

**If you think something is broken, something is missing or have any questions, please open an [Issue](https://github.com/JKorf/Bitfinex.Net/issues)**

## CryptoExchange.Net
This library is build upon the CryptoExchange.Net library, make sure to check out the documentation on that for basic usage: [docs](https://github.com/JKorf/CryptoExchange.Net)

## Donate / Sponsor
I develop and maintain this package on my own for free in my spare time. Donations are greatly appreciated. If you prefer to donate any other currency please contact me.

**Btc**:  12KwZk3r2Y3JZ2uMULcjqqBvXmpDwjhhQS  
**Eth**:  0x069176ca1a4b1d6e0b7901a6bc0dbf3bb0bf5cc2  
**Nano**: xrb_1ocs3hbp561ef76eoctjwg85w5ugr8wgimkj8mfhoyqbx4s1pbc74zggw7gs  

Alternatively, sponsor me on Github using [Github Sponsors](https://github.com/sponsors/JKorf)  

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

