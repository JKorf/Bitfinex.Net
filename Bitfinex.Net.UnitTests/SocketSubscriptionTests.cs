
using Bitfinex.Net.Clients;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            var logger = new LoggerFactory();
            logger.AddProvider(new TraceLoggerProvider());

            var client = new BitfinexSocketClient(Options.Create(new BitfinexSocketOptions
            {
                ApiCredentials = new BitfinexCredentials("123", "456"),
                OutputOriginalData = true,
            }), logger);
            var tester = new SocketSubscriptionValidator<BitfinexSocketClient>(client, "Subscriptions/Spot", "wss://api.bitfinex.com/", nestedPropertyForCompare: "1");
            await tester.ValidateAsync<BitfinexStreamTicker>((client, handler) => client.ExchangeApi.SubscribeToTickerUpdatesAsync("tETHUST", handler), "Ticker");
            await tester.ValidateAsync<BitfinexStreamFundingTicker>((client, handler) => client.ExchangeApi.SubscribeToFundingTickerUpdatesAsync("fUSD", handler), "TickerFunding");
            await tester.ValidateAsync<BitfinexOrderBookEntry[]>((client, handler) => client.ExchangeApi.SubscribeToOrderBookUpdatesAsync("tETHUST", Enums.Precision.PrecisionLevel2, Enums.Frequency.TwoSeconds, 25, handler), "OrderBook");
            await tester.ValidateAsync<BitfinexOrderBookFundingEntry[]>((client, handler) => client.ExchangeApi.SubscribeToFundingOrderBookUpdatesAsync("fUSD", Enums.Precision.PrecisionLevel2, Enums.Frequency.TwoSeconds, 25, handler), "OrderBookFunding");
            await tester.ValidateAsync<BitfinexRawOrderBookEntry[]>((client, handler) => client.ExchangeApi.SubscribeToRawOrderBookUpdatesAsync("tETHUST", 25, handler), "OrderBookRaw");
            await tester.ValidateAsync<BitfinexRawOrderBookFundingEntry[]>((client, handler) => client.ExchangeApi.SubscribeToRawFundingOrderBookUpdatesAsync("fUSD", 25, handler), "OrderBookRawFunding");
            await tester.ValidateAsync<BitfinexTradeSimple[]>((client, handler) => client.ExchangeApi.SubscribeToTradeUpdatesAsync("tETHUST", handler), "Trades");
            await tester.ValidateAsync<BitfinexKline[]>((client, handler) => client.ExchangeApi.SubscribeToKlineUpdatesAsync("tETHUST", Enums.KlineInterval.OneHour, handler), "Klines");
            await tester.ValidateAsync<BitfinexLiquidation[]>((client, handler) => client.ExchangeApi.SubscribeToLiquidationUpdatesAsync(handler), "Liquidations");
            await tester.ValidateAsync<BitfinexDerivativesStatusUpdate>((client, handler) => client.ExchangeApi.SubscribeToDerivativesUpdatesAsync("tBTCF0:USTF0", handler), "DerivStatus");

            await tester.ValidateAsync<BitfinexOrder[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(orderHandler: handler), "OrderSnapshot");
            await tester.ValidateAsync<BitfinexOrder[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(orderHandler: handler), "OrderUpdate");
            await tester.ValidateAsync<BitfinexPosition[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(positionHandler: handler), "PositionSnapshot");
            await tester.ValidateAsync<BitfinexPosition[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(positionHandler: handler), "PositionUpdate");
            await tester.ValidateAsync<BitfinexFundingOffer[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(fundingOfferHandler: handler), "FundingOfferSnapshot");
            await tester.ValidateAsync<BitfinexFundingOffer[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(fundingOfferHandler: handler), "FundingOfferUpdate");
            await tester.ValidateAsync<BitfinexFundingCredit[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(fundingCreditHandler: handler), "FundingCreditSnapshot");
            await tester.ValidateAsync<BitfinexFundingCredit[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(fundingCreditHandler: handler), "FundingCreditUpdate");
            await tester.ValidateAsync<BitfinexFunding[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(fundingLoanHandler: handler), "FundingLoanSnapshot");
            await tester.ValidateAsync<BitfinexFunding[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(fundingLoanHandler: handler), "FundingLoanUpdate");
            await tester.ValidateAsync<BitfinexWallet[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(walletHandler: handler), "WalletSnapshot");
            await tester.ValidateAsync<BitfinexWallet[]>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(walletHandler: handler), "WalletUpdate");
            await tester.ValidateAsync<BitfinexBalance>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(balanceHandler: handler), "BalanceUpdate");
            await tester.ValidateAsync<BitfinexTradeDetails>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(tradeHandler: handler), "UserTrade");
            await tester.ValidateAsync<BitfinexFundingTrade>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(fundingTradeHandler: handler), "FundingUserTrade");
            await tester.ValidateAsync<BitfinexFundingInfo>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(fundingInfoHandler: handler), "FundingInfo");
            await tester.ValidateAsync<BitfinexMarginBase>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(marginBaseHandler: handler), "MarginBase");
            await tester.ValidateAsync<BitfinexMarginSymbol>((client, handler) => client.ExchangeApi.SubscribeToUserUpdatesAsync(marginSymbolHandler: handler), "MarginSymbol");
        }

        [Test]
        public async Task TestDoubleSubscription()
        {
            var client = new BitfinexSocketClient();
            
            var sub1 = await client.ExchangeApi.SubscribeToTickerUpdatesAsync("tETHUST", (data) => { });
            var sub2 = await client.ExchangeApi.SubscribeToTickerUpdatesAsync("tETHUST", (data) => { });

            Assert.That(sub1.Success);
            Assert.That(sub2.Success);
        }
    }
}
