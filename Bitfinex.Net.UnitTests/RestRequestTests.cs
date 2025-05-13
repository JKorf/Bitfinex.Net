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
                opts.ApiCredentials = new ApiCredentials("123", "456");
            });
            var tester = new RestRequestValidator<BitfinexRestClient>(client, "Endpoints/Spot/Account", "https://api.bitfinex.com", IsAuthenticated);
            await tester.ValidateAsync(client => client.SpotApi.Account.GetBalancesAsync(), "GetBalances");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetBaseMarginInfoAsync(), "GetBaseMarginInfo");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetSymbolMarginInfoAsync("tETHUST"), "GetSymbolMarginInfo");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetMovementsAsync(), "GetMovements");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetMovementsDetailsAsync(123), "GetMovementsDetails");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetAlertListAsync(), "GetAlertList");
            await tester.ValidateAsync(client => client.SpotApi.Account.SetAlertAsync("tETHUST", 123), "SetAlert");
            await tester.ValidateAsync(client => client.SpotApi.Account.DeleteAlertAsync("tETHUST", 123), "DeleteAlert");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetAvailableBalanceAsync("tETHUST", Enums.OrderSide.Buy, 1, Enums.WalletType.Funding), "GetAvailableBalance");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetLedgerEntriesAsync("tETHUST"), "GetLedgerEntries");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetUserInfoAsync(), "GetUserInfo");
            await tester.ValidateAsync(client => client.SpotApi.Account.Get30DaySummaryAndFeesAsync(), "Get30DaySummaryAndFees");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetDepositAddressAsync("123", Enums.WithdrawWallet.Exchange), "GetDepositAddress");
            await tester.ValidateAsync(client => client.SpotApi.Account.WalletTransferAsync("ETH", 1, Enums.WithdrawWallet.Exchange, Enums.WithdrawWallet.Exchange), "WalletTransfer");
            await tester.ValidateAsync(client => client.SpotApi.Account.WithdrawAsync("ETH", Enums.WithdrawWallet.Exchange, 1), "Withdraw", useSingleArrayItem: true, ignoreProperties: new System.Collections.Generic.List<string> { "status" });
            await tester.ValidateAsync(client => client.SpotApi.Account.WithdrawV2Async("BITCOIN", Enums.WithdrawWallet.Exchange, 1, "123"), "WithdrawV2");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetLoginHistoryAsync(), "GetLoginHistory");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetApiKeyPermissionsAsync(), "GetApiKeyPermissions");
            await tester.ValidateAsync(client => client.SpotApi.Account.GetAccountChangeLogAsync(), "GetAccountChangeLog");
        }

        [Test]
        public async Task ValidateSpotExchangeDataCalls()
        {
            var client = new BitfinexRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new ApiCredentials("123", "456");
            });
            var tester = new RestRequestValidator<BitfinexRestClient>(client, "Endpoints/Spot/ExchangeData", "https://api.bitfinex.com", IsAuthenticated);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetTickersAsync(), "GetTickers");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetPlatformStatusAsync(), "GetPlatformStatus");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAssetsListAsync(), "GetAssetsList");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetSymbolNamesAsync(Enums.SymbolType.Exchange), "GetSymbolNames");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAssetNamesAsync(), "GetAssetNames");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAssetSymbolsAsync(), "GetAssetSymbols", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAssetFullNamesAsync(), "GetAssetFullNames", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAssetUnitsAsync(), "GetAssetUnits", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAssetUnderlyingsAsync(), "GetAssetUnderlyings", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAssetNetworksAsync(), "GetAssetNetworks", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAssetBlockExplorerUrlsAsync(), "GetAssetBlockExplorerUrls", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAssetWithdrawalFeesAsync(), "GetAssetWithdrawalFees", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAssetDepositWithdrawalMethodsAsync(), "GetAssetDepositWithdrawalMethods", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetSymbolsAsync(), "GetSymbols", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetFuturesSymbolsAsync(), "GetFuturesSymbols", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetDepositWithdrawalStatusAsync(), "GetDepositWithdrawalStatus", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetMarginInfoAsync(), "GetMarginInfo", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetDerivativesFeesAsync(), "GetDerivativesFees", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetTickerAsync("tETHUST"), "GetTicker", useSingleArrayItem: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetTickersAsync(), "GetTickers");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetFundingTickerAsync("fUSD"), "GetFundingTicker", useSingleArrayItem: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetFundingTickersAsync(), "GetFundingTickers");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetTickerHistoryAsync(), "GetTickerHistory");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetTradeHistoryAsync("tETHUST"), "GetTradeHistory");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetOrderBookAsync("tETHUST", Enums.Precision.PrecisionLevel2), "GetOrderBook", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetFundingOrderBookAsync("fUSD", Enums.Precision.PrecisionLevel2), "GetFundingOrderBook", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetRawOrderBookAsync("tETHUST"), "GetRawOrderBook", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetRawFundingOrderBookAsync("fUSD"), "GetRawFundingOrderBook", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetLastKlineAsync("tETHUST", Enums.KlineInterval.OneDay), "GetLastKline");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetKlinesAsync("tETHUST", Enums.KlineInterval.OneDay), "GetKlines");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetAveragePriceAsync("tETHUST", 1), "GetAveragePrice");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetForeignExchangeRateAsync("ETH", "UST"), "GetForeignExchangeRate");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetDerivativesStatusAsync(), "GetDerivativesStatus");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetDerivativesStatusHistoryAsync("tETHUST"), "GetDerivativesStatusHistory");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetLiquidationsAsync(), "GetLiquidations", useSingleArrayItem: true);
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetFundingStatisticsAsync("tETHUST"), "GetFundingStatistics");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetFundingSizeHistoryAsync("UST"), "GetFundingSize");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetCreditSizeHistoryAsync("UST"), "GetCreditSize");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetCreditSizeHistoryAsync("UST", "tETHUST"), "GetCreditSize2");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetLongsShortsTotalsHistoryAsync("tETHUST", Enums.StatSide.Long), "GetLongsShortsTotals");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetTradingVolumeHistoryAsync(1), "GetTradingVolume");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetVolumeWeightedAveragePriceHistoryAsync("tETHUST"), "GetVolumeWeightedAveragePrice");
        }

        [Test]
        public async Task ValidateSpotTradingCalls()
        {
            var client = new BitfinexRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new ApiCredentials("123", "456");
            });
            var tester = new RestRequestValidator<BitfinexRestClient>(client, "Endpoints/Spot/Trading", "https://api.bitfinex.com", IsAuthenticated);
            await tester.ValidateAsync(client => client.SpotApi.Trading.GetOpenOrdersAsync(), "GetOpenOrders");
            await tester.ValidateAsync(client => client.SpotApi.Trading.GetClosedOrdersAsync(), "GetClosedOrders");
            await tester.ValidateAsync(client => client.SpotApi.Trading.GetOrderTradesAsync("tETHUST", 123), "GetOrderTrades");
            await tester.ValidateAsync(client => client.SpotApi.Trading.GetUserTradesAsync(), "GetUserTrades");
            await tester.ValidateAsync(client => client.SpotApi.Trading.PlaceOrderAsync("tETHUST", Enums.OrderSide.Buy, Enums.OrderType.Market, 1, 0), "PlaceOrder", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApi.Trading.CancelOrderAsync(123), "CancelOrder");
            await tester.ValidateAsync(client => client.SpotApi.Trading.CancelOrdersAsync(new[] { 123L }), "CancelOrders");
            await tester.ValidateAsync(client => client.SpotApi.Trading.GetPositionHistoryAsync(), "GetPositionHistory");
            await tester.ValidateAsync(client => client.SpotApi.Trading.ClaimPositionAsync(123, 1), "ClaimPosition");
            await tester.ValidateAsync(client => client.SpotApi.Trading.IncreasePositionAsync("tETHUST", 1), "IncreasePosition");
            await tester.ValidateAsync(client => client.SpotApi.Trading.GetIncreasePositionInfoAsync("tETHUST", 1), "GetIncreasePositionInfo");
            await tester.ValidateAsync(client => client.SpotApi.Trading.GetPositionsAsync(), "GetPositions");
            await tester.ValidateAsync(client => client.SpotApi.Trading.GetPositionSnapshotsAsync(), "GetPositionSnapshots");
            await tester.ValidateAsync(client => client.SpotApi.Trading.GetPositionsByIdAsync(new[] { "123" }), "GetPositionsById");
        }

        private bool IsAuthenticated(WebCallResult result)
        {
            return result.RequestUrl.Contains("bfx-signature") || result.RequestHeaders.Any(x => x.Key == "bfx-signature") || result.RequestHeaders.Any(x => x.Key == "X-BFX-SIGNATURE");
        }
    }
}
