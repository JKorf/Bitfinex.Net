using Bitfinex.Net;
using Bitfinex.Net.Clients;
using Bitfinex.Net.SymbolOrderBooks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bitfinex.Net.UnitTests
{
    [NonParallelizable]
    internal class BitfinexRestIntegrationTests : RestIntegrationTest<BitfinexRestClient>
    {
        public override bool Run { get; set; } = true;

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
                ApiCredentials = Authenticated ? new BitfinexCredentials(key, sec) : null
            }));
        }

        [Test]
        public async Task TestErrorResponseParsing()
        {
            if (!ShouldRun())
                return;

            var result = await CreateClient().ExchangeApi.ExchangeData.GetOrderBookAsync("TST-TST", default);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.ErrorCode, Is.EqualTo("10020"));
        }

        [Test]
        public async Task TestSpotAccount()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetBalancesAsync(default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetBaseMarginInfoAsync(default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetSymbolMarginInfoAsync("tETHUSD", default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetMovementsAsync(default, default, default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetAlertListAsync(default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetAvailableBalanceAsync("tETHUSD", Enums.OrderSide.Buy, 1000, Enums.WalletType.Exchange, default, default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetLedgerEntriesAsync(default, default, default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetUserInfoAsync(default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.Get30DaySummaryAndFeesAsync(default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetLoginHistoryAsync(default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetApiKeyPermissionsAsync(default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Account.GetAccountChangeLogAsync(default), true);
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestSpotExchangeData()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetPlatformStatusAsync(default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetAssetsListAsync(default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetSymbolNamesAsync(Enums.SymbolType.Exchange, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetAssetNamesAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetAssetSymbolsAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetAssetFullNamesAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetAssetUnitsAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetAssetUnderlyingsAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetAssetNetworksAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetAssetBlockExplorerUrlsAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetAssetWithdrawalFeesAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetAssetDepositWithdrawalMethodsAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetSymbolsAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetFuturesSymbolsAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetDepositWithdrawalStatusAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetMarginInfoAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetDerivativesFeesAsync(default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetTickerAsync("tETHUST", default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetFundingTickerAsync("fUSD", default), false);
            await RunAndCheckResult(client => client.ExchangeApi.ExchangeData.GetTickersAsync(default, default), false);
            //await RunAndCheckResult(client => client.SpotApi.ExchangeData.GetFundingTickersAsync(default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetTickerHistoryAsync(default, default, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetTradeHistoryAsync("tETHUST", default, default, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetOrderBookAsync("tETHUST", Enums.Precision.PrecisionLevel2, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetFundingOrderBookAsync("fUSD", Enums.Precision.PrecisionLevel2, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetRawOrderBookAsync("fUSD", default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetRawFundingOrderBookAsync("fUSD", default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetLastKlineAsync("tETHUST", Enums.KlineInterval.OneDay, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetKlinesAsync("tETHUST", Enums.KlineInterval.OneDay, default, default, default, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetAveragePriceAsync("tETHUST", 1, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetForeignExchangeRateAsync("BTC", "USD", default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetDerivativesStatusAsync(default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetDerivativesStatusHistoryAsync("tBTCF0:USTF0", default, default, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetLiquidationsAsync(default, default, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetFundingStatisticsAsync("fUSD", default, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetLastFundingSizeAsync("fUSD", default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetFundingSizeHistoryAsync("fUSD", default, default, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetLastCreditSizeAsync("fUSD", default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetCreditSizeHistoryAsync("fUSD", default, default, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetLastLongsShortsTotalsAsync("tETHUSD", Enums.StatSide.Long, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetLongsShortsTotalsHistoryAsync("tETHUSD", Enums.StatSide.Long, default, default, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetLastTradingVolumeAsync(1, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetTradingVolumeHistoryAsync(1, default, default, default, default, default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetLastVolumeWeightedAveragePriceAsync("tETHUSD", default), false);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.ExchangeData.GetVolumeWeightedAveragePriceHistoryAsync("tETHUSD", default, default, default, default, default), false);
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestSpotTrading()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Trading.GetOpenOrdersAsync(default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Trading.GetClosedOrdersAsync(default, default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Trading.GetUserTradesAsync(default, default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Trading.GetPositionHistoryAsync(default, default, default, default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Trading.GetPositionsAsync(default), true);
            await RunAndCheckResult(warnings, client => client.ExchangeApi.Trading.GetPositionSnapshotsAsync(default, default, default, default), true);
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestOrderBooks()
        {
            await TestOrderBook(new BitfinexSymbolOrderBook("tETHUSD"));
        }
    }
}
