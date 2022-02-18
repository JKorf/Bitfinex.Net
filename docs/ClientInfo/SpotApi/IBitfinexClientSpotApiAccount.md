---
title: IBitfinexClientSpotApiAccount
has_children: false
parent: IBitfinexClientSpotApi
grand_parent: Rest API documentation
---
*[generated documentation]*  
`BitfinexClient > SpotApi > Account`  
*Bitfinex account endpoints. Account endpoints include balance info, withdraw/deposit info and requesting and account settings*
  

***

## DeleteAlertAsync  

[https://docs.bitfinex.com/reference#rest-auth-alert-del](https://docs.bitfinex.com/reference#rest-auth-alert-del)  
<p>

*Delete an existing alert*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.DeleteAlertAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexSuccessResult>> DeleteAlertAsync(string symbol, decimal price, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol of the alert to delete|
|price|The price of the alert to delete|
|_[Optional]_ ct|Cancellation token|

</p>

***

## Get30DaySummaryAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-summary](https://docs.bitfinex.com/v1/reference#rest-auth-summary)  
<p>

*Get 30-day summary on trading volume and margin funding*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.Get30DaySummaryAsync();  
```  

```csharp  
Task<WebCallResult<Bitfinex30DaySummary>> Get30DaySummaryAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAccountInfoAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-account-info](https://docs.bitfinex.com/v1/reference#rest-auth-account-info)  
<p>

*Get information about your account*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetAccountInfoAsync();  
```  

```csharp  
Task<WebCallResult<BitfinexAccountInfo>> GetAccountInfoAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAlertListAsync  

[https://docs.bitfinex.com/reference#rest-auth-alerts](https://docs.bitfinex.com/reference#rest-auth-alerts)  
<p>

*Get the list of alerts*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetAlertListAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexAlert>>> GetAlertListAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetAvailableBalanceAsync  

[https://docs.bitfinex.com/reference#rest-auth-calc-order-avail](https://docs.bitfinex.com/reference#rest-auth-calc-order-avail)  
<p>

*Calculates the available balance for a symbol at a specific rate*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetAvailableBalanceAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexAvailableBalance>> GetAvailableBalanceAsync(string symbol, OrderSide side, decimal rate, WalletType type, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol|
|side|Buy or sell|
|rate|The rate/price|
|type|The wallet type|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetBalancesAsync  

[https://docs.bitfinex.com/reference#rest-auth-wallets](https://docs.bitfinex.com/reference#rest-auth-wallets)  
<p>

*Get all balances*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetBalancesAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexWallet>>> GetBalancesAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetBaseMarginInfoAsync  

[https://docs.bitfinex.com/reference#rest-auth-info-margin](https://docs.bitfinex.com/reference#rest-auth-info-margin)  
<p>

*Get the base margin info*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetBaseMarginInfoAsync();  
```  

```csharp  
Task<WebCallResult<BitfinexMarginBase>> GetBaseMarginInfoAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetDepositAddressAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-deposit](https://docs.bitfinex.com/v1/reference#rest-auth-deposit)  
<p>

*Gets a deposit address for an asset*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetDepositAddressAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexDepositAddress>> GetDepositAddressAsync(string asset, WithdrawWallet toWallet, bool? forceNew = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset|The asset to get address for|
|toWallet|The type of wallet the deposit is for|
|_[Optional]_ forceNew|If true a new address will be generated (previous addresses will still be valid)|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetLedgerEntriesAsync  

[https://docs.bitfinex.com/reference#rest-auth-ledgers](https://docs.bitfinex.com/reference#rest-auth-ledgers)  
<p>

*Get changes in your balance for an asset*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetLedgerEntriesAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexLedgerEntry>>> GetLedgerEntriesAsync(string? asset = default, DateTime? startTime = default, DateTime? endTime = default, int? limit = default, int? category = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ asset|The asset to check the ledger for|
|_[Optional]_ startTime|Start time of the data to return|
|_[Optional]_ endTime|End time of the data to return|
|_[Optional]_ limit|Max amount of results|
|_[Optional]_ category|Filter by category, see https://docs.bitfinex.com/reference#rest-auth-ledgers|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetMovementsAsync  

[https://docs.bitfinex.com/reference#rest-auth-movements](https://docs.bitfinex.com/reference#rest-auth-movements)  
<p>

*Get the withdrawal/deposit history*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetMovementsAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<BitfinexMovement>>> GetMovementsAsync(string? symbol = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbol|Symbol to get history for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetSymbolMarginInfoAsync  

[https://docs.bitfinex.com/reference#rest-auth-info-margin](https://docs.bitfinex.com/reference#rest-auth-info-margin)  
<p>

*Get the margin info for a symbol*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetSymbolMarginInfoAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexMarginSymbol>> GetSymbolMarginInfoAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the info for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetUserInfoAsync  

[https://docs.bitfinex.com/reference#rest-auth-info-user](https://docs.bitfinex.com/reference#rest-auth-info-user)  
<p>

*Gets information about the user associated with the api key/secret*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetUserInfoAsync();  
```  

```csharp  
Task<WebCallResult<BitfinexUserInfo>> GetUserInfoAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetWithdrawalFeesAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-fees](https://docs.bitfinex.com/v1/reference#rest-auth-fees)  
<p>

*Get withdrawal fees for this account*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.GetWithdrawalFeesAsync();  
```  

```csharp  
Task<WebCallResult<BitfinexWithdrawalFees>> GetWithdrawalFeesAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## SetAlertAsync  

[https://docs.bitfinex.com/reference#rest-auth-alert-set](https://docs.bitfinex.com/reference#rest-auth-alert-set)  
<p>

*Set an alert*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.SetAlertAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexAlert>> SetAlertAsync(string symbol, decimal price, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to set the alert for|
|price|The price to set the alert for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## WalletTransferAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-transfer-between-wallets](https://docs.bitfinex.com/v1/reference#rest-auth-transfer-between-wallets)  
<p>

*Transfers funds from one wallet to another*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.WalletTransferAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexTransferResult>> WalletTransferAsync(string asset, decimal quantity, WithdrawWallet fromWallet, WithdrawWallet toWallet, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset|The asset to transfer|
|quantity|The quantity to transfer|
|fromWallet|The wallet to remove funds from|
|toWallet|The wallet to add funds to|
|_[Optional]_ ct|Cancellation token|

</p>

***

## WithdrawAsync  

[https://docs.bitfinex.com/v1/reference#rest-auth-withdrawal](https://docs.bitfinex.com/v1/reference#rest-auth-withdrawal)  
<p>

*Withdraw funds from Bitfinex, either to a crypto currency address or a bank account*  
*All withdrawals need the withdrawType, wallet and quantity parameters*  
*CryptoCurrency withdrawals need the address parameters, the paymentId can be used for Monero as payment id and for Ripple as tag*  
*Wire withdrawals need the bank parameters. In some cases your bank will require the use of an intermediary bank, if this is the case, please supply those fields as well.*  

```csharp  
var client = new BitfinexClient();  
var result = await client.SpotApi.Account.WithdrawAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<BitfinexWithdrawalResult>> WithdrawAsync(string withdrawType, WithdrawWallet wallet, decimal quantity, string? address = default, string? accountNumber = default, string? bankSwift = default, string? bankName = default, string? bankAddress = default, string? bankCity = default, string? bankCountry = default, string? paymentDetails = default, bool? expressWire = default, string? intermediaryBankName = default, string? intermediaryBankAddress = default, string? intermediaryBankCity = default, string? intermediaryBankCountry = default, string? intermediaryBankAccount = default, string? intermediaryBankSwift = default, string? accountName = default, string? paymentId = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|withdrawType|The type of funds to withdraw|
|wallet|The wallet to withdraw from|
|quantity|The quantity to withdraw|
|_[Optional]_ address|The destination of the withdrawal|
|_[Optional]_ accountNumber|The account number|
|_[Optional]_ bankSwift|The SWIFT code of the bank|
|_[Optional]_ bankName|The bank name|
|_[Optional]_ bankAddress|The bank address|
|_[Optional]_ bankCity|The bank city|
|_[Optional]_ bankCountry|The bank country|
|_[Optional]_ paymentDetails|Message for the receiver|
|_[Optional]_ expressWire|Whether it is an express wire withdrawal|
|_[Optional]_ intermediaryBankName|Intermediary bank name|
|_[Optional]_ intermediaryBankAddress|Intermediary bank address|
|_[Optional]_ intermediaryBankCity|Intermediary bank city|
|_[Optional]_ intermediaryBankCountry|Intermediary bank country|
|_[Optional]_ intermediaryBankAccount|Intermediary bank account|
|_[Optional]_ intermediaryBankSwift|Intermediary bank SWIFT code|
|_[Optional]_ accountName|The name of the account|
|_[Optional]_ paymentId|Hex string for Monero transaction|
|_[Optional]_ ct|Cancellation token|

</p>
