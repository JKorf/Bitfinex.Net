using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Interfaces.Clients;
using Bitfinex.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bitfinex.Net.UnitTests.TestImplementations
{
    public class TestHelpers
    {
        [ExcludeFromCodeCoverage]
        public static bool AreEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                var type = self.GetType();
                var ignoreList = new List<string>(ignore);
                foreach (var pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (ignoreList.Contains(pi.Name))
                    {
                        continue;
                    }

                    var selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                    var toValue = type.GetProperty(pi.Name).GetValue(to, null);

                    if (pi.PropertyType.IsClass && !pi.PropertyType.Module.ScopeName.Equals("System.Private.CoreLib.dll"))
                    {
                        // Check of "CommonLanguageRuntimeLibrary" is needed because string is also a class
                        if (AreEqual(selfValue, toValue, ignore))
                        {
                            continue;
                        }

                        return false;
                    }

                    if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                    {
                        return false;
                    }
                }

                return true;
            }

            return self == to;
        }

        public static BitfinexSocketClient CreateAuthenticatedSocketClient(IWebsocket socket, Action<BitfinexSocketOptions> options = null)
        {
            return CreateSocketClient(socket, options ?? new Action<BitfinexSocketOptions>((x) => { x.ApiCredentials = new ApiCredentials("Test", "Test"); }));
        }

        public static BitfinexSocketClient CreateSocketClient(IWebsocket socket, Action<BitfinexSocketOptions> options = null)
        {
            BitfinexSocketClient client;
            client = options != null ? new BitfinexSocketClient(options) : new BitfinexSocketClient();
            client.SpotApi.SocketFactory = Mock.Of<IWebsocketFactory>();
            Mock.Get(client.SpotApi.SocketFactory).Setup(f => f.CreateWebsocket(It.IsAny<ILogger>(), It.IsAny<WebSocketParameters>())).Returns(socket);
            return client;
        }

        public static IBitfinexRestClient CreateClient(Action<BitfinexRestOptions> options = null)
        {
            IBitfinexRestClient client;
            client = options != null ? new BitfinexRestClient(options) : new BitfinexRestClient();
            client.SpotApi.RequestFactory = Mock.Of<IRequestFactory>();
            client.GeneralApi.RequestFactory = Mock.Of<IRequestFactory>();
            return client;
        }

        public static IBitfinexRestClient CreateResponseClient(string response, Action<BitfinexRestOptions> options = null, HttpStatusCode code = HttpStatusCode.OK)
        {
            var client = (BitfinexRestClient)CreateClient(options);
            SetResponse(client, response, code);
            return client;
        }

        public static Mock<IRequest> SetResponse(BitfinexRestClient client, string responseData, HttpStatusCode code = HttpStatusCode.OK)
        {
            var expectedBytes = Encoding.UTF8.GetBytes(responseData);
            var responseStream = new MemoryStream();
            responseStream.Write(expectedBytes, 0, expectedBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            var response = new Mock<IResponse>();
            response.Setup(c => c.IsSuccessStatusCode).Returns(code == HttpStatusCode.OK);
            response.Setup(c => c.StatusCode).Returns(code);
            response.Setup(c => c.GetResponseStreamAsync()).Returns(Task.FromResult((Stream)responseStream));

            var request = new Mock<IRequest>();
            request.Setup(c => c.Uri).Returns(new Uri("http://www.test.com"));
            request.Setup(c => c.GetResponseAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(response.Object));
            request.Setup(c => c.GetHeaders()).Returns(new KeyValuePair<string, string[]>[0]);

            var factory = Mock.Get(client.SpotApi.RequestFactory);
            factory.Setup(c => c.Create(It.IsAny<Version>(), It.IsAny<HttpMethod>(), It.IsAny<Uri>(), It.IsAny<int>()))
                .Returns(request.Object);
            factory = Mock.Get(client.GeneralApi.RequestFactory);
            factory.Setup(c => c.Create(It.IsAny<Version>(), It.IsAny<HttpMethod>(), It.IsAny<Uri>(), It.IsAny<int>()))
                .Returns(request.Object);
            return request;
        }
    }
}
