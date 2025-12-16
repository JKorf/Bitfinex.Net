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

        public override BitfinexSocketClient GetClient(ILoggerFactory loggerFactory, bool useUpdatedDeserialization)
        {
            var key = Environment.GetEnvironmentVariable("APIKEY");
            var sec = Environment.GetEnvironmentVariable("APISECRET");

            Authenticated = key != null && sec != null;
            return new BitfinexSocketClient(Options.Create(new BitfinexSocketOptions
            {
                OutputOriginalData = true,
                UseUpdatedDeserialization = useUpdatedDeserialization,
                ApiCredentials = Authenticated ? new CryptoExchange.Net.Authentication.ApiCredentials(key, sec) : null
            }), loggerFactory);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task TestSubscriptions(bool useUpdatedDeserialization)
        {
            await RunAndCheckUpdate<BitfinexStreamTicker>(useUpdatedDeserialization , (client, updateHandler) => client.SpotApi.SubscribeToUserUpdatesAsync(default, default, default, default, default, default, default, default, default , default , default , default , default), false, true);
            await RunAndCheckUpdate<BitfinexStreamTicker>(useUpdatedDeserialization , (client, updateHandler) => client.SpotApi.SubscribeToTickerUpdatesAsync("tETHUST", updateHandler, default), true, false);
        } 
    }
}
