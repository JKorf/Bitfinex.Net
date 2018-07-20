# ![Icon](https://github.com/JKorf/Bitfinex.Net/blob/master/Resources/icon.png?raw=true) Bitfinex.Net 

![Build status](https://travis-ci.org/JKorf/Bitfinex.Net.svg?branch=master)

Bitfinex.Net is a .Net wrapper for the Bitfinex API as described on [Bitfinex](https://docs.bitfinex.com/docs). It includes all features the API provides using clear and readable C# objects including 
* Reading market info
* Placing and managing orders
* Reading balances and funds
* Live updates using the websocket

Next to that it adds some convenience features like:
* Configurable rate limiting
* Autmatic logging

**If you think something is broken, something is missing or have any questions, please open an [Issue](https://github.com/JKorf/Bitfinex.Net/issues)**

---
Also check out my other exchange API wrappers:
<table>
<tr>
<td><img src="https://github.com/JKorf/Binance.Net/blob/master/Resources/binance-coin.png?raw=true">
<br />
<a href="https://github.com/JKorf/Binance.Net">Binance</a>
</td>
<td><img src="https://github.com/JKorf/Bittrex.Net/blob/master/Resources/icon.png?raw=true">
<br />
<a href="https://github.com/JKorf/Bittrex.Net">Bittrex</a></td>
</table>

## Donations
Donations are greatly appreciated and a motivation to keep improving.

**Btc**:  12KwZk3r2Y3JZ2uMULcjqqBvXmpDwjhhQS  
**Eth**:  0x069176ca1a4b1d6e0b7901a6bc0dbf3bb0bf5cc2  
**Nano**: xrb_1ocs3hbp561ef76eoctjwg85w5ugr8wgimkj8mfhoyqbx4s1pbc74zggw7gs  

## Installation
![Nuget version](https://img.shields.io/nuget/v/bitfinex.net.svg) ![Nuget downloads](https://img.shields.io/nuget/dt/Bitfinex.Net.svg)

Available on [NuGet](https://www.nuget.org/packages/Bitfinex.Net/):
```
PM> Install-Package Bitfinex.Net
```
To get started with Bitfinex.Net first you will need to get the library itself. The easiest way to do this is to install the package into your project using [NuGet](https://www.nuget.org/packages/Bitfinex.Net/). Using Visual Studio this can be done in two ways.

### Using the package manager
In Visual Studio right click on your solution and select 'Manage NuGet Packages for solution...'. A screen will appear which initially shows the currently installed packages. In the top bit select 'Browse'. This will let you download net package from the NuGet server. In the search box type 'Bitfinex.Net' and hit enter. The Bitfinex.Net package should come up in the results. After selecting the package you can then on the right hand side select in which projects in your solution the package should install. After you've selected all project you wish to install and use Bitfinex.Net in hit 'Install' and the package will be downloaded and added to you projects.

### Using the package manager console
In Visual Studio in the top menu select 'Tools' -> 'NuGet Package Manager' -> 'Package Manager Console'. This should open up a command line interface. On top of the interface there is a dropdown menu where you can select the Default Project. This is the project that Bitfinex.Net will be installed in. After selecting the correct project type  `Install-Package Bitfinex.Net`  in the command line interface. This should install the latest version of the package in your project.

After doing either of above steps you should now be ready to actually start using Bitfinex.Net.

## Getting started
After  it's time to actually use it. To get started we have to add the Bitfinex.Net namespace:  `using Bitfinex.Net;`.

Bitfinex.Net provides two clients to interact with the Bitfinex API. The `BitfinexClient` provides all rest API calls. The `BitfinexSocketClient` provides functions to interact with the websocket provided by the Bitfinex API.

Most API methods are available in two flavors, sync and async:
````C#
public void NonAsyncMethod()
{
    using(var client = new BitfinexClient())
    {
        var result = client.GetPlatformStatus();
    }
}

public async Task AsyncMethod()
{
    using(var client = new BitfinexClient())
    {
        var result2 = await client.GetPlatformStatusAsync();
    }
}
````

## Response handling
All API requests will respond with an CallResult object. This object contains whether the call was successful, the data returned from the call and an error if the call wasn't successful. As such, one should always check the Success flag when processing a response.
For example:
````C#
using(var client = new BitfinexClient())
{
	var priceResult = client.GetTicker("tBTCETH");
	if (priceResult.Success)
		Console.WriteLine($"BTC-ETH price: {priceResult.Data.Last}");
	else
		Console.WriteLine($"Error: {priceResult.Error}");
}
````

## Options & Authentication
The default behavior of the clients can be changed by providing options to the constructor, or using the `SetDefaultOptions` before creating a new client to set options for all new clients. Api credentials can be provided in the options.

## Websocket
To use the websocket the various Subscribe methods can be used. The `Start` method should be used to actually connect the websocket. When connection to the websocket has been made the server will send various snapshots of data, so make sure your event handlers are set up before starting.
The websocket will automatically handle reconnect/pause messages send from the server. To unsubscribe from a stream use the `UnsubscribeFromChannel` method with the stream id you received when subscribing. To stop the websocket completly the `Stop` method can be used.
````C#
var options = new BitfinexSocketClientOptions()
{
	ApiCredentials = new ApiCredentials("KEY", "SECRET")
};

var client = new BitfinexSocketClient(options);
var walletSub = client.SubscribeToWalletUpdates(data =>
{
	Console.WriteLine("Wallet update");
});
var tickerSub = await client.SubscribeToTicker("tBTCUSD", data =>
{
	Console.WriteLine("Ticker update");
});

var startOk = client.Start();
Console.WriteLine(startOk ? "Connected": "Connection failed");

Console.ReadLine();
await client.UnsubscribeFromChannel(walletSub.Data);
await client.UnsubscribeFromChannel(tickerSub.Data);
client.Stop();
````

The websocket also supports the placing and canceling of orders:
````C#
var options = new BitfinexSocketClientOptions()
{
	ApiCredentials = new ApiCredentials("KEY", "SECRET")
};

var client = new BitfinexSocketClient(options);
var startOk = client.Start();
Console.WriteLine(startOk ? "Connected": "Connection failed");

Console.ReadLine();

var placeResult = client.PlaceOrder(OrderType.ExchangeLimit, "tBTCUSD", 1, price: 1);
Console.WriteLine(placeResult.Success ? "order placed": "failed placing order: " + placeResult.Error);

if (placeResult.Success)
{
	var cancelResult = client.CancelOrder(placeResult.Data.Id);
	Console.WriteLine(cancelResult.Success ? "order canceled" : "failed canceling order: " + cancelResult.Error);
}

Console.ReadLine();
````


## Release notes
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