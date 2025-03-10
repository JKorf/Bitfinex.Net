
using Bitfinex.Net.Clients;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using CryptoExchange.Net.Testing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class SocketSubscriptionTests
    {
        [Test]
        public async Task ValidateSubscriptions()
        {
            var client = new BitfinexSocketClient(opts =>
            {
                opts.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "456");
            });
            var tester = new SocketSubscriptionValidator<BitfinexSocketClient>(client, "Subscriptions/Spot", "wss://api.bitfinex.com/", nestedPropertyForCompare: "1");
            await tester.ValidateAsync<BitfinexStreamTicker>((client, handler) => client.SpotApi.SubscribeToTickerUpdatesAsync("tETHUST", handler), "Ticker");
            await tester.ValidateAsync<BitfinexStreamFundingTicker>((client, handler) => client.SpotApi.SubscribeToFundingTickerUpdatesAsync("fUSD", handler), "TickerFunding");
            await tester.ValidateAsync<BitfinexOrderBookEntry[]>((client, handler) => client.SpotApi.SubscribeToOrderBookUpdatesAsync("tETHUST", Enums.Precision.PrecisionLevel2, Enums.Frequency.TwoSeconds, 25, handler), "OrderBook");
            await tester.ValidateAsync<BitfinexOrderBookFundingEntry[]>((client, handler) => client.SpotApi.SubscribeToFundingOrderBookUpdatesAsync("fUSD", Enums.Precision.PrecisionLevel2, Enums.Frequency.TwoSeconds, 25, handler), "OrderBookFunding");
            await tester.ValidateAsync<BitfinexRawOrderBookEntry[]>((client, handler) => client.SpotApi.SubscribeToRawOrderBookUpdatesAsync("tETHUST", 25, handler), "OrderBookRaw");
            await tester.ValidateAsync<BitfinexRawOrderBookFundingEntry[]>((client, handler) => client.SpotApi.SubscribeToRawFundingOrderBookUpdatesAsync("fUSD", 25, handler), "OrderBookRawFunding");
            await tester.ValidateAsync<BitfinexTradeSimple[]>((client, handler) => client.SpotApi.SubscribeToTradeUpdatesAsync("tETHUST", handler), "Trades");
            await tester.ValidateAsync<BitfinexKline[]>((client, handler) => client.SpotApi.SubscribeToKlineUpdatesAsync("tETHUST", Enums.KlineInterval.OneHour, handler), "Klines");
            await tester.ValidateAsync<BitfinexLiquidation[]>((client, handler) => client.SpotApi.SubscribeToLiquidationUpdatesAsync(handler), "Liquidations");
            await tester.ValidateAsync<BitfinexDerivativesStatusUpdate>((client, handler) => client.SpotApi.SubscribeToDerivativesUpdatesAsync("tBTCF0:USTF0", handler), "DerivStatus");

            await tester.ValidateAsync<BitfinexOrder[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(orderHandler: handler), "OrderSnapshot");
            await tester.ValidateAsync<BitfinexOrder[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(orderHandler: handler), "OrderUpdate");
            await tester.ValidateAsync<BitfinexPosition[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(positionHandler: handler), "PositionSnapshot");
            await tester.ValidateAsync<BitfinexPosition[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(positionHandler: handler), "PositionUpdate");
            await tester.ValidateAsync<BitfinexFundingOffer[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingOfferHandler: handler), "FundingOfferSnapshot");
            await tester.ValidateAsync<BitfinexFundingOffer[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingOfferHandler: handler), "FundingOfferUpdate");
            await tester.ValidateAsync<BitfinexFundingCredit[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingCreditHandler: handler), "FundingCreditSnapshot");
            await tester.ValidateAsync<BitfinexFundingCredit[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingCreditHandler: handler), "FundingCreditUpdate");
            await tester.ValidateAsync<BitfinexFunding[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingLoanHandler: handler), "FundingLoanSnapshot");
            await tester.ValidateAsync<BitfinexFunding[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingLoanHandler: handler), "FundingLoanUpdate");
            await tester.ValidateAsync<BitfinexWallet[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(walletHandler: handler), "WalletSnapshot");
            await tester.ValidateAsync<BitfinexWallet[]>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(walletHandler: handler), "WalletUpdate");
            await tester.ValidateAsync<BitfinexBalance>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(balanceHandler: handler), "BalanceUpdate");
            await tester.ValidateAsync<BitfinexTradeDetails>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(tradeHandler: handler), "UserTrade");
            await tester.ValidateAsync<BitfinexFundingTrade>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingTradeHandler: handler), "FundingUserTrade");
            await tester.ValidateAsync<BitfinexFundingInfo>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingInfoHandler: handler), "FundingInfo");
            await tester.ValidateAsync<BitfinexMarginBase>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(marginBaseHandler: handler), "MarginBase");
            await tester.ValidateAsync<BitfinexMarginSymbol>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(marginSymbolHandler: handler), "MarginSymbol");
        }

        [Test]
        public async Task TestDoubleSubscription()
        {
            var client = new BitfinexSocketClient();
            
            var sub1 = await client.SpotApi.SubscribeToTickerUpdatesAsync("tETHUST", (data) => { });
            var sub2 = await client.SpotApi.SubscribeToTickerUpdatesAsync("tETHUST", (data) => { });

            Assert.That(sub1.Success);
            Assert.That(sub2.Success);
        }
    }
}
