---
title: IBitfinexClientGeneralApiFunding
has_children: false
parent: IBitfinexClientGeneralApi
grand_parent: Rest API documentation
---
*[generated documentation]*  
`BitfinexClient > GeneralApi > Funding`  
*Bitfinex funding endpoints.*
  

***

## CancelFundingOfferAsync  

[https://docs.bitfinex.com/reference#rest-auth-cancel-funding-offer](https://docs.bitfinex.com/reference#rest-auth-cancel-funding-offer)  
<p>

*Cancels an existing Funding Offer based on the offer ID entered.*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.CancelFundingOfferAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexWriteResult<BitfinexFundingOffer>>> CancelFundingOfferAsync(long offerId, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|offerId|The id of the offer to cancel|
|_[Optional]_ ct|Cancellation token|

</p>

***

## CancelOfferAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-cancel-offer](https://docs.bitfinex.com/v1/reference#rest-auth-cancel-offer)  
<p>

*Cancel an offer*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.CancelOfferAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexOffer>> CancelOfferAsync(long offerId, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|offerId|The id of the offer to cancel|
|_[Optional]_ ct|Cancellation token|

</p>

***

## CloseMarginFundingAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-close-margin-funding](https://docs.bitfinex.com/v1/reference#rest-auth-close-margin-funding)  
<p>

*Close margin funding*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.CloseMarginFundingAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexFundingContract>> CloseMarginFundingAsync(long swapId, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|swapId|The id to close|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetActiveFundingOffersAsync  

[https://docs.bitfinex.com/reference#rest-auth-funding-offers](https://docs.bitfinex.com/reference#rest-auth-funding-offers)  
<p>

*Get the active funding offers*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.GetActiveFundingOffersAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexFundingOffer>>> GetActiveFundingOffersAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to return the funding offer for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingCreditsAsync  

[https://docs.bitfinex.com/reference#rest-auth-funding-credits](https://docs.bitfinex.com/reference#rest-auth-funding-credits)  
<p>

*Get the funding credits*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.GetFundingCreditsAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexFundingCredit>>> GetFundingCreditsAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the funding credits for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingCreditsHistoryAsync  

[https://docs.bitfinex.com/reference#rest-auth-funding-credits-hist](https://docs.bitfinex.com/reference#rest-auth-funding-credits-hist)  
<p>

*Get the funding credits history*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.GetFundingCreditsHistoryAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexFundingCredit>>> GetFundingCreditsHistoryAsync(string symbol, DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get history for|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingInfoAsync  

[https://docs.bitfinex.com/reference#rest-auth-info-funding](https://docs.bitfinex.com/reference#rest-auth-info-funding)  
<p>

*Get funding info for a symbol*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.GetFundingInfoAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexFundingInfo>> GetFundingInfoAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the info for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingLoansAsync  

[https://docs.bitfinex.com/reference#rest-auth-funding-loans](https://docs.bitfinex.com/reference#rest-auth-funding-loans)  
<p>

*Get the funding loans*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.GetFundingLoansAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexFunding>>> GetFundingLoansAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the funding loans for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingLoansHistoryAsync  

[https://docs.bitfinex.com/reference#rest-auth-funding-loans-hist](https://docs.bitfinex.com/reference#rest-auth-funding-loans-hist)  
<p>

*Get the funding loan history*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.GetFundingLoansHistoryAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexFunding>>> GetFundingLoansHistoryAsync(string symbol, DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get history for|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingOfferHistoryAsync  

[https://docs.bitfinex.com/reference#rest-auth-funding-offers-hist](https://docs.bitfinex.com/reference#rest-auth-funding-offers-hist)  
<p>

*Get the funding offer history*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.GetFundingOfferHistoryAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexFundingOffer>>> GetFundingOfferHistoryAsync(string symbol, DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get history for|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetFundingTradesHistoryAsync  

[https://docs.bitfinex.com/reference#rest-auth-funding-trades-hist](https://docs.bitfinex.com/reference#rest-auth-funding-trades-hist)  
<p>

*Get the funding trades history*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.GetFundingTradesHistoryAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexFundingTrade>>> GetFundingTradesHistoryAsync(string symbol, DateTime? startTime = default, DateTime? endTime = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get history for|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOfferAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-offer-status](https://docs.bitfinex.com/v1/reference#rest-auth-offer-status)  
<p>

*Cancel an offer*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.GetOfferAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexOffer>> GetOfferAsync(long offerId, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|offerId|The id of the offer to cancel|
|_[Optional]_ ct|Cancellation token|

</p>

***

## NewOfferAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-new-offer](https://docs.bitfinex.com/v1/reference#rest-auth-new-offer)  
<p>

*Submit a new offer*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.NewOfferAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexOffer>> NewOfferAsync(string asset, decimal quantity, decimal price, int period, FundingType direction, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset|The asset|
|quantity|The quantity|
|price|Rate to lend or borrow at in percent per 365 days (0 for FRR)|
|period|Number of days|
|direction|Direction of the offer|
|_[Optional]_ ct|Cancellation token|

</p>

***

## SubmitFundingOfferAsync  

[https://docs.bitfinex.com/reference#rest-auth-submit-funding-offer](https://docs.bitfinex.com/reference#rest-auth-submit-funding-offer)  
<p>

*Submit a new funding offer.*  

```csharp  
var client = new BitfinexClient();  
var result = await client.GeneralApi.Funding.SubmitFundingOfferAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexWriteResult<BitfinexFundingOffer>>> SubmitFundingOfferAsync(FundingOrderType fundingOrderType, string symbol, decimal quantity, decimal rate, int period, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|fundingOrderType|Order Type (LIMIT, FRRDELTAVAR, FRRDELTAFIX).|
|symbol|Symbol for desired pair (fUSD, fBTC, etc..).|
|quantity|Quantity (positive for offer, negative for bid).|
|rate|Daily rate.|
|period|Time period of offer. Minimum 2 days. Maximum 120 days.|
|_[Optional]_ ct|Cancellation token|

</p>
