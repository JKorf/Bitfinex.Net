# Bitfinex.Net API surfaces

Use this reference to route an intent to the relevant member. Table entries abbreviate overloads; confirm parameters in the installed package before use. See [usage](usage.md) for setup and results.

## Client roots

| Surface | Purpose |
| --- | --- |
| `BitfinexRestClient.ExchangeApi.ExchangeData` | Trading and funding market data, symbol metadata, derivatives status, and public statistics |
| `BitfinexRestClient.ExchangeApi.Account` | Wallets, margin info, movements, ledgers, deposit addresses, transfers, withdrawals, and key permissions |
| `BitfinexRestClient.ExchangeApi.Trading` | Spot, margin, and derivatives orders, trades, and positions |
| `BitfinexRestClient.GeneralApi.Funding` | Authenticated funding offers, loans, credits, trades, and auto-renew |
| `BitfinexSocketClient.ExchangeApi` | Public streams, authenticated user streams, order commands, and funding commands |

The tables use `client` for a `BitfinexRestClient` and `socketClient` for a `BitfinexSocketClient`. There are no product-specific `SpotApi`, `FuturesApi`, or top-level `FundingApi` roots. `BitfinexEnvironment.Live` is the standard environment; do not assume another exchange's testnet configuration applies.

## Symbols

| Symbol type | Format |
|---|---|
| Trading | `tBTCUSD`, `tETHUSD` |
| Funding | `fUSD`, `fBTC` |
| Derivatives | `tBTCF0:USTF0` |

## Exchange Data REST

| User intent | Bitfinex.Net member |
|---|---|
| Get platform status | `client.ExchangeApi.ExchangeData.GetPlatformStatusAsync()` |
| Get ticker | `client.ExchangeApi.ExchangeData.GetTickerAsync("tBTCUSD")` |
| Get tickers | `client.ExchangeApi.ExchangeData.GetTickersAsync(new[] { "tBTCUSD" })` |
| Get funding ticker | `client.ExchangeApi.ExchangeData.GetFundingTickerAsync("fUSD")` |
| Get funding tickers | `client.ExchangeApi.ExchangeData.GetFundingTickersAsync(new[] { "fUSD" })` |
| Get ticker history | `client.ExchangeApi.ExchangeData.GetTickerHistoryAsync(...)` |
| Get trade history | `client.ExchangeApi.ExchangeData.GetTradeHistoryAsync("tBTCUSD")` |
| Get order book | `client.ExchangeApi.ExchangeData.GetOrderBookAsync("tBTCUSD", Precision.PrecisionLevel0)` |
| Get raw order book | `client.ExchangeApi.ExchangeData.GetRawOrderBookAsync("tBTCUSD")` |
| Get funding order book | `client.ExchangeApi.ExchangeData.GetFundingOrderBookAsync("fUSD", Precision.PrecisionLevel0)` |
| Get raw funding order book | `client.ExchangeApi.ExchangeData.GetRawFundingOrderBookAsync("fUSD")` |
| Get last kline | `client.ExchangeApi.ExchangeData.GetLastKlineAsync("tBTCUSD", KlineInterval.OneMinute)` |
| Get klines | `client.ExchangeApi.ExchangeData.GetKlinesAsync("tBTCUSD", KlineInterval.OneMinute)` |
| Get average execution price | `client.ExchangeApi.ExchangeData.GetAveragePriceAsync("tBTCUSD", quantity)` |
| Get foreign exchange rate | `client.ExchangeApi.ExchangeData.GetForeignExchangeRateAsync("BTC", "USD")` |
| Get derivatives status | `client.ExchangeApi.ExchangeData.GetDerivativesStatusAsync(new[] { "tBTCF0:USTF0" })` |
| Get derivatives status history | `client.ExchangeApi.ExchangeData.GetDerivativesStatusHistoryAsync("tBTCF0:USTF0")` |
| Get liquidation history | `client.ExchangeApi.ExchangeData.GetLiquidationsAsync()` |
| Get funding statistics | `client.ExchangeApi.ExchangeData.GetFundingStatisticsAsync("fUSD")` |
| Get funding size | `client.ExchangeApi.ExchangeData.GetLastFundingSizeAsync("USD")` |
| Get credit size | `client.ExchangeApi.ExchangeData.GetLastCreditSizeAsync("USD")` |
| Get longs/shorts totals | `client.ExchangeApi.ExchangeData.GetLastLongsShortsTotalsAsync("tBTCUSD", StatSide.Long)` |
| Get trading volume | `client.ExchangeApi.ExchangeData.GetLastTradingVolumeAsync(1)` |
| Get VWAP | `client.ExchangeApi.ExchangeData.GetLastVolumeWeightedAveragePriceAsync("tBTCUSD")` |
| Get symbol names | `client.ExchangeApi.ExchangeData.GetSymbolNamesAsync(SymbolType.Exchange)` |
| Get asset names | `client.ExchangeApi.ExchangeData.GetAssetNamesAsync()` |
| Get asset symbol map | `client.ExchangeApi.ExchangeData.GetAssetSymbolsAsync()` |
| Get asset full names | `client.ExchangeApi.ExchangeData.GetAssetFullNamesAsync()` |
| Get asset withdrawal fees | `client.ExchangeApi.ExchangeData.GetAssetWithdrawalFeesAsync()` |
| Get symbols info | `client.ExchangeApi.ExchangeData.GetSymbolsAsync()` |
| Get derivatives symbols info | `client.ExchangeApi.ExchangeData.GetFuturesSymbolsAsync()` |
| Get deposit/withdrawal status | `client.ExchangeApi.ExchangeData.GetDepositWithdrawalStatusAsync()` |
| Get margin info | `client.ExchangeApi.ExchangeData.GetMarginInfoAsync()` |
| Get derivatives fees | `client.ExchangeApi.ExchangeData.GetDerivativesFeesAsync()` |

## Account REST

| User intent | Bitfinex.Net member |
|---|---|
| Get wallets/balances | `client.ExchangeApi.Account.GetBalancesAsync()` |
| Get base margin info | `client.ExchangeApi.Account.GetBaseMarginInfoAsync()` |
| Get symbol margin info | `client.ExchangeApi.Account.GetSymbolMarginInfoAsync("tBTCUSD")` |
| Get movements | `client.ExchangeApi.Account.GetMovementsAsync("BTC")` |
| Get movement details | `client.ExchangeApi.Account.GetMovementsDetailsAsync(id)` |
| Get alert list | `client.ExchangeApi.Account.GetAlertListAsync()` |
| Set price alert | `client.ExchangeApi.Account.SetAlertAsync("tBTCUSD", price)` |
| Delete price alert | `client.ExchangeApi.Account.DeleteAlertAsync("tBTCUSD", price)` |
| Calculate available balance | `client.ExchangeApi.Account.GetAvailableBalanceAsync("tBTCUSD", OrderSide.Buy, rate, WalletType.Exchange)` |
| Get ledger entries | `client.ExchangeApi.Account.GetLedgerEntriesAsync("BTC")` |
| Get user info | `client.ExchangeApi.Account.GetUserInfoAsync()` |
| Get fee summary | `client.ExchangeApi.Account.Get30DaySummaryAndFeesAsync()` |
| Get deposit address | `client.ExchangeApi.Account.GetDepositAddressAsync(method, WithdrawWallet.Exchange)` |
| Transfer between wallets | `client.ExchangeApi.Account.WalletTransferAsync("BTC", quantity, WithdrawWallet.Exchange, WithdrawWallet.Margin)` |
| Withdraw | `client.ExchangeApi.Account.WithdrawV2Async(method, wallet, quantity, address)` |
| Get login history | `client.ExchangeApi.Account.GetLoginHistoryAsync()` |
| Get API key permissions | `client.ExchangeApi.Account.GetApiKeyPermissionsAsync()` |
| Get account change log | `client.ExchangeApi.Account.GetAccountChangeLogAsync()` |

## Trading REST

| User intent | Bitfinex.Net member |
|---|---|
| Get open orders | `client.ExchangeApi.Trading.GetOpenOrdersAsync("tBTCUSD")` |
| Get closed orders | `client.ExchangeApi.Trading.GetClosedOrdersAsync("tBTCUSD")` |
| Get order trades | `client.ExchangeApi.Trading.GetOrderTradesAsync("tBTCUSD", orderId)` |
| Get user trades | `client.ExchangeApi.Trading.GetUserTradesAsync("tBTCUSD")` |
| Place exchange limit order | `client.ExchangeApi.Trading.PlaceOrderAsync("tBTCUSD", OrderSide.Buy, OrderType.ExchangeLimit, quantity, price)` |
| Place margin limit order | `client.ExchangeApi.Trading.PlaceOrderAsync("tBTCUSD", OrderSide.Buy, OrderType.Limit, quantity, price)` |
| Place derivative order with leverage | `client.ExchangeApi.Trading.PlaceOrderAsync("tBTCF0:USTF0", OrderSide.Buy, OrderType.Limit, quantity, price, leverage: 10)` |
| Cancel order | `client.ExchangeApi.Trading.CancelOrderAsync(orderId)` |
| Cancel multiple orders | `client.ExchangeApi.Trading.CancelOrdersAsync(...)` |
| Get active positions | `client.ExchangeApi.Trading.GetPositionsAsync()` |
| Get position history | `client.ExchangeApi.Trading.GetPositionHistoryAsync()` |
| Get positions by id | `client.ExchangeApi.Trading.GetPositionsByIdAsync(ids)` |
| Get position snapshots | `client.ExchangeApi.Trading.GetPositionSnapshotsAsync()` |
| Claim position | `client.ExchangeApi.Trading.ClaimPositionAsync(id, quantity)` |
| Increase position | `client.ExchangeApi.Trading.IncreasePositionAsync("tBTCUSD", quantity)` |
| Get increase position info | `client.ExchangeApi.Trading.GetIncreasePositionInfoAsync("tBTCUSD", quantity)` |

## Funding REST

| User intent | Bitfinex.Net member |
|---|---|
| Get active funding offers | `client.GeneralApi.Funding.GetActiveFundingOffersAsync("fUSD")` |
| Get funding offer history | `client.GeneralApi.Funding.GetFundingOfferHistoryAsync("fUSD")` |
| Submit funding offer | `client.GeneralApi.Funding.SubmitFundingOfferAsync(FundingOrderType.Limit, "fUSD", quantity, rate, period)` |
| Cancel funding offer | `client.GeneralApi.Funding.CancelFundingOfferAsync(offerId)` |
| Cancel all funding offers | `client.GeneralApi.Funding.CancelAllFundingOffersAsync("USD")` |
| Get funding loans | `client.GeneralApi.Funding.GetFundingLoansAsync("fUSD")` |
| Get funding loan history | `client.GeneralApi.Funding.GetFundingLoansHistoryAsync("fUSD")` |
| Get funding credits | `client.GeneralApi.Funding.GetFundingCreditsAsync("fUSD")` |
| Get funding credit history | `client.GeneralApi.Funding.GetFundingCreditsHistoryAsync("fUSD")` |
| Get funding trade history | `client.GeneralApi.Funding.GetFundingTradesHistoryAsync("fUSD")` |
| Get funding info | `client.GeneralApi.Funding.GetFundingInfoAsync("fUSD")` |
| Submit funding auto-renew | `client.GeneralApi.Funding.SubmitFundingAutoRenewAsync("USD", true, quantity, rate, period)` |
| Get funding auto-renew status | `client.GeneralApi.Funding.GetFundingAutoRenewStatusAsync("USD")` |
| Keep funding | `client.GeneralApi.Funding.KeepFundingAsync(FundType.Credit, ids)` |
| Close funding | `client.GeneralApi.Funding.CloseFundingAsync(id)` |

## WebSocket

| User intent | Bitfinex.Net member |
|---|---|
| Subscribe ticker updates | `socketClient.ExchangeApi.SubscribeToTickerUpdatesAsync("tBTCUSD", handler)` |
| Subscribe funding ticker updates | `socketClient.ExchangeApi.SubscribeToFundingTickerUpdatesAsync("fUSD", handler)` |
| Subscribe order book updates | `socketClient.ExchangeApi.SubscribeToOrderBookUpdatesAsync("tBTCUSD", Precision.PrecisionLevel0, Frequency.Realtime, 25, handler)` |
| Subscribe raw order book updates | `socketClient.ExchangeApi.SubscribeToRawOrderBookUpdatesAsync("tBTCUSD", 25, handler)` |
| Subscribe trades | `socketClient.ExchangeApi.SubscribeToTradeUpdatesAsync("tBTCUSD", handler)` |
| Subscribe klines | `socketClient.ExchangeApi.SubscribeToKlineUpdatesAsync("tBTCUSD", KlineInterval.OneMinute, handler)` |
| Subscribe liquidation updates | `socketClient.ExchangeApi.SubscribeToLiquidationUpdatesAsync(handler)` |
| Subscribe derivatives updates | `socketClient.ExchangeApi.SubscribeToDerivativesUpdatesAsync("tBTCF0:USTF0", handler)` |
| Subscribe authenticated user updates | `socketClient.ExchangeApi.SubscribeToUserUpdatesAsync(...)` |
| Socket place order | `socketClient.ExchangeApi.PlaceOrderAsync(OrderSide.Buy, OrderType.ExchangeLimit, "tBTCUSD", quantity, price: price)` |
| Socket cancel order | `socketClient.ExchangeApi.CancelOrderAsync(orderId)` |
| Socket submit funding offer | `socketClient.ExchangeApi.SubmitFundingOfferAsync(FundingOfferType.Limit, "fUSD", quantity, price, period)` |

## Shared API V2

Use `.ExchangeApi.SharedApi` on the REST or socket client. The same surface supports spot and derivatives workflows through the applicable trading mode. Keep native request/model types separate from Shared API types.

| Intent | Capability or member |
| --- | --- |
| Get a ticker | `IGetTickerRest.GetTickerAsync` |
| Get/filter spot symbols | `IGetSpotSymbolsRest.GetSpotSymbolsAsync` |
| Get/filter derivatives symbols | `IGetFuturesSymbolsRest.GetFuturesSymbolsAsync` |
| Place a spot order | `IPlaceSpotOrderRest.PlaceSpotOrderAsync` |
| Place a derivatives order | `IPlaceFuturesOrderRest.PlaceFuturesOrderAsync` |
| Subscribe to tickers | `ISubscribeTickerSocket.SubscribeToTickerUpdatesAsync` |
| Subscribe to trades | `ISubscribeTradesSocket.SubscribeToTradeUpdatesAsync` |
| Subscribe to order books | `ISubscribeOrderBookSocket.SubscribeToOrderBookUpdatesAsync` |
| Place/cancel spot socket orders | `IPlaceSpotOrderSocket` / `ICancelSpotOrderSocket` |
| Place/cancel derivatives socket orders | `IPlaceFuturesOrderSocket` / `ICancelFuturesOrderSocket` |

`IBitfinexSharedApiClient` groups the two surfaces:

| Property | Interface |
| --- | --- |
| `Rest` | `IBitfinexRestClientExchangeSharedApi` |
| `Socket` | `IBitfinexSocketClientExchangeSharedApi` |

Use a surface property when the operation is known. For dynamic selection, call `GetCapability` on an aggregate instance with a typed descriptor such as `SharedCapabilities.Tickers.GetTicker.Rest`, and specify the intended trading mode. Check for a null match, then use its `Capability` and `Options`. Use `GetCapabilities` for all matches and `Discover` for summary metadata.

`AddBitfinex` registers the aggregate and supported capabilities. Where several exchanges implement an injected capability, select the intended implementation explicitly instead of relying on the last DI registration.

### Symbols and quantities

Use `SharedSymbol(TradingMode.Spot, "BTC", "USD")` for portable spot code; do not put native `t`/`f` prefixes into SharedSymbol assets. Select the correct trading mode for derivatives.

Call `GetSpotSymbolsAsync` or `GetFuturesSymbolsAsync` successfully before reading the corresponding `SpotSymbolCatalog` or `FuturesSymbolCatalog`. `GetSymbolsRequest` supports asset-type filters. Symbol responses populate display names, base/quote asset types, and subtypes, including stablecoin, commodity, and equity metadata.

Shared quantities use `SharedOrderQuantity`; read `QuantityInBaseAsset`, `QuantityInQuoteAsset`, or `QuantityInContracts` as appropriate. Bitfinex shared order books report `QuantityType` as `SharedQuantityType.BaseAsset`.

Shared subscriptions are closed through the concrete `BitfinexSocketClient.UnsubscribeAsync(subscription.Data)`.

## Source navigation in a checkout

These links are available in a source checkout, not in the standalone NuGet skill:

- [Client interfaces](../../../../Bitfinex.Net/Interfaces/Clients/): exact signatures and endpoint documentation. The ExchangeApi interfaces currently reside in the SpotApi source folder; the public root is still ExchangeApi.
- [Client implementations](../../../../Bitfinex.Net/Clients/): transport and Shared API mappings.
- [Compiled examples](../../../../Examples/ai-friendly/): complete programs.

For packaged usage patterns, see [usage](usage.md).
