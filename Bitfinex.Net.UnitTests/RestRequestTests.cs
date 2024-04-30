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
        }

        private bool IsAuthenticated(WebCallResult result)
        {
            return result.RequestUrl.Contains("bfx-signature");
        }
    }
}
