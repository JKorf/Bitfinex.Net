using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects.Models;
using Bitfinex.Net.Objects.Models.Socket;
using Bitfinex.Net.Objects.Sockets;
using NUnit.Framework.Legacy;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;
using System.Text.Json;
using CryptoExchange.Net.Testing;
using Bitfinex.Net.Clients;
using Bitfinex.Net.UnitTests.TestImplementations;
using Microsoft.Extensions.Logging;
using CryptoExchange.Net.Objects;
using Bitfinex.Net.Objects.Options;
using Microsoft.Extensions.Options;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class BitfinexSocketClientTests
    {
        [TestCase(Precision.PrecisionLevel0, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel1, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel2, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel3, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel0, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel1, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel2, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel3, Frequency.TwoSeconds)]
        public async Task SubscribingToBookUpdates_Should_SubscribeSuccessfully(Precision prec, Frequency freq)
        {
            // arrange
            var client = new BitfinexSocketClient();
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            BitfinexOrderBookEntry[] result;
            var subTask = client.SpotApi.SubscribeToOrderBookUpdatesAsync("tBTCUSD", prec, freq, 25, data => result = data.Data.ToArray());

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Frequency = EnumConverter.GetString(freq),
                Length = 25,
                Pair = "BTCUSD",
                Precision = EnumConverter.GetString(prec),
                Symbol = "tBTCUSD"
            };

            // act
            socket.InvokeMessage(JsonSerializer.Serialize(subResponse));

            var taskResult = await subTask.ConfigureAwait(false);

            // assert
            Assert.That(taskResult.Success);
        }

        [TestCase(Precision.PrecisionLevel0, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel1, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel2, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel3, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel0, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel1, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel2, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel3, Frequency.TwoSeconds)]
        public void SubscribingToBookUpdates_Should_TriggerWithBookUpdate(Precision prec, Frequency freq)
        {
            // arrange
            var client = new BitfinexSocketClient();
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            BitfinexOrderBookEntry[] result = null;
            var subTask = client.SpotApi.SubscribeToOrderBookUpdatesAsync("tBTCUSD", prec, freq, 25, data => result = data.Data.ToArray());

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Frequency = EnumConverter.GetString(freq),
                Length = 25,
                Pair = "BTCUSD",
                Precision = EnumConverter.GetString(prec),
                Symbol = "tBTCUSD"
            };
            socket.InvokeMessage(JsonSerializer.Serialize(subResponse));
            subTask.Wait(5000);
            BitfinexOrderBookEntry[] expected = new[] { new BitfinexOrderBookEntry() { RawPrice = "1", RawQuantity = "2", Count = 3, Price = 1, Quantity = 2 } };

            // act
            socket.InvokeMessage($"[1, {JsonSerializer.Serialize(expected, SerializerOptions.WithConverters(BitfinexExchange._serializerContext))}]");

            // assert
            Assert.That(CryptoExchange.Net.Testing.TestHelpers.AreEqual(result[0], expected[0]));
        }

        [TestCase(KlineInterval.OneMinute)]
        [TestCase(KlineInterval.FiveMinutes)]
        [TestCase(KlineInterval.FifteenMinutes)]
        [TestCase(KlineInterval.ThirtyMinutes)]
        [TestCase(KlineInterval.OneHour)]
        [TestCase(KlineInterval.ThreeHours)]
        [TestCase(KlineInterval.SixHours)]
        [TestCase(KlineInterval.TwelveHours)]
        [TestCase(KlineInterval.OneDay)]
        [TestCase(KlineInterval.SevenDays)]
        [TestCase(KlineInterval.FourteenDays)]
        public void SubscribingToCandleUpdates_Should_SubscribeSuccessfully(KlineInterval timeframe)
        {
            // arrange
            var client = new BitfinexSocketClient();
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            var subTask = client.SpotApi.SubscribeToKlineUpdatesAsync("tBTCUSD", timeframe, data => { });

            var subResponse = new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Key = "trade:" + EnumConverter.GetString(timeframe) + ":tBTCUSD"
            };

            // act
            socket.InvokeMessage(JsonSerializer.Serialize(subResponse));

            subTask.Wait(5000);

            // assert
            Assert.That(subTask.Result.Success);
        }

        [TestCase(KlineInterval.OneMinute)]
        [TestCase(KlineInterval.FiveMinutes)]
        [TestCase(KlineInterval.FifteenMinutes)]
        [TestCase(KlineInterval.ThirtyMinutes)]
        [TestCase(KlineInterval.OneHour)]
        [TestCase(KlineInterval.ThreeHours)]
        [TestCase(KlineInterval.SixHours)]
        [TestCase(KlineInterval.TwelveHours)]
        [TestCase(KlineInterval.OneDay)]
        [TestCase(KlineInterval.SevenDays)]
        [TestCase(KlineInterval.FourteenDays)]
        public void SubscribingToCandleUpdates_Should_TriggerWithCandleUpdate(KlineInterval timeframe)
        {
            // arrange
            var client = new BitfinexSocketClient();
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            BitfinexKline[] result = null;
            var subTask = client.SpotApi.SubscribeToKlineUpdatesAsync("tBTCUSD", timeframe, data => result = data.Data.ToArray());

            var subResponse = new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Key = "trade:" + EnumConverter.GetString(timeframe) + ":tBTCUSD"
            };
            socket.InvokeMessage(JsonSerializer.Serialize(subResponse));
            subTask.Wait(5000);
            BitfinexKline[] expected = new[] { new BitfinexKline() };

            // act
            socket.InvokeMessage($"[1, {JsonSerializer.Serialize(expected, SerializerOptions.WithConverters(BitfinexExchange._serializerContext))}]");

            // assert
            Assert.That(CryptoExchange.Net.Testing.TestHelpers.AreEqual(result[0], expected[0]));
        }

        [Test]
        public void SubscribingToTickerUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var client = new BitfinexSocketClient();
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            var subTask = client.SpotApi.SubscribeToTickerUpdatesAsync("tBTCUSD", data => { });

            var subResponse = new TickerSubscriptionResponse()
            {
                Channel = "ticker",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "tBTCUSD",
                Pair = "BTCUSD"
            };

            // act
            socket.InvokeMessage(JsonSerializer.Serialize(subResponse));

            subTask.Wait(5000);

            // assert
            Assert.That(subTask.Result.Success);
        }

        [Test]
        public void SubscribingToTickerUpdates_Should_TriggerWithTickerUpdate()
        {
            // arrange
            var client = new BitfinexSocketClient();
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            BitfinexStreamTicker result = null;
            var subTask = client.SpotApi.SubscribeToTickerUpdatesAsync("tBTCUSD", data => result = data.Data);

            var subResponse = new TickerSubscriptionResponse()
            {
                Channel = "ticker",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "tBTCUSD",
                Pair = "BTCUSD"
            };
            socket.InvokeMessage(JsonSerializer.Serialize(subResponse));
            subTask.Wait(5000);
            BitfinexStreamTicker expected = new BitfinexStreamTicker();

            // act
            socket.InvokeMessage($"[1, {JsonSerializer.Serialize(expected)}]");

            // assert
            Assert.That(CryptoExchange.Net.Testing.TestHelpers.AreEqual(result, expected));
        }

        [Test]
        public void SubscribingToRawBookUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var client = new BitfinexSocketClient();
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            var subTask = client.SpotApi.SubscribeToRawOrderBookUpdatesAsync("tBTCUSD", 10, data => { });

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "tBTCUSD",
                Pair = "BTCUSD",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            };

            // act
            socket.InvokeMessage(JsonSerializer.Serialize(subResponse));

            subTask.Wait(5000);

            // assert
            Assert.That(subTask.Result.Success);
        }

        [Test]
        public void SubscribingToRawBookUpdates_Should_TriggerWithRawBookUpdate()
        {
            // arrange
            var client = new BitfinexSocketClient();
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            BitfinexRawOrderBookEntry[] result = null;
            var subTask = client.SpotApi.SubscribeToRawOrderBookUpdatesAsync("tBTCUSD", 10, data => result = data.Data.ToArray());

            var subResponse = new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "tBTCUSD",
                Pair = "BTCUSD",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            };
            socket.InvokeMessage(JsonSerializer.Serialize(subResponse));
            subTask.Wait(5000);
            BitfinexRawOrderBookEntry[] expected = new[] { new BitfinexRawOrderBookEntry() };

            // act
            socket.InvokeMessage($"[1, {JsonSerializer.Serialize(expected)}]");

            // assert
            Assert.That(CryptoExchange.Net.Testing.TestHelpers.AreEqual(result[0], expected[0]));
        }

        [Test]
        public void SubscribingToTradeUpdates_Should_SubscribeSuccessfully()
        {
            // arrange
            var client = new BitfinexSocketClient();
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            var subTask = client.SpotApi.SubscribeToTradeUpdatesAsync("BTCUSD", data => { });

            var subResponse = new TradesSubscriptionResponse()
            {
                Channel = "trades",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "BTCUSD",
                Pair = "BTCUSD"
            };

            // act
            socket.InvokeMessage(JsonSerializer.Serialize(subResponse));

            subTask.Wait(5000);

            // assert
            Assert.That(subTask.Result.Success);
        }

        [Test]
        public void SubscribingToTradeUpdates_Should_TriggerWithTradeUpdate()
        {
            // arrange
            var client = new BitfinexSocketClient();
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            BitfinexTradeSimple[] result = null;
            var subTask = client.SpotApi.SubscribeToTradeUpdatesAsync("BTCUSD", data => result = data.Data.ToArray());

            var subResponse = new TradesSubscriptionResponse()
            {
                Channel = "trades",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "BTCUSD",
                Pair = "BTCUSD"
            };
            socket.InvokeMessage(JsonSerializer.Serialize(subResponse));
            subTask.Wait(5000);
            BitfinexTradeSimple[] expected = new[] { new BitfinexTradeSimple() };

            // act
            socket.InvokeMessage($"[1, {JsonSerializer.Serialize(expected)}]");

            // assert
            Assert.That(CryptoExchange.Net.Testing.TestHelpers.AreEqual(result[0], expected[0]));
        }

        [Test]
        public void PlacingAnOrder_Should_SucceedIfSuccessResponse()
        {
            // arrange
            var client = new BitfinexSocketClient(x =>
            {
                x.ApiCredentials = new ApiCredentials("1", "2");
            });
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            var expected = new BitfinexOrder()
            {
                Price = 0.1m,
                QuantityRaw = 0.2m,
                Symbol = "tBTCUSD",
                Type = OrderType.ExchangeLimit,
                ClientOrderId = 1234,
                StatusString = "ACTIVE"
            };

            // act
            var placeTask = client.SpotApi.PlaceOrderAsync(OrderSide.Buy, OrderType.ExchangeLimit, "tBTCUSD", 1, price: 1, clientOrderId: 1234);
            socket.InvokeMessage(JsonSerializer.Serialize(new BitfinexResponse() { Event = "auth", Status = "OK" }));
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"n\", [0, \"on-req\", 0, 0, {JsonSerializer.Serialize(expected)}, 0, \"SUCCESS\", \"Submitted\"], 7, 1768997393433]");
            var result = placeTask.Result;

            // assert
            Assert.That(result.Success);
            Assert.That(CryptoExchange.Net.Testing.TestHelpers.AreEqual(expected, result.Data));
        }

        [Test]
        public void PlacingAnOrder_Should_FailIfErrorResponse()
        {
            // arrange
            var client = new BitfinexSocketClient(x =>
            {
                x.ApiCredentials = new ApiCredentials("1", "2");
            });
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");
            var order = new BitfinexOrder() { ClientOrderId = 123 };

            // act
            var placeTask = client.SpotApi.PlaceOrderAsync(OrderSide.Buy, OrderType.ExchangeLimit, "tBTCUSD", 1, price: 1, clientOrderId: 123);
            socket.InvokeMessage(JsonSerializer.Serialize(new BitfinexResponse() { Event = "auth", Status = "OK" }));
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"n\", [0, \"on-req\", 0, 0, {JsonSerializer.Serialize(order)}, 0, \"error\", \"order placing failed\"], 7, 1768997393433]");
            var result = placeTask.Result;

            // assert
            ClassicAssert.IsFalse(result.Success);
            Assert.That(result.Error.Message.Contains("order placing failed"));
        }

        [Test]
        public void PlacingAnOrder_Should_FailIfNoResponse()
        {
            // arrange
            var client = new BitfinexSocketClient(x =>
            {
                x.ApiCredentials = new ApiCredentials("1", "2");
                x.RequestTimeout = TimeSpan.FromMilliseconds(100);
            });
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            // act
            var placeTask = client.SpotApi.PlaceOrderAsync(OrderSide.Buy, OrderType.ExchangeLimit, "tBTCUSD", 1, price: 1, clientOrderId: 123);
            socket.InvokeMessage(JsonSerializer.Serialize(new BitfinexResponse() { Event = "auth", Status = "OK" }));
            var result = placeTask.Result;

            // assert
            ClassicAssert.IsFalse(result.Success);
        }

        [Test]
        public void PlacingAnMarketOrder_Should_SucceedIfSuccessResponse()
        {
            // arrange
            var client = new BitfinexSocketClient(x =>
            {
                x.ApiCredentials = new ApiCredentials("1", "2");
            });
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            var expected = new BitfinexOrder()
            {
                Price = 0.1m,
                QuantityRaw = 0.2m,
                Symbol = "tBTCUSD",
                Type = OrderType.ExchangeMarket,
                ClientOrderId = 1234,
                Status = OrderStatus.Executed,
                StatusString = "EXECUTED"
            };

            // act
            var placeTask = client.SpotApi.PlaceOrderAsync(OrderSide.Buy, OrderType.ExchangeMarket, "tBTCUSD", 1, price: 1, clientOrderId: 1234);
            socket.InvokeMessage(JsonSerializer.Serialize(new BitfinexResponse() { Event = "auth", Status = "OK" }));
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"n\", [0, \"on-req\", 0, 0, {JsonSerializer.Serialize(expected)}, 0, \"SUCCESS\", \"Submitted\"]]");
            var result = placeTask.Result;

            // assert
            Assert.That(result.Success);
            Assert.That(CryptoExchange.Net.Testing.TestHelpers.AreEqual(expected, result.Data));
        }

        [Test]
        public async Task PlacingAnFOKOrder_Should_SucceedIfSuccessResponse()
        {
            // arrange
            var client = new BitfinexSocketClient(x =>
            {
                x.ApiCredentials = new ApiCredentials("1", "2");
            });
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            var expected = new BitfinexOrder()
            {
                Price = 0.1m,
                QuantityRaw = 0.2m,
                Symbol = "tBTCUSD",
                Type = OrderType.ExchangeFillOrKill,
                ClientOrderId = 1234,
                StatusString = "CANCELED",
                Status = OrderStatus.Canceled
            };

            // act
            var placeTask = client.SpotApi.PlaceOrderAsync(OrderSide.Buy, OrderType.ExchangeFillOrKill, "tBTCUSD", 1, price: 1, clientOrderId: 1234);
            await Task.Delay(100);
            socket.InvokeMessage(JsonSerializer.Serialize(new BitfinexResponse() { Event = "auth", Status = "OK" }));
            await Task.Delay(100);
            var str = $"[0, \"n\", [0, \"on-req\", 0, 0, {JsonSerializer.Serialize(expected, SerializerOptions.WithConverters(BitfinexExchange._serializerContext))}, 0, \"SUCCESS\", \"Submitted\"]]";
            socket.InvokeMessage(str);
            var result = placeTask.Result;

            // assert
            Assert.That(result.Success);
            Assert.That(CryptoExchange.Net.Testing.TestHelpers.AreEqual(expected, result.Data));
        }

        [Test]
        public void PlacingAnFundingOffer_Should_SucceedIfSuccessResponse()
        {
            // arrange
            var client = new BitfinexSocketClient(x =>
            {
                x.ApiCredentials = new ApiCredentials("1", "2");
            });
            var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            var expected = new BitfinexFundingOffer()
            {
                Rate = 0.1m,
                Period = 1,
                Symbol = "fUSD",
                Quantity = 1,
                QuantityOriginal = 1,
                StatusString = "CANCELED",
                Status = OrderStatus.Canceled
            };

            // act
            var placeTask = client.SpotApi.SubmitFundingOfferAsync(FundingOfferType.Limit, "fUSD", 1, 1, 1);
            socket.InvokeMessage(JsonSerializer.Serialize(new BitfinexResponse() { Event = "auth", Status = "OK" }));
            Thread.Sleep(100);
            socket.InvokeMessage($"[0, \"n\", [0, \"fon-req\", 0, 0, {JsonSerializer.Serialize(expected)}, 0, \"SUCCESS\", \"Submitted\"]]");
            var result = placeTask.Result;

            // assert
            Assert.That(result.Success);
            Assert.That(CryptoExchange.Net.Testing.TestHelpers.AreEqual(expected, result.Data));
        }


        [Test]
        public async Task ReceivingAReconnectMessage_Should_ReconnectWebsocket()
        {
            // Re-enable on next CryptoExchange.Net version; bug in testing implementation
            return;

            //// arrange
            //var logger = new LoggerFactory();
            //logger.AddProvider(new TraceLoggerProvider());
            //var client = new BitfinexSocketClient(Options.Create(new BitfinexSocketOptions
            //{
            //    ApiCredentials = new ApiCredentials("1", "2"),
            //    OutputOriginalData = true
            //}), logger);
            //var socket = CryptoExchange.Net.Testing.TestHelpers.ConfigureSocketClient(client, "wss://localhost");

            //var rstEvent = new ManualResetEvent(false);
            //var subTask = client.SpotApi.SubscribeToKlineUpdatesAsync("tBTCUSD", KlineInterval.FiveMinutes, data => { });
            //socket.InvokeMessage(JsonSerializer.Serialize(new CandleSubscriptionResponse()
            //{
            //    Channel = "candles",
            //    Event = "subscribed",
            //    ChannelId = 1,
            //    Key = "trade:" + EnumConverter.GetString(KlineInterval.FiveMinutes) + ":tBTCUSD"
            //}));
            //var subResult = await subTask;

            //subResult.Data.ConnectionRestored += (t) => rstEvent.Set();

            //// act
            //socket.InvokeMessage("{\"event\":\"info\", \"code\": 20051}");
            //Thread.Sleep(100);
            //socket.InvokeMessage(JsonSerializer.Serialize(new CandleSubscriptionResponse()
            //{
            //    Channel = "candles",
            //    Event = "subscribed",
            //    ChannelId = 1,
            //    Key = "trade:" + EnumConverter.GetString(KlineInterval.FiveMinutes) + ":tBTCUSD"
            //}));

            //var triggered = rstEvent.WaitOne(1000);

            //// assert
            //Assert.That(triggered);
        }
    }
}
