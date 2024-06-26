
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
            var tester = new SocketSubscriptionValidator<BitfinexSocketClient>(client, "Subscriptions/Spot", "wss://api.bitfinex.com/", nestedPropertyForCompare: "1", stjCompare: false);
            await tester.ValidateAsync<BitfinexStreamTicker>((client, handler) => client.SpotApi.SubscribeToTickerUpdatesAsync("tETHUST", handler), "Ticker");
            await tester.ValidateAsync<BitfinexStreamFundingTicker>((client, handler) => client.SpotApi.SubscribeToFundingTickerUpdatesAsync("fUSD", handler), "TickerFunding");
            await tester.ValidateAsync<IEnumerable<BitfinexOrderBookEntry>>((client, handler) => client.SpotApi.SubscribeToOrderBookUpdatesAsync("tETHUST", Enums.Precision.PrecisionLevel2, Enums.Frequency.TwoSeconds, 25, handler), "OrderBook");
            await tester.ValidateAsync<IEnumerable<BitfinexOrderBookFundingEntry>>((client, handler) => client.SpotApi.SubscribeToFundingOrderBookUpdatesAsync("fUSD", Enums.Precision.PrecisionLevel2, Enums.Frequency.TwoSeconds, 25, handler), "OrderBookFunding");
            await tester.ValidateAsync<IEnumerable<BitfinexRawOrderBookEntry>>((client, handler) => client.SpotApi.SubscribeToRawOrderBookUpdatesAsync("tETHUST", 25, handler), "OrderBookRaw");
            await tester.ValidateAsync<IEnumerable<BitfinexRawOrderBookFundingEntry>>((client, handler) => client.SpotApi.SubscribeToRawFundingOrderBookUpdatesAsync("fUSD", 25, handler), "OrderBookRawFunding");
            await tester.ValidateAsync<IEnumerable<BitfinexTradeSimple>>((client, handler) => client.SpotApi.SubscribeToTradeUpdatesAsync("tETHUST", handler), "Trades");
            await tester.ValidateAsync<IEnumerable<BitfinexKline>>((client, handler) => client.SpotApi.SubscribeToKlineUpdatesAsync("tETHUST", Enums.KlineInterval.OneHour, handler), "Klines");
            await tester.ValidateAsync<IEnumerable<BitfinexLiquidation>>((client, handler) => client.SpotApi.SubscribeToLiquidationUpdatesAsync(handler), "Liquidations");
            await tester.ValidateAsync<BitfinexDerivativesStatusUpdate>((client, handler) => client.SpotApi.SubscribeToDerivativesUpdatesAsync("tBTCF0:USTF0", handler), "DerivStatus");

            await tester.ValidateAsync<IEnumerable<BitfinexOrder>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(orderHandler: handler), "OrderSnapshot");
            await tester.ValidateAsync<IEnumerable<BitfinexOrder>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(orderHandler: handler), "OrderUpdate");
            await tester.ValidateAsync<IEnumerable<BitfinexPosition>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(positionHandler: handler), "PositionSnapshot");
            await tester.ValidateAsync<IEnumerable<BitfinexPosition>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(positionHandler: handler), "PositionUpdate");
            await tester.ValidateAsync<IEnumerable<BitfinexFundingOffer>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingOfferHandler: handler), "FundingOfferSnapshot");
            await tester.ValidateAsync<IEnumerable<BitfinexFundingOffer>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingOfferHandler: handler), "FundingOfferUpdate");
            await tester.ValidateAsync<IEnumerable<BitfinexFundingCredit>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingCreditHandler: handler), "FundingCreditSnapshot");
            await tester.ValidateAsync<IEnumerable<BitfinexFundingCredit>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingCreditHandler: handler), "FundingCreditUpdate");
            await tester.ValidateAsync<IEnumerable<BitfinexFunding>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingLoanHandler: handler), "FundingLoanSnapshot");
            await tester.ValidateAsync<IEnumerable<BitfinexFunding>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingLoanHandler: handler), "FundingLoanUpdate");
            await tester.ValidateAsync<IEnumerable<BitfinexWallet>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(walletHandler: handler), "WalletSnapshot");
            await tester.ValidateAsync<IEnumerable<BitfinexWallet>>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(walletHandler: handler), "WalletUpdate");
            await tester.ValidateAsync<BitfinexBalance>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(balanceHandler: handler), "BalanceUpdate");
            await tester.ValidateAsync<BitfinexTradeDetails>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(tradeHandler: handler), "UserTrade");
            await tester.ValidateAsync<BitfinexFundingTrade>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingTradeHandler: handler), "FundingUserTrade");
            await tester.ValidateAsync<BitfinexFundingInfo>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(fundingInfoHandler: handler), "FundingInfo");
            await tester.ValidateAsync<BitfinexMarginBase>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(marginBaseHandler: handler), "MarginBase");
            await tester.ValidateAsync<BitfinexMarginSymbol>((client, handler) => client.SpotApi.SubscribeToUserUpdatesAsync(marginSymbolHandler: handler), "MarginSymbol");
        }
    }
}
