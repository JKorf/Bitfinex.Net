# Bitfinex.Net AI API Quick Map

Use this file to route common user intents to the correct Bitfinex.Net client member. If a method name or parameter is not listed here, inspect `Bitfinex.Net/Interfaces/Clients/**` before generating code.

## Client Roots

| Intent | Use |
|---|---|
| REST calls | `new BitfinexRestClient()` |
| WebSocket streams and socket API requests | `new BitfinexSocketClient()` |
| API key authentication | `options.ApiCredentials = new BitfinexCredentials("key", "secret")` |
| Live environment | `BitfinexEnvironment.Live` |
| Dependency injection | `services.AddBitfinex(options => { ... })` |
| Market/account/trading REST | `client.SpotApi` |
| Authenticated funding REST | `client.GeneralApi.Funding` |
| WebSocket streams and socket actions | `socketClient.SpotApi` |

## Symbols

| Symbol type | Format |
|---|---|
| Trading | `tBTCUSD`, `tETHUSD` |
| Funding | `fUSD`, `fBTC` |
| Derivatives | `tBTCF0:USTF0` |

## Exchange Data REST

| User intent | Bitfinex.Net member |
|---|---|
| Get platform status | `client.SpotApi.ExchangeData.GetPlatformStatusAsync()` |
| Get ticker | `client.SpotApi.ExchangeData.GetTickerAsync("tBTCUSD")` |
| Get tickers | `client.SpotApi.ExchangeData.GetTickersAsync(new[] { "tBTCUSD" })` |
| Get funding ticker | `client.SpotApi.ExchangeData.GetFundingTickerAsync("fUSD")` |
| Get funding tickers | `client.SpotApi.ExchangeData.GetFundingTickersAsync(new[] { "fUSD" })` |
| Get ticker history | `client.SpotApi.ExchangeData.GetTickerHistoryAsync(...)` |
| Get trade history | `client.SpotApi.ExchangeData.GetTradeHistoryAsync("tBTCUSD")` |
| Get order book | `client.SpotApi.ExchangeData.GetOrderBookAsync("tBTCUSD", Precision.PrecisionLevel0)` |
| Get raw order book | `client.SpotApi.ExchangeData.GetRawOrderBookAsync("tBTCUSD")` |
| Get funding order book | `client.SpotApi.ExchangeData.GetFundingOrderBookAsync("fUSD", Precision.PrecisionLevel0)` |
| Get raw funding order book | `client.SpotApi.ExchangeData.GetRawFundingOrderBookAsync("fUSD")` |
| Get last kline | `client.SpotApi.ExchangeData.GetLastKlineAsync("tBTCUSD", KlineInterval.OneMinute)` |
| Get klines | `client.SpotApi.ExchangeData.GetKlinesAsync("tBTCUSD", KlineInterval.OneMinute)` |
| Get average execution price | `client.SpotApi.ExchangeData.GetAveragePriceAsync("tBTCUSD", quantity)` |
| Get foreign exchange rate | `client.SpotApi.ExchangeData.GetForeignExchangeRateAsync("BTC", "USD")` |
| Get derivatives status | `client.SpotApi.ExchangeData.GetDerivativesStatusAsync(new[] { "tBTCF0:USTF0" })` |
| Get derivatives status history | `client.SpotApi.ExchangeData.GetDerivativesStatusHistoryAsync("tBTCF0:USTF0")` |
| Get liquidation history | `client.SpotApi.ExchangeData.GetLiquidationsAsync()` |
| Get funding statistics | `client.SpotApi.ExchangeData.GetFundingStatisticsAsync("fUSD")` |
| Get funding size | `client.SpotApi.ExchangeData.GetLastFundingSizeAsync("USD")` |
| Get credit size | `client.SpotApi.ExchangeData.GetLastCreditSizeAsync("USD")` |
| Get longs/shorts totals | `client.SpotApi.ExchangeData.GetLastLongsShortsTotalsAsync("tBTCUSD", StatSide.Long)` |
| Get trading volume | `client.SpotApi.ExchangeData.GetLastTradingVolumeAsync(1)` |
| Get VWAP | `client.SpotApi.ExchangeData.GetLastVolumeWeightedAveragePriceAsync("tBTCUSD")` |
| Get symbol names | `client.SpotApi.ExchangeData.GetSymbolNamesAsync(SymbolType.Exchange)` |
| Get asset names | `client.SpotApi.ExchangeData.GetAssetNamesAsync()` |
| Get asset symbol map | `client.SpotApi.ExchangeData.GetAssetSymbolsAsync()` |
| Get asset full names | `client.SpotApi.ExchangeData.GetAssetFullNamesAsync()` |
| Get asset withdrawal fees | `client.SpotApi.ExchangeData.GetAssetWithdrawalFeesAsync()` |
| Get symbols info | `client.SpotApi.ExchangeData.GetSymbolsAsync()` |
| Get derivatives symbols info | `client.SpotApi.ExchangeData.GetFuturesSymbolsAsync()` |
| Get deposit/withdrawal status | `client.SpotApi.ExchangeData.GetDepositWithdrawalStatusAsync()` |
| Get margin info | `client.SpotApi.ExchangeData.GetMarginInfoAsync()` |
| Get derivatives fees | `client.SpotApi.ExchangeData.GetDerivativesFeesAsync()` |

## Account REST

| User intent | Bitfinex.Net member |
|---|---|
| Get wallets/balances | `client.SpotApi.Account.GetBalancesAsync()` |
| Get base margin info | `client.SpotApi.Account.GetBaseMarginInfoAsync()` |
| Get symbol margin info | `client.SpotApi.Account.GetSymbolMarginInfoAsync("tBTCUSD")` |
| Get movements | `client.SpotApi.Account.GetMovementsAsync("BTC")` |
| Get movement details | `client.SpotApi.Account.GetMovementsDetailsAsync(id)` |
| Get alert list | `client.SpotApi.Account.GetAlertListAsync()` |
| Set price alert | `client.SpotApi.Account.SetAlertAsync("tBTCUSD", price)` |
| Delete price alert | `client.SpotApi.Account.DeleteAlertAsync("tBTCUSD", price)` |
| Calculate available balance | `client.SpotApi.Account.GetAvailableBalanceAsync("tBTCUSD", OrderSide.Buy, rate, WalletType.Exchange)` |
| Get ledger entries | `client.SpotApi.Account.GetLedgerEntriesAsync("BTC")` |
| Get user info | `client.SpotApi.Account.GetUserInfoAsync()` |
| Get fee summary | `client.SpotApi.Account.Get30DaySummaryAndFeesAsync()` |
| Get deposit address | `client.SpotApi.Account.GetDepositAddressAsync(method, WithdrawWallet.Exchange)` |
| Transfer between wallets | `client.SpotApi.Account.WalletTransferAsync("BTC", quantity, WithdrawWallet.Exchange, WithdrawWallet.Margin)` |
| Withdraw | `client.SpotApi.Account.WithdrawV2Async(method, wallet, quantity, address)` |
| Get login history | `client.SpotApi.Account.GetLoginHistoryAsync()` |
| Get API key permissions | `client.SpotApi.Account.GetApiKeyPermissionsAsync()` |
| Get account change log | `client.SpotApi.Account.GetAccountChangeLogAsync()` |

## Trading REST

| User intent | Bitfinex.Net member |
|---|---|
| Get open orders | `client.SpotApi.Trading.GetOpenOrdersAsync("tBTCUSD")` |
| Get closed orders | `client.SpotApi.Trading.GetClosedOrdersAsync("tBTCUSD")` |
| Get order trades | `client.SpotApi.Trading.GetOrderTradesAsync("tBTCUSD", orderId)` |
| Get user trades | `client.SpotApi.Trading.GetUserTradesAsync("tBTCUSD")` |
| Place exchange limit order | `client.SpotApi.Trading.PlaceOrderAsync("tBTCUSD", OrderSide.Buy, OrderType.ExchangeLimit, quantity, price)` |
| Place margin limit order | `client.SpotApi.Trading.PlaceOrderAsync("tBTCUSD", OrderSide.Buy, OrderType.Limit, quantity, price)` |
| Place derivative order with leverage | `client.SpotApi.Trading.PlaceOrderAsync("tBTCF0:USTF0", OrderSide.Buy, OrderType.Limit, quantity, price, leverage: 10)` |
| Cancel order | `client.SpotApi.Trading.CancelOrderAsync(orderId)` |
| Cancel multiple orders | `client.SpotApi.Trading.CancelOrdersAsync(...)` |
| Get active positions | `client.SpotApi.Trading.GetPositionsAsync()` |
| Get position history | `client.SpotApi.Trading.GetPositionHistoryAsync()` |
| Get positions by id | `client.SpotApi.Trading.GetPositionsByIdAsync(ids)` |
| Get position snapshots | `client.SpotApi.Trading.GetPositionSnapshotsAsync()` |
| Claim position | `client.SpotApi.Trading.ClaimPositionAsync(id, quantity)` |
| Increase position | `client.SpotApi.Trading.IncreasePositionAsync("tBTCUSD", quantity)` |
| Get increase position info | `client.SpotApi.Trading.GetIncreasePositionInfoAsync("tBTCUSD", quantity)` |

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
| Subscribe ticker updates | `socketClient.SpotApi.SubscribeToTickerUpdatesAsync("tBTCUSD", handler)` |
| Subscribe funding ticker updates | `socketClient.SpotApi.SubscribeToFundingTickerUpdatesAsync("fUSD", handler)` |
| Subscribe order book updates | `socketClient.SpotApi.SubscribeToOrderBookUpdatesAsync("tBTCUSD", Precision.PrecisionLevel0, Frequency.Realtime, 25, handler)` |
| Subscribe raw order book updates | `socketClient.SpotApi.SubscribeToRawOrderBookUpdatesAsync("tBTCUSD", 25, handler)` |
| Subscribe trades | `socketClient.SpotApi.SubscribeToTradeUpdatesAsync("tBTCUSD", handler)` |
| Subscribe klines | `socketClient.SpotApi.SubscribeToKlineUpdatesAsync("tBTCUSD", KlineInterval.OneMinute, handler)` |
| Subscribe liquidation updates | `socketClient.SpotApi.SubscribeToLiquidationUpdatesAsync(handler)` |
| Subscribe derivatives updates | `socketClient.SpotApi.SubscribeToDerivativesUpdatesAsync("tBTCF0:USTF0", handler)` |
| Subscribe authenticated user updates | `socketClient.SpotApi.SubscribeToUserUpdatesAsync(...)` |
| Socket place order | `socketClient.SpotApi.PlaceOrderAsync(OrderSide.Buy, OrderType.ExchangeLimit, "tBTCUSD", quantity, price: price)` |
| Socket cancel order | `socketClient.SpotApi.CancelOrderAsync(orderId)` |
| Socket submit funding offer | `socketClient.SpotApi.SubmitFundingOfferAsync(FundingOfferType.Limit, "fUSD", quantity, price, period)` |

## SharedApis

| User intent | Bitfinex.Net member or interface |
|---|---|
| Shared spot REST client | `new BitfinexRestClient().SpotApi.SharedClient` |
| Shared spot socket client | `new BitfinexSocketClient().SpotApi.SharedClient` |
| Shared spot ticker REST | `ISpotTickerRestClient.GetSpotTickerAsync(new GetTickerRequest(symbol))` |
| Shared spot order REST | `ISpotOrderRestClient.PlaceSpotOrderAsync(...)` |
| Shared ticker socket | `ITickerSocketClient.SubscribeToTickerUpdatesAsync(...)` |
| Shared order book socket | `IOrderBookSocketClient.SubscribeToOrderBookUpdatesAsync(...)` |

For shared socket subscriptions, keep the concrete socket client and unsubscribe with `await socketClient.UnsubscribeAsync(subscription.Data)`.

## Common Routing Pitfalls

| Do not use | Use instead |
|---|---|
| Raw `HttpClient` | `BitfinexRestClient` / `BitfinexSocketClient` |
| `ApiCredentials` | `BitfinexCredentials` |
| `FuturesApi` | Derivative symbols under `SpotApi` |
| Top-level `FundingApi` | `GeneralApi.Funding` |
| `BTCUSD` | `tBTCUSD` |
| `USD` for funding symbol | `fUSD` |
| `.Data` without `.Success` check | Check `.Success` first |
| Shared socket `UnsubscribeAsync(...)` | Keep the concrete socket client and call `socketClient.UnsubscribeAsync(subscription.Data)` |
| Custom `clientOrderId` by default | Let Bitfinex.Net auto-generate it |

