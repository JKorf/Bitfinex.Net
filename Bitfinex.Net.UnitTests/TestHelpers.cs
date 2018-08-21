using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bitfinex.Net.UnitTests
{
    public class TestHelpers
    {
        [ExcludeFromCodeCoverage]
        public static bool PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
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
                        if (PublicInstancePropertiesEqual(selfValue, toValue, ignore))
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

        public static MockObjects<T> PrepareClient<T>(Func<T> construct, string responseData) where T : ExchangeClient, new()
        {
            var expectedBytes = Encoding.UTF8.GetBytes(responseData);
            var responseStream = new MemoryStream();
            responseStream.Write(expectedBytes, 0, expectedBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            var response = new Mock<IResponse>();
            response.Setup(c => c.GetResponseStream()).Returns(responseStream);

            var requestStream = new Mock<Stream>();

            var request = new Mock<IRequest>();
            request.Setup(c => c.GetRequestStream()).Returns(Task.FromResult(requestStream.Object));
            request.Setup(c => c.Headers).Returns(new WebHeaderCollection());
            request.Setup(c => c.Uri).Returns(new Uri("http://www.test.com"));
            request.Setup(c => c.GetResponse()).Returns(Task.FromResult(response.Object));

            var factory = new Mock<IRequestFactory>();
            factory.Setup(c => c.Create(It.IsAny<string>()))
                .Returns(request.Object);

            var client = construct();
            client.RequestFactory = factory.Object;
            var log = (Log)typeof(T).GetField("log", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(client);
            log.Level = LogVerbosity.Debug;
            return new MockObjects<T>()
            {
                Client = client,
                Request = request,
                RequestStream = requestStream,
                Response = response,
                RequestFactory = factory
            };
        }

        public static T PrepareExceptionClient<T>(string responseData, string exceptionMessage, int statusCode) where T : ExchangeClient, new()
        {
            var expectedBytes = Encoding.UTF8.GetBytes(responseData);
            var responseStream = new MemoryStream();
            responseStream.Write(expectedBytes, 0, expectedBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            var we = new WebException();
            var r = Activator.CreateInstance<HttpWebResponse>();
            var re = new HttpResponseMessage();

            typeof(HttpResponseMessage).GetField("_statusCode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(re, (HttpStatusCode)statusCode);
            typeof(HttpWebResponse).GetField("_httpResponseMessage", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(r, re);
            typeof(WebException).GetField("_message", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(we, exceptionMessage);
            typeof(WebException).GetField("_response", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(we, r);

            var response = new Mock<IResponse>();
            response.Setup(c => c.GetResponseStream()).Throws(we);

            var request = new Mock<IRequest>();
            request.Setup(c => c.Headers).Returns(new WebHeaderCollection());
            request.Setup(c => c.GetResponse()).Returns(Task.FromResult(response.Object));

            var factory = new Mock<IRequestFactory>();
            factory.Setup(c => c.Create(It.IsAny<string>()))
                .Returns(request.Object);

            var client = new T
            {
                RequestFactory = factory.Object
            };
            var log = (Log)typeof(T).GetField("log", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(client);
            log.Level = LogVerbosity.Debug;
            return client;
        }

        public static T PrepareSocketClient<T>(Func<T> construct) where T : ExchangeClient, new()
        {
            var factory = new Mock<IWebsocketFactory>();
            factory.Setup(s => s.CreateWebsocket(It.IsAny<Log>(), It.IsAny<string>())).Returns(CreateSocket);

            var client = construct();
            var log = (Log)typeof(T).GetField("log", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(client);
            if (log.Level == LogVerbosity.Info)
                log.Level = LogVerbosity.None;
            typeof(T).GetProperty("SocketFactory").SetValue(client, factory.Object);
            return client;
        }

        private static event Action<Mock<IWebsocket>> OnOpen;
        private static event Action<Mock<IWebsocket>> OnClose;
        private static event Action<Mock<IWebsocket>, string> OnSend;

        private static IWebsocket CreateSocket()
        {
            bool open = false;
            bool closed = true;

            IWebsocket obj = Mock.Of<IWebsocket>();
            var socket = Mock.Get(obj);
            socket.Setup(s => s.Close()).Returns(Task.FromResult(true)).Callback(() =>
            {
                var closing = socket.Object.IsOpen;
                open = false; closed = true;
                if(closing)
                    socket.Raise(s => s.OnClose += null);
                OnClose?.Invoke(socket);
            });
            socket.Setup(s => s.IsOpen).Returns(() => open);
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((data) =>
            {
                OnSend?.Invoke(socket, data);
            }));
            socket.Setup(s => s.IsClosed).Returns(() => closed);
            socket.Setup(s => s.Connect()).Returns(Task.FromResult(true)).Callback(() =>
            {
                open = true; closed = false;
                socket.Raise(s => s.OnOpen += null);
                OnOpen?.Invoke(socket);
            });
            socket.Setup(s => s.SetEnabledSslProtocols(It.IsAny<System.Security.Authentication.SslProtocols>()));
            return socket.Object;
        }

        public static async Task<bool> WaitForConnect(BitfinexSocketClient client, int timeout = 1000)
        {
            var evnt = new ManualResetEvent(false);
            var handler = new Action<Mock<IWebsocket>>((s) =>
            {
                if (client.socket == null)
                    return;

                var mock = Mock.Get(client.socket);
                if (s.Equals(mock))
                    evnt.Set();
            });

            OnOpen += handler;
            return await Task.Run(() =>
            {
                var result = evnt.WaitOne(timeout);
                OnOpen -= handler;
                return result;
            });
        }

        public static async Task<bool> WaitForClose(BitfinexSocketClient client, int timeout = 1000)
        {
            var evnt = new ManualResetEvent(false);
            var handler = new Action<Mock<IWebsocket>>((s) =>
            {
                if (client.socket == null)
                    return;

                var mock = Mock.Get(client.socket);
                if (s.Equals(mock))
                    evnt.Set();
            });

            OnClose += handler;
            return await Task.Run(() =>
            {
                var result = evnt.WaitOne(timeout);
                OnClose -= handler;
                return result;
            });
        }

        public static void CloseWebsocket(BitfinexSocketClient client)
        {
            var mock = Mock.Get(client.socket);
            mock.Setup(s => s.IsOpen).Returns(() => false);
            mock.Setup(s => s.IsClosed).Returns(() => true);

            mock.Raise(r => r.OnClose += null);
        }

        public static async Task<bool> WaitForSend(BitfinexSocketClient client, int timeout = 3000, string expectedData = null)
        {
            var evnt = new ManualResetEvent(false);
            var handler = new Action<Mock<IWebsocket>, string>((s, data) =>
            {
                if (client.socket == null)
                    return;

                var mock = Mock.Get(client.socket);
                if (s.Equals(mock))
                {
                    if (expectedData == null || data == expectedData)
                        evnt.Set();
                }
            });

            OnSend += handler;
            return await Task.Run(() =>
            {
                var result = evnt.WaitOne(timeout);
                OnSend -= handler;
                return result;
            });
        }

        public static void InvokeWebsocket(BitfinexSocketClient client, string data)
        {
            var mock = Mock.Get(client.socket);
            mock.Raise(r => r.OnMessage += null, data);
        }

        public class MockObjects<T> where T : ExchangeClient
        {
            public T Client { get; set; }
            public Mock<IRequest> Request { get; set; }
            public Mock<Stream> RequestStream { get; set; }
            public Mock<IResponse> Response { get; set; }
            public Mock<IRequestFactory> RequestFactory { get; set; }

        }
    }
}
