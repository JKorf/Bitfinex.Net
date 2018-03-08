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
```C#
using(var client = new BitfinexClient())
{
	var priceResult = client.GetTicker("tBTCETH");
	if (priceResult.Success)
		Console.WriteLine($"BTC-ETH price: {priceResult.Data.Last}");
	else
		Console.WriteLine($"Error: {priceResult.Error}");
}
```

## Options & Authentication
The default behavior of the clients can be changed by providing options to the constructor, or using the `SetDefaultOptions` before creating a new client to set options for all new clients. Api credentials can be provided in the options.

## Websocket
To use the websocket the various Subscribe methods can be used. The `Start` method should be used to actually connect the websocket. When connection to the websocket has been made the server will send various snapshots of data, so make sure your event handlers are set up before starting.

TODO


## Release notes

* Version 0.0.1 - 
	* Initial release