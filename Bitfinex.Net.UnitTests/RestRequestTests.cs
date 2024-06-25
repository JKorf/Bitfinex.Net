using NUnit.Framework;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Bitfinex.Net.Clients;
using CryptoExchange.Net.Testing;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class RestRequestTests
    {
        [Test]
        public async Task ValidateSpotExchangeDataCalls()
        {
            var client = new BitfinexRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new ApiCredentials("123", "456");
            });
            var tester = new RestRequestValidator<BitfinexRestClient>(client, "Endpoints/Spot/ExchangeData", "https://api.bitfinex.com", IsAuthenticated, stjCompare: false);
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
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetTickerAsync("tETHUST"), "GetTicker");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetTickersAsync(), "GetTickers");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetFundingTickerAsync("fUSD"), "GetFundingTicker");
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
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetFundingSizeAsync("UST", Enums.StatSection.Last), "GetFundingSize");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetCreditSizeAsync("UST", Enums.StatSection.Last), "GetCreditSize");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetCreditSizeAsync("UST", "tETHUST", Enums.StatSection.Last), "GetCreditSize2");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetLongsShortsTotalsAsync("tETHUST", Enums.StatSide.Long, Enums.StatSection.History), "GetLongsShortsTotals");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetTradingVolumeAsync(1, Enums.StatSection.History), "GetTradingVolume");
            await tester.ValidateAsync(client => client.SpotApi.ExchangeData.GetVolumeWeightedAveragePriceAsync("tETHUST", Enums.StatSection.Last), "GetVolumeWeightedAveragePrice");
        }

        private bool IsAuthenticated(WebCallResult result)
        {
            return result.RequestUrl.Contains("bfx-signature");
        }
    }
}
