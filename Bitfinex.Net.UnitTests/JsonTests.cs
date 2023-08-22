using Bitfinex.Net.Interfaces;
using Bitfinex.Net.Objects;
using Bitfinex.Net.UnitTests;
using Bitfinex.Net.UnitTests.TestImplementations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoExchange.Net.Interfaces;
using Bitfinex.Net.Interfaces.Clients;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class JsonTests
    {
        private JsonToObjectComparer<IBitfinexRestClient> _comparer = new JsonToObjectComparer<IBitfinexRestClient>((json) => TestHelpers.CreateResponseClient(json, x =>
        {
            x.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "123");
            x.SpotOptions.RateLimiters = new List<IRateLimiter>();
            x.SpotOptions.OutputOriginalData = true;
        }, System.Net.HttpStatusCode.OK));

        [Test]
        public async Task ValidateSpotAccountCalls()
        {   
            await _comparer.ProcessSubject("Spot/Account", c => c.SpotApi.Account,
                parametersToSetNull: new[] { "limit", "clientOrderId" },
                takeFirstItemForCompare: new List<string> { "WithdrawAsync", "GetAccountInfoAsync", "WalletTransferAsync" }
                );
        }

        [Test]
        public async Task ValidateSpotExchangeDataCalls()
        {
            await _comparer.ProcessSubject("Spot/ExchangeData", c => c.SpotApi.ExchangeData,
                parametersToSetNull: new[] { "limit", "clientOrderId" }
                );
        }

        [Test]
        public async Task ValidateSpotTradingCalls()
        {
            await _comparer.ProcessSubject("Spot/Trading", c => c.SpotApi.Trading,
                parametersToSetNull: new[] { "limit", "clientOrderId" },
                ignoreProperties: new Dictionary<string, List<string>>
                {
                    { "GetOrderAsync", new List<string> { "meta" } }
                }
                );
        }

        [Test]
        public async Task ValidateSpotFundingCalls()
        {
            await _comparer.ProcessSubject("Spot/Funding", c => c.GeneralApi.Funding,
                parametersToSetNull: new[] { "limit", "clientOrderId" }
                );
        }
    }
}
