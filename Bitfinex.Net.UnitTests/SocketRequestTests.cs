using Bitfinex.Net.Clients;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class SocketRequestTests
    {
        private BitfinexSocketClient CreateClient()
        {
            var fact = new LoggerFactory();
            fact.AddProvider(new TraceLoggerProvider());
            var client = new BitfinexSocketClient(Options.Create(new BitfinexSocketOptions
            {
                RequestTimeout = TimeSpan.FromSeconds(1),
                ApiCredentials = new BitfinexCredentials("123", "456")
            }), fact);
            return client;
        }

        [Test]
        public async Task ValidateSpotAccountCalls()
        {
            var tester = new SocketRequestValidator<BitfinexSocketClient>("Socket/Spot");
            await tester.ValidateAsync(CreateClient(), client => client.ExchangeApi.PlaceOrderAsync(Enums.OrderSide.Buy, Enums.OrderType.Limit, "tETHUST", 1), "PlaceOrder", nestedJsonProperty: "2.4");
            await tester.ValidateAsync(CreateClient(), client => client.ExchangeApi.UpdateOrderAsync(123), "UpdateOrder", nestedJsonProperty: "2.4");
            await tester.ValidateAsync(CreateClient(), client => client.ExchangeApi.CancelOrderAsync(123), "CancelOrder", nestedJsonProperty: "2.4");
            await tester.ValidateAsync(CreateClient(), client => client.ExchangeApi.SubmitFundingOfferAsync(Enums.FundingOfferType.Limit, "tETHUSD", 1, 1, 1), "SubmitFundingOffer", nestedJsonProperty: "2.4");
            //await tester.ValidateAsync(CreateClient(), client => client.SpotApi.CancelFundingOfferAsync(123), "CancelFundingOffer", nestedJsonProperty: "2.4");

        }
    }
}
