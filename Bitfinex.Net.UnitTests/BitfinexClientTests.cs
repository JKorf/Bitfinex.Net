using CryptoExchange.Net.Authentication;
using NUnit.Framework;
using Bitfinex.Net.UnitTests.TestImplementations;
using Moq;
using System.Net;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using Bitfinex.Net.Clients;
using CryptoExchange.Net.Clients;
using System.Net.Http;
using System.Collections.Generic;
using CryptoExchange.Net.Testing.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bitfinex.Net.Interfaces.Clients;

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
            Assert.That(HttpStatusCode.BadRequest == result.ResponseStatusCode);
        }


        [Test]
        public void ProvidingApiCredentials_Should_SaveApiCredentials()
        {
            // arrange
            // act
            var authProvider = new BitfinexAuthenticationProvider(new ApiCredentials("TestKey", "TestSecret"), null);

            // assert
            Assert.That(authProvider.ApiKey == "TestKey");
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

        [Test]
        [TestCase(TradeEnvironmentNames.Live, "https://api.bitfinex.com")]
        [TestCase("", "https://api.bitfinex.com")]
        public void TestConstructorEnvironments(string environmentName, string expected)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Bitfinex:Environment:Name", environmentName },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddBitfinex(configuration.GetSection("Bitfinex"));
            var provider = collection.BuildServiceProvider();

            var client = provider.GetRequiredService<IBitfinexRestClient>();

            var address = client.SpotApi.BaseAddress;

            Assert.That(address, Is.EqualTo(expected));
        }

        [Test]
        public void TestConstructorNullEnvironment()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Bitfinex", null },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddBitfinex(configuration.GetSection("Bitfinex"));
            var provider = collection.BuildServiceProvider();

            var client = provider.GetRequiredService<IBitfinexRestClient>();

            var address = client.SpotApi.BaseAddress;

            Assert.That(address, Is.EqualTo("https://api.bitfinex.com"));
        }

        [Test]
        public void TestConstructorApiOverwriteEnvironment()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Bitfinex:Environment:Name", "test" },
                    { "Bitfinex:Rest:Environment:Name", "live" },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddBitfinex(configuration.GetSection("Bitfinex"));
            var provider = collection.BuildServiceProvider();

            var client = provider.GetRequiredService<IBitfinexRestClient>();

            var address = client.SpotApi.BaseAddress;

            Assert.That(address, Is.EqualTo("https://api.bitfinex.com"));
        }

        [Test]
        public void TestConstructorConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ApiCredentials:Key", "123" },
                    { "ApiCredentials:Secret", "456" },
                    { "Socket:ApiCredentials:Key", "456" },
                    { "Socket:ApiCredentials:Secret", "789" },
                    { "Rest:OutputOriginalData", "true" },
                    { "Socket:OutputOriginalData", "false" },
                    { "Rest:Proxy:Host", "host" },
                    { "Rest:Proxy:Port", "80" },
                    { "Socket:Proxy:Host", "host2" },
                    { "Socket:Proxy:Port", "81" },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddBitfinex(configuration);
            var provider = collection.BuildServiceProvider();

            var restClient = provider.GetRequiredService<IBitfinexRestClient>();
            var socketClient = provider.GetRequiredService<IBitfinexSocketClient>();

            Assert.That(((BaseApiClient)restClient.SpotApi).OutputOriginalData, Is.True);
            Assert.That(((BaseApiClient)socketClient.SpotApi).OutputOriginalData, Is.False);
            Assert.That(((BaseApiClient)restClient.SpotApi).AuthenticationProvider.ApiKey, Is.EqualTo("123"));
            Assert.That(((BaseApiClient)socketClient.SpotApi).AuthenticationProvider.ApiKey, Is.EqualTo("456"));
            Assert.That(((BaseApiClient)restClient.SpotApi).ClientOptions.Proxy.Host, Is.EqualTo("host"));
            Assert.That(((BaseApiClient)restClient.SpotApi).ClientOptions.Proxy.Port, Is.EqualTo(80));
            Assert.That(((BaseApiClient)socketClient.SpotApi).ClientOptions.Proxy.Host, Is.EqualTo("host2"));
            Assert.That(((BaseApiClient)socketClient.SpotApi).ClientOptions.Proxy.Port, Is.EqualTo(81));
        }
    }
}
