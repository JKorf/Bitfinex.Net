using Bitfinex.Net;
using Bitfinex.Net.Clients;
using Bitfinex.Net.SymbolOrderBooks;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bitfinex.Net.UnitTests
{
    [NonParallelizable]
    internal class BitfinexRestIntegrationTests : RestIntegrationTest<BitfinexRestClient>
    {
        public override bool Run { get; set; }

        public BitfinexRestIntegrationTests()
        {
        }

        public override BitfinexRestClient GetClient(ILoggerFactory loggerFactory)
        {
            var key = Environment.GetEnvironmentVariable("APIKEY");
            var sec = Environment.GetEnvironmentVariable("APISECRET");

            Authenticated = key != null && sec != null;
            return new BitfinexRestClient(null, loggerFactory, Options.Create(new Objects.Options.BitfinexRestOptions
            {
                OutputOriginalData = true,
                ApiCredentials = Authenticated ? new CryptoExchange.Net.Authentication.ApiCredentials(key, sec) : null
            }));
        }

        [Test]
        public async Task TestErrorResponseParsing()
        {
            if (!ShouldRun())
                return;

            var result = await CreateClient().SpotApi.ExchangeData.GetOrderBookAsync("TST-TST", default);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.ErrorCode, Is.EqualTo("10020"));
        }

        [Test]
        public async Task TestSpotAccount()
        {
            await RunAndCheckResult(client => client.SpotApi.Account.GetBalancesAsync(default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.GetBaseMarginInfoAsync(default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.GetSymbolMarginInfoAsync("tETHUSD", default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.GetMovementsAsync(default, default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.GetAlertListAsync(default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.GetAvailableBalanceAsync("tETHUSD", Enums.OrderSide.Buy, 1000, Enums.WalletType.Exchange, default, default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.GetLedgerEntriesAsync(default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.GetUserInfoAsync(default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.Get30DaySummaryAndFeesAsync(default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.GetLoginHistoryAsync(default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.GetApiKeyPermissionsAsync(default), true);
            await RunAndCheckResult(client => client.SpotApi.Account.GetAccountChangeLogAsync(default), true);
        }

        [Test]
        public async Task TestSpotExchangeData()
        {
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetPlatformStatusAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAssetsListAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetSymbolNamesAsync(Enums.SymbolType.Exchange, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAssetNamesAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAssetSymbolsAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAssetFullNamesAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAssetUnitsAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAssetUnderlyingsAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAssetNetworksAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAssetBlockExplorerUrlsAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAssetWithdrawalFeesAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAssetDepositWithdrawalMethodsAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetSymbolsAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetFuturesSymbolsAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetDepositWithdrawalStatusAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetMarginInfoAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetDerivativesFeesAsync(default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetTickerAsync("tETHUST", default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetFundingTickerAsync("fUSD", default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetTickersAsync(default, default), false);
            //await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetFundingTickersAsync(default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetTickerHistoryAsync(default, default, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetTradeHistoryAsync("tETHUST", default, default, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetOrderBookAsync("tETHUST", Enums.Precision.PrecisionLevel2, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetFundingOrderBookAsync("fUSD", Enums.Precision.PrecisionLevel2, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetRawOrderBookAsync("fUSD", default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetRawFundingOrderBookAsync("fUSD", default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetLastKlineAsync("tETHUST", Enums.KlineInterval.OneDay, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetKlinesAsync("tETHUST", Enums.KlineInterval.OneDay, default, default, default, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetAveragePriceAsync("tETHUST", 1, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetForeignExchangeRateAsync("BTC", "USD", default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetDerivativesStatusAsync(default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetDerivativesStatusHistoryAsync("tBTCF0:USTF0", default, default, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetLiquidationsAsync(default, default, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetFundingStatisticsAsync("fUSD", default, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetLastFundingSizeAsync("fUSD", default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetFundingSizeHistoryAsync("fUSD", default, default, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetLastCreditSizeAsync("fUSD", default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetCreditSizeHistoryAsync("fUSD", default, default, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetLastLongsShortsTotalsAsync("tETHUSD", Enums.StatSide.Long, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetLongsShortsTotalsHistoryAsync("tETHUSD", Enums.StatSide.Long, default, default, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetLastTradingVolumeAsync(1, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetTradingVolumeHistoryAsync(1, default, default, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetLastVolumeWeightedAveragePriceAsync("tETHUSD", default), false);
            await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetVolumeWeightedAveragePriceHistoryAsync("tETHUSD", default, default, default, default, default), false);

        }

        [Test]
        public async Task TestSpotTrading()
        {
            await RunAndCheckResult(client => client.SpotApi.Trading.GetOpenOrdersAsync(default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApi.Trading.GetClosedOrdersAsync(default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApi.Trading.GetUserTradesAsync(default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApi.Trading.GetPositionHistoryAsync(default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApi.Trading.GetPositionsAsync(default), true);
            await RunAndCheckResult(client => client.SpotApi.Trading.GetPositionSnapshotsAsync(default, default, default, default), true);
        }

        [Test]
        public async Task TestOrderBooks()
        {
            await TestOrderBook(new BitfinexSymbolOrderBook("tETHUSD"));
        }
    }
}
