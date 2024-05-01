using System;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net;
using NUnit.Framework;
using System.Linq;
using Bitfinex.Net.UnitTests.TestImplementations;
using Moq;
using System.Net;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Bitfinex.Net.Clients;
using Bitfinex.Net.ExtensionMethods;
using CryptoExchange.Net.Objects.Sockets;
using NUnit.Framework.Legacy;
using CryptoExchange.Net.Clients;
using System.Net.Http;
using System.Collections.Generic;
using CryptoExchange.Net.Converters.JsonNet;
using CryptoExchange.Net.Testing.Implementations;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture()]
    public class BitfinexClientTests
    {
        [TestCase]
        public async Task ReceivingHttpError_Should_ResultInErrorResult()
        {
            // arrange
            var client = TestHelpers.CreateResponseClient("Error message", null, HttpStatusCode.BadRequest);

            // act
            var result = await client.SpotApi.ExchangeData.GetAssetFullNamesAsync();

            // assert
            Assert.That(false == result.Success);
            Assert.That(result.Error.ToString().Contains("Error message"));
            Assert.That(HttpStatusCode.BadRequest == result.ResponseStatusCode);
        }


        [Test]
        public void ProvidingApiCredentials_Should_SaveApiCredentials()
        {
            // arrange
            // act
            var authProvider = new BitfinexAuthenticationProvider(new ApiCredentials("TestKey", "TestSecret"), null);

            // assert
            Assert.That(authProvider.GetApiKey() == "TestKey");
        }

        [Test]
        public async Task MakingAuthv2Call_Should_SendAuthHeaders()
        {
            // arrange
            var client = TestHelpers.CreateClient(x => { x.ApiCredentials = new ApiCredentials("TestKey", "t"); });
            var request = TestHelpers.SetResponse((BitfinexRestClient)client, "{}");

            // act
            await client.SpotApi.Trading.GetOpenOrdersAsync();

            // assert
            request.Verify(r => r.AddHeader("bfx-nonce", It.IsAny<string>()));
            request.Verify(r => r.AddHeader("bfx-signature", It.IsAny<string>()));
            request.Verify(r => r.AddHeader("bfx-apikey", "TestKey"));
        }

        [Test]
        public void CheckSignatureExample1()
        {
            var authProvider = new BitfinexAuthenticationProvider(
                new ApiCredentials("hO6oQotzTE0S5FRYze2Jx2wGx7eVnJGMolpA1nZyehsoMgCcgKNWQHd4QgTFZuwl4Zt4xMe2PqGBegWXO4A", "mheO6dR8ovSsxZQCOYEFCtelpuxcWGTfHw7te326y6jOwq5WpvFQ9JNljoTwBXZGv5It07m9RXSPpDQEK2w"), 
                new TestNonceProvider(1696751141337)
                );
            var client = (RestApiClient)new BitfinexRestClient().SpotApi;

            CryptoExchange.Net.Testing.TestHelpers.CheckSignature(
                client,
                authProvider,
                HttpMethod.Post,
                "v2/auth/w/order/submit",
                (uriParams, bodyParams, headers) =>
                {
                    return headers["bfx-signature"].ToString();
                },
                "5bead18437434889be3fda165655289d30ee7433d5cdaa9bffd1c3291ea625971b452ca87c7ed11af4e9c959352ec91a",
                new Dictionary<string, object>
                {
                    { "type", "LIMIT" },
                    { "symbol", "tBTCUSD" },
                    { "price", 15 },
                    { "amount", 0.1 },
                },
                disableOrdering: true);

        }

        [Test]
        public void CheckInterfaces()
        {
            CryptoExchange.Net.Testing.TestHelpers.CheckForMissingRestInterfaces<BitfinexRestClient>();
            CryptoExchange.Net.Testing.TestHelpers.CheckForMissingSocketInterfaces<BitfinexSocketClient>();
        }
    }
}
