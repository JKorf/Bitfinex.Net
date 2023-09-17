---
title: IBitfinexRestClientSpotApiTrading
has_children: false
parent: IBitfinexRestClientSpotApi
grand_parent: Rest API documentation
---
*[generated documentation]*  
`BitfinexRestClient > SpotApi > Trading`  
*Bitfinex trading endpoints, placing and mananging orders.*
  

***

## CancelOrderAsync  

[https://docs.bitfinex.com/reference#rest-auth-cancel-order](https://docs.bitfinex.com/reference#rest-auth-cancel-order)  
<p>

*Cancel a specific order*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.CancelOrderAsync();  
```  

```csharp  
Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> CancelOrderAsync(long? orderId = default, long? clientOrderId = default, DateTime? clientOrderIdDate = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ orderId|The id of the order to cancel|
|_[Optional]_ clientOrderId|The client order id of the order to cancel|
|_[Optional]_ clientOrderIdDate|The date of the client order (year month and day)|
|_[Optional]_ ct|Cancellation token|

</p>

***

## CancelOrdersAsync  

[https://docs.bitfinex.com/reference/rest-auth-cancel-orders-multiple](https://docs.bitfinex.com/reference/rest-auth-cancel-orders-multiple)  
<p>

*Cancels open orders*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.CancelOrdersAsync();  
```  

```csharp  
Task<WebCallResult<BitfinexWriteResult<IEnumerable<BitfinexOrder>>>> CancelOrdersAsync(IEnumerable<long>? orderIds = default, IEnumerable<long>? groupIds = default, Dictionary<long,DateTime>? clientOrderIds = default, bool? all = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ orderIds|Ids of orders to cancel|
|_[Optional]_ groupIds|Group ids to cancel|
|_[Optional]_ clientOrderIds|Client order ids to cancel|
|_[Optional]_ all|Cancel all orders|
|_[Optional]_ ct|Cancellation token|

</p>

***

## ClaimPositionAsync  

[https://docs.bitfinex.com/reference/rest-auth-position-claim](https://docs.bitfinex.com/reference/rest-auth-position-claim)  
<p>

*The claim feature allows the use of funds you have in your Margin Wallet to settle a leveraged position as an exchange buy or sale*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.ClaimPositionAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexWriteResult<BitfinexPosition>>> ClaimPositionAsync(long id, decimal quantity, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|id|The id of the position to claim|
|quantity|The (partial) quantity to be claimed|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetClosedOrdersAsync  

[https://docs.bitfinex.com/reference#rest-auth-orders-history](https://docs.bitfinex.com/reference#rest-auth-orders-history)  
<p>

*Get the order history for a symbol for this account*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.GetClosedOrdersAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetClosedOrdersAsync(string? symbol = default, IEnumerable<long>? orderIds = default, DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbol|The symbol to get the history for|
|_[Optional]_ orderIds|Filter by specific order ids|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetIncreasePositionInfoAsync  

[https://docs.bitfinex.com/reference/rest-auth-increase-position-info](https://docs.bitfinex.com/reference/rest-auth-increase-position-info)  
<p>

*Returns information relevant to the increase position endpoint.*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.GetIncreasePositionInfoAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexIncreasePositionInfo>> GetIncreasePositionInfoAsync(string symbol, decimal quantity, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol|
|quantity|Quantity|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOpenOrdersAsync  

[https://docs.bitfinex.com/reference#rest-auth-orders](https://docs.bitfinex.com/reference#rest-auth-orders)  
<p>

*Get the active orders*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.GetOpenOrdersAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetOpenOrdersAsync(string? symbol = default, IEnumerable<long>? orderIds = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbol|Filter by symbol|
|_[Optional]_ orderIds|Filter by specific order ids|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOrderTradesAsync  

[https://docs.bitfinex.com/reference#rest-auth-order-trades](https://docs.bitfinex.com/reference#rest-auth-order-trades)  
<p>

*Get the individual trades for an order*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.GetOrderTradesAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexTradeDetails>>> GetOrderTradesAsync(string symbol, long orderId, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol of the order|
|orderId|The order Id|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetPositionHistoryAsync  

[https://docs.bitfinex.com/reference#rest-auth-positions-hist](https://docs.bitfinex.com/reference#rest-auth-positions-hist)  
<p>

*Get a list of historical positions*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.GetPositionHistoryAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetPositionHistoryAsync(DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetPositionsAsync  

[https://docs.bitfinex.com/reference#rest-auth-positions](https://docs.bitfinex.com/reference#rest-auth-positions)  
<p>

*Get the active positions*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.GetPositionsAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetPositionsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetPositionsByIdAsync  

[https://docs.bitfinex.com/reference#rest-auth-positions-audit](https://docs.bitfinex.com/reference#rest-auth-positions-audit)  
<p>

*Get positions by id*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.GetPositionsByIdAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetPositionsByIdAsync(IEnumerable<string> ids, DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|ids|The id's of positions to return|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetPositionSnapshotsAsync  

[https://docs.bitfinex.com/reference/rest-auth-positions-snap](https://docs.bitfinex.com/reference/rest-auth-positions-snap)  
<p>

*Returns position snapshots of user positions between the specified start and end perimiters. Snapshots are taken daily.*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.GetPositionSnapshotsAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetPositionSnapshotsAsync(DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetUserTradesAsync  

[https://docs.bitfinex.com/reference#rest-auth-trades](https://docs.bitfinex.com/reference#rest-auth-trades)  
<p>

*Get the trade history for a symbol*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.GetUserTradesAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexTradeDetails>>> GetUserTradesAsync(string? symbol = default, DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbol|The symbol to get history for|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## IncreasePositionAsync  

[https://docs.bitfinex.com/reference/rest-auth-position-increase](https://docs.bitfinex.com/reference/rest-auth-position-increase)  
<p>

*Essentially a reverse of the Claim Position feature, the Increase Position feature allows you to create a new position using the funds in your margin wallet*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.IncreasePositionAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexWriteResult<BitfinexPositionBasic>>> IncreasePositionAsync(string symbol, decimal quantity, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol|
|quantity|Quantity|
|_[Optional]_ ct|Cancellation token|

</p>

***

## PlaceOrderAsync  

[https://docs.bitfinex.com/reference#rest-auth-submit-order](https://docs.bitfinex.com/reference#rest-auth-submit-order)  
<p>

*Place a new order*  

```csharp  
var client = new BitfinexRestClient();  
var result = await client.SpotApi.Trading.PlaceOrderAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> PlaceOrderAsync(string symbol, OrderSide side, OrderType type, decimal quantity, decimal price, OrderFlags? flags = default, int? leverage = default, int? groupId = default, int? clientOrderId = default, decimal? priceTrailing = default, decimal? priceAuxLimit = default, decimal? priceOcoStop = default, DateTime? cancelTime = default, string? affiliateCode = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|Symbol to place order for|
|side|Side of the order|
|type|Type of the order|
|quantity|The quantity of the order|
|price|The price for the order|
|_[Optional]_ flags||
|_[Optional]_ leverage|Set the leverage for a derivative order, supported by derivative symbol orders only. The value should be between 1 and 100 inclusive. The field is optional, if omitted the default leverage value of 10 will be used.|
|_[Optional]_ groupId|Group id|
|_[Optional]_ clientOrderId|Client order id|
|_[Optional]_ priceTrailing|The trailing price for a trailing stop order|
|_[Optional]_ priceAuxLimit|Auxiliary Limit price (for STOP LIMIT)|
|_[Optional]_ priceOcoStop|OCO stop price|
|_[Optional]_ cancelTime|datetime for automatic order cancelation|
|_[Optional]_ affiliateCode|Affiliate code for the order|
|_[Optional]_ ct|Cancellation token|

</p>
