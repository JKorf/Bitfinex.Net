---
title: IBitfinexClientSpotApiTrading
has_children: false
parent: IBitfinexClientSpotApi
grand_parent: Rest API documentation
---
*[generated documentation]*  
`BitfinexClient > SpotApi > Trading`  
*Bitfinex trading endpoints, placing and mananging orders.*
  

***

## CancelAllOrdersAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-cancel-all-orders](https://docs.bitfinex.com/v1/reference#rest-auth-cancel-all-orders)  
<p>

*Cancels all open orders*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Trading.CancelAllOrdersAsync();  
```  

```csharp  
Task<WebCallResult<BitfinexResult>> CancelAllOrdersAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token<returns></returns>|

</p>

***

## CancelOrderAsync  

[https://docs.bitfinex.com/reference#rest-auth-cancel-order](https://docs.bitfinex.com/reference#rest-auth-cancel-order)  
<p>

*Cancel a specific order*  

```csharp  
var client = new BitfinexClient();  
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

## ClaimPositionAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-claim-position](https://docs.bitfinex.com/v1/reference#rest-auth-claim-position)  
<p>

*Claim a position*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Trading.ClaimPositionAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexPositionV1>> ClaimPositionAsync(long id, decimal quantity, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|id|The id of the position to claim|
|quantity|The (partial) quantity to be claimed|
|_[Optional]_ ct|Cancellation token|

</p>

***

## ClosePositionAsync  

[https://docs.bitfinex.com/v1/reference#close-position](https://docs.bitfinex.com/v1/reference#close-position)  
<p>

*Close a position*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Trading.ClosePositionAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexClosePositionResult>> ClosePositionAsync(long positionId, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|positionId|The id to close|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetActivePositionsAsync  

[https://docs.bitfinex.com/reference#rest-auth-positions](https://docs.bitfinex.com/reference#rest-auth-positions)  
<p>

*Get the active positions*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Trading.GetActivePositionsAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexPosition>>> GetActivePositionsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetClosedOrdersAsync  

[https://docs.bitfinex.com/reference#rest-auth-orders-history](https://docs.bitfinex.com/reference#rest-auth-orders-history)  
<p>

*Get the order history for a symbol for this account*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Trading.GetClosedOrdersAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetClosedOrdersAsync(string? symbol = default, DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbol|The symbol to get the history for|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOpenOrdersAsync  

[https://docs.bitfinex.com/reference#rest-auth-orders](https://docs.bitfinex.com/reference#rest-auth-orders)  
<p>

*Get the active orders*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Trading.GetOpenOrdersAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexOrder>>> GetOpenOrdersAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOrderAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-order-status](https://docs.bitfinex.com/v1/reference#rest-auth-order-status)  
<p>

*Get the status of a specific order*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Trading.GetOrderAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexPlacedOrder>> GetOrderAsync(long orderId, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|orderId|The order id of the order to get|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOrderTradesAsync  

[https://docs.bitfinex.com/reference#rest-auth-order-trades](https://docs.bitfinex.com/reference#rest-auth-order-trades)  
<p>

*Get the individual trades for an order*  

```csharp  
var client = new BitfinexClient();  
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
var client = new BitfinexClient();  
var result = await client.SpotApi.Trading.GetPositionHistoryAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexPositionExtended>>> GetPositionHistoryAsync(DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetPositionsByIdAsync  

[https://docs.bitfinex.com/reference#rest-auth-positions-audit](https://docs.bitfinex.com/reference#rest-auth-positions-audit)  
<p>

*Get positions by id*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Trading.GetPositionsByIdAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexPositionExtended>>> GetPositionsByIdAsync(IEnumerable<string> ids, DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
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

## GetUserTradesAsync  

[https://docs.bitfinex.com/reference#rest-auth-trades](https://docs.bitfinex.com/reference#rest-auth-trades)  
<p>

*Get the trade history for a symbol*  

```csharp  
var client = new BitfinexClient();  
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

## PlaceOrderAsync  

[https://docs.bitfinex.com/reference#rest-auth-submit-order](https://docs.bitfinex.com/reference#rest-auth-submit-order)  
<p>

*Place a new order*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Trading.PlaceOrderAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexWriteResult<BitfinexOrder>>> PlaceOrderAsync(string symbol, OrderSide side, OrderType type, decimal quantity, decimal price, int? flags = default, int? leverage = default, int? groupId = default, int? clientOrderId = default, decimal? priceTrailing = default, decimal? priceAuxLimit = default, decimal? priceOcoStop = default, DateTime? cancelTime = default, string? affiliateCode = default, CancellationToken ct = default);  
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
