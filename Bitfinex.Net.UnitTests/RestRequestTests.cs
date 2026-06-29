using NUnit.Framework;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Bitfinex.Net.Clients;
using CryptoExchange.Net.Testing;
using System.Linq;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class RestRequestTests
    {
        [Test]
        public async Task ValidateSpotAccountCalls()
        {
            var client = new BitfinexRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new BitfinexCredentials("123", "456");
            });
            var tester = new RestRequestValidator<BitfinexRestClient>(client, "Endpoints/Spot/Account", "https://api.bitfinex.com", IsAuthenticated);
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetBalancesAsync(), "GetBalances");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetBaseMarginInfoAsync(), "GetBaseMarginInfo");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetSymbolMarginInfoAsync("tETHUST"), "GetSymbolMarginInfo");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetMovementsAsync(), "GetMovements");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetMovementsDetailsAsync(123), "GetMovementsDetails");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetAlertListAsync(), "GetAlertList");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.SetAlertAsync("tETHUST", 123), "SetAlert");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.DeleteAlertAsync("tETHUST", 123), "DeleteAlert");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetAvailableBalanceAsync("tETHUST", Enums.OrderSide.Buy, 1, Enums.WalletType.Funding), "GetAvailableBalance");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetLedgerEntriesAsync("tETHUST"), "GetLedgerEntries");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetUserInfoAsync(), "GetUserInfo");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.Get30DaySummaryAndFeesAsync(), "Get30DaySummaryAndFees");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetDepositAddressAsync("123", Enums.WithdrawWallet.Exchange), "GetDepositAddress");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.WalletTransferAsync("ETH", 1, Enums.WithdrawWallet.Exchange, Enums.WithdrawWallet.Exchange), "WalletTransfer");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.WithdrawAsync("ETH", Enums.WithdrawWallet.Exchange, 1), "Withdraw", useSingleArrayItem: true, ignoreProperties: new System.Collections.Generic.List<string> { "status" });
            await tester.ValidateAsync(client => client.ExchangeApi.Account.WithdrawV2Async("BITCOIN", Enums.WithdrawWallet.Exchange, 1, "123"), "WithdrawV2");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetLoginHistoryAsync(), "GetLoginHistory");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetApiKeyPermissionsAsync(), "GetApiKeyPermissions");
            await tester.ValidateAsync(client => client.ExchangeApi.Account.GetAccountChangeLogAsync(), "GetAccountChangeLog");
        }

        [Test]
        public async Task ValidateSpotExchangeDataCalls()
        {
            var client = new BitfinexRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new BitfinexCredentials("123", "456");
            });
            var tester = new RestRequestValidator<BitfinexRestClient>(client, "Endpoints/Spot/ExchangeData", "https://api.bitfinex.com", IsAuthenticated);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetTickersAsync(), "GetTickers");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetPlatformStatusAsync(), "GetPlatformStatus");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetsListAsync(), "GetAssetsList");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetSymbolNamesAsync(Enums.SymbolType.Exchange), "GetSymbolNames");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetNamesAsync(), "GetAssetNames");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetSymbolsAsync(), "GetAssetSymbols", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetFullNamesAsync(), "GetAssetFullNames", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetUnitsAsync(), "GetAssetUnits", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetUnderlyingsAsync(), "GetAssetUnderlyings", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetNetworksAsync(), "GetAssetNetworks", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetBlockExplorerUrlsAsync(), "GetAssetBlockExplorerUrls", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetWithdrawalFeesAsync(), "GetAssetWithdrawalFees", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAssetDepositWithdrawalMethodsAsync(), "GetAssetDepositWithdrawalMethods", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetSymbolsAsync(), "GetSymbols", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetFuturesSymbolsAsync(), "GetFuturesSymbols", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetDepositWithdrawalStatusAsync(), "GetDepositWithdrawalStatus", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetMarginInfoAsync(), "GetMarginInfo", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetDerivativesFeesAsync(), "GetDerivativesFees", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetTickerAsync("tETHUST"), "GetTicker", useSingleArrayItem: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetTickersAsync(), "GetTickers");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetFundingTickerAsync("fUSD"), "GetFundingTicker", useSingleArrayItem: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetFundingTickersAsync(), "GetFundingTickers");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetTickerHistoryAsync(), "GetTickerHistory");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetTradeHistoryAsync("tETHUST"), "GetTradeHistory");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetOrderBookAsync("tETHUST", Enums.Precision.PrecisionLevel2), "GetOrderBook", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetFundingOrderBookAsync("fUSD", Enums.Precision.PrecisionLevel2), "GetFundingOrderBook", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetRawOrderBookAsync("tETHUST"), "GetRawOrderBook", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetRawFundingOrderBookAsync("fUSD"), "GetRawFundingOrderBook", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetLastKlineAsync("tETHUST", Enums.KlineInterval.OneDay), "GetLastKline");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetKlinesAsync("tETHUST", Enums.KlineInterval.OneDay), "GetKlines");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetAveragePriceAsync("tETHUST", 1), "GetAveragePrice");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetForeignExchangeRateAsync("ETH", "UST"), "GetForeignExchangeRate");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetDerivativesStatusAsync(), "GetDerivativesStatus");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetDerivativesStatusHistoryAsync("tETHUST"), "GetDerivativesStatusHistory");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetLiquidationsAsync(), "GetLiquidations", useSingleArrayItem: true);
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetFundingStatisticsAsync("tETHUST"), "GetFundingStatistics");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetFundingSizeHistoryAsync("UST"), "GetFundingSize");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetCreditSizeHistoryAsync("UST"), "GetCreditSize");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetCreditSizeHistoryAsync("UST", "tETHUST"), "GetCreditSize2");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetLongsShortsTotalsHistoryAsync("tETHUST", Enums.StatSide.Long), "GetLongsShortsTotals");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetTradingVolumeHistoryAsync(1), "GetTradingVolume");
            await tester.ValidateAsync(client => client.ExchangeApi.ExchangeData.GetVolumeWeightedAveragePriceHistoryAsync("tETHUST"), "GetVolumeWeightedAveragePrice");
        }

        [Test]
        public async Task ValidateSpotTradingCalls()
        {
            var client = new BitfinexRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new BitfinexCredentials("123", "456");
            });
            var tester = new RestRequestValidator<BitfinexRestClient>(client, "Endpoints/Spot/Trading", "https://api.bitfinex.com", IsAuthenticated);
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetOpenOrdersAsync(), "GetOpenOrders");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetClosedOrdersAsync(), "GetClosedOrders");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetOrderTradesAsync("tETHUST", 123), "GetOrderTrades");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetUserTradesAsync(), "GetUserTrades");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.PlaceOrderAsync("tETHUST", Enums.OrderSide.Buy, Enums.OrderType.Market, 1, 0), "PlaceOrder", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.CancelOrderAsync(123), "CancelOrder");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.CancelOrdersAsync(new[] { 123L }), "CancelOrders");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetPositionHistoryAsync(), "GetPositionHistory");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.ClaimPositionAsync(123, 1), "ClaimPosition");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.IncreasePositionAsync("tETHUST", 1), "IncreasePosition");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetIncreasePositionInfoAsync("tETHUST", 1), "GetIncreasePositionInfo");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetPositionsAsync(), "GetPositions");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetPositionSnapshotsAsync(), "GetPositionSnapshots");
            await tester.ValidateAsync(client => client.ExchangeApi.Trading.GetPositionsByIdAsync(new[] { "123" }), "GetPositionsById");
        }

        private bool IsAuthenticated(IHttpResult result)
        {
            return result.RequestUrl.Contains("bfx-signature") || result.RequestHeaders.Any(x => x.Key == "bfx-signature") || result.RequestHeaders.Any(x => x.Key == "X-BFX-SIGNATURE");
        }
    }
}
