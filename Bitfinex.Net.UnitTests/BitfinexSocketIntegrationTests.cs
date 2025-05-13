using Bitfinex.Net.Clients;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Bitfinex.Net.UnitTests
{
    [NonParallelizable]
    internal class BitfinexSocketIntegrationTests : SocketIntegrationTest<BitfinexSocketClient>
    {
        public override bool Run { get; set; } = false;

        public BitfinexSocketIntegrationTests()
        {
        }

        public override BitfinexSocketClient GetClient(ILoggerFactory loggerFactory)
        {
            var key = Environment.GetEnvironmentVariable("APIKEY");
            var sec = Environment.GetEnvironmentVariable("APISECRET");

            Authenticated = key != null && sec != null;
            return new BitfinexSocketClient(Options.Create(new BitfinexSocketOptions
            {
                OutputOriginalData = true,
                ApiCredentials = Authenticated ? new CryptoExchange.Net.Authentication.ApiCredentials(key, sec) : null
            }), loggerFactory);
        }


        [Test]
        public async Task TestSubscriptions()
        {
            await RunAndCheckUpdate<BitfinexStreamTicker>((client, updateHandler) => client.SpotApi.SubscribeToUserUpdatesAsync(default, default, default, default, default, default, default, default, default , default , default , default , default), false, true);
            await RunAndCheckUpdate<BitfinexStreamTicker>((client, updateHandler) => client.SpotApi.SubscribeToTickerUpdatesAsync("tETHUST", updateHandler, default), true, false);
        } 
    }
}
