using Bitfinex.Net.Converters;
using Bitfinex.Net.Objects;
using Bitfinex.Net.Objects.SocketObjects;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;

namespace Bitfinex.Net.UnitTests
{
    [TestFixture]
    public class BitfinexSocketClientTests
    {
        [TestCase()]
        public void CallingStart_Should_ConnectWebsocket()
        {
            // arrange
            var client = TestHelpers.PrepareSocketClient(() => Construct());

            // act
            var waitTask = TestHelpers.WaitForConnect(client);
            var result = client.Start();

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(waitTask.Result);
        }

        [TestCase()]
        public void CallingStop_Should_DisconnectWebsocket()
        {
            // arrange
            var client = TestHelpers.PrepareSocketClient(() => Construct());

            // act
            var result = client.Start();
            var waitTask = TestHelpers.WaitForClose(client);
            client.Stop();

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(waitTask.Result);
        }

        [TestCase()]
        public void CallingPlaceOrderWithoutStarting_Should_FailAndGiveError()
        {
            // arrange
            var client = TestHelpers.PrepareSocketClient(() => Construct());

            // act
            var result = client.PlaceOrder(OrderType.Stop, "Test", 1);

            // assert
            Assert.IsFalse(result.Success);
        }

        [TestCase()]
        public void NotReceivingSubResponse_Should_ResultInError()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                SubscribeResponseTimeout = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));
            client.Start();

            // Act
            var result = client.SubscribeToBookUpdates("Test", Precision.PrecisionLevel0, Frequency.Realtime, 10, data => { });
            result.Wait();

            // assert
            Assert.IsFalse(result.Result.Success);

            client.Stop();
        }

        [TestCase(Precision.PrecisionLevel0, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel1, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel2, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel3, Frequency.Realtime)]
        [TestCase(Precision.PrecisionLevel0, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel1, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel2, Frequency.TwoSeconds)]
        [TestCase(Precision.PrecisionLevel3, Frequency.TwoSeconds)]
        public void SubscribingToBookUpdates_Should_SubscribeSuccessfully(Precision prec, Frequency freq)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));
            client.Start();

            // act
            var result = client.SubscribeToBookUpdates("Test", prec, freq, 10, data => { });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Frequency = JsonConvert.SerializeObject(freq, new FrequencyConverter(false)),
                Length = 10,
                Pair = "Test",
                Precision = JsonConvert.SerializeObject(prec, new PrecisionConverter(false)),
                Symbol = "Test"
            }));
            result.Wait();

            // assert
            Assert.IsTrue(result.Result.Success);

            client.Stop();
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
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));
            client.Start();
            BitfinexOrderBookEntry[] expected = new[] { new BitfinexOrderBookEntry() };
            BitfinexOrderBookEntry[] actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);
            var sub = client.SubscribeToBookUpdates("Test", prec, freq, 10, data =>
            {
                actual = data;
                evnt.Set();
            });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Frequency = JsonConvert.SerializeObject(freq, new FrequencyConverter(false)),
                Length = 10,
                Pair = "Test",
                Precision = JsonConvert.SerializeObject(prec, new PrecisionConverter(false)),
                Symbol = "Test"
            }));
            sub.Wait();

            // act
            TestHelpers.InvokeWebsocket(client, $"[1, {JsonConvert.SerializeObject(expected)}]");
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected[0], actual[0]));

            client.Stop();
        }

        [TestCase(TimeFrame.OneMinute)]
        [TestCase(TimeFrame.FiveMinute)]
        [TestCase(TimeFrame.FiveteenMinute)]
        [TestCase(TimeFrame.ThirtyMinute)]
        [TestCase(TimeFrame.OneHour)]
        [TestCase(TimeFrame.ThreeHour)]
        [TestCase(TimeFrame.SixHour)]
        [TestCase(TimeFrame.TwelfHour)]
        [TestCase(TimeFrame.OneDay)]
        [TestCase(TimeFrame.SevenDay)]
        [TestCase(TimeFrame.FourteenDay)]
        public void SubscribingToCandleUpdates_Should_SubscribeSuccessfully(TimeFrame timeframe)
        {
            // arrange
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));
            client.Start();

            // act
            var result = client.SubscribeToCandleUpdates("Test", timeframe, data => { });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Key = "trade:" + JsonConvert.SerializeObject(timeframe, new TimeFrameConverter(false)) + ":Test"
            }));
            result.Wait();

            // assert
            Assert.IsTrue(result.Result.Success);

            client.Stop();
        }

        [TestCase(TimeFrame.OneMinute)]
        [TestCase(TimeFrame.FiveMinute)]
        [TestCase(TimeFrame.FiveteenMinute)]
        [TestCase(TimeFrame.ThirtyMinute)]
        [TestCase(TimeFrame.OneHour)]
        [TestCase(TimeFrame.ThreeHour)]
        [TestCase(TimeFrame.SixHour)]
        [TestCase(TimeFrame.TwelfHour)]
        [TestCase(TimeFrame.OneDay)]
        [TestCase(TimeFrame.SevenDay)]
        [TestCase(TimeFrame.FourteenDay)]
        public void SubscribingToCandleUpdates_Should_TriggerWithCandleUpdate(TimeFrame timeframe)
        {
            // arrange
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));
            client.Start();
            BitfinexCandle[] expected = new[] { new BitfinexCandle() };
            BitfinexCandle[] actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);
            var sub = client.SubscribeToCandleUpdates("Test", timeframe, data =>
            {
                actual = data;
                evnt.Set();
            });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Key = "trade:" + JsonConvert.SerializeObject(timeframe, new TimeFrameConverter(false)) + ":Test"
            }));
            sub.Wait();

            // act
            TestHelpers.InvokeWebsocket(client, $"[1, {JsonConvert.SerializeObject(expected)}]");
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected[0], actual[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToTickerUpdates_Should_SubscribeSuccessfully()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));
            client.Start();

            // act
            var result = client.SubscribeToTickerUpdates("Test", data => { });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new TickerSubscriptionResponse()
            {
                Channel = "ticker",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            }));
            result.Wait();

            // assert
            Assert.IsTrue(result.Result.Success);

            client.Stop();
        }

        [Test]
        public void SubscribingToTickerUpdates_Should_TriggerWithTickerUpdate()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));
            client.Start();
            BitfinexMarketOverview[] expected = new[] { new BitfinexMarketOverview() };
            BitfinexMarketOverview[] actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);
            var sub = client.SubscribeToTickerUpdates("Test", data =>
            {
                actual = data;
                evnt.Set();
            });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new TickerSubscriptionResponse()
            {
                Channel = "ticker",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            }));
            sub.Wait();

            // act
            TestHelpers.InvokeWebsocket(client, $"[1, {JsonConvert.SerializeObject(expected)}]");
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected[0], actual[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToRawBookUpdates_Should_SubscribeSuccessfully()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));
            client.Start();

            // act
            var result = client.SubscribeToRawBookUpdates("Test", 10, data => { });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            }));
            result.Wait();

            // assert
            Assert.IsTrue(result.Result.Success);

            client.Stop();
        }

        [Test]
        public void SubscribingToRawBookUpdates_Should_TriggerWithRawBookUpdate()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));
            client.Start();
            BitfinexRawOrderBookEntry[] expected = new[] { new BitfinexRawOrderBookEntry() };
            BitfinexRawOrderBookEntry[] actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);
            var sub = client.SubscribeToRawBookUpdates("Test", 10, data =>
            {
                actual = data;
                evnt.Set();
            });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            }));
            sub.Wait();

            // act
            TestHelpers.InvokeWebsocket(client, $"[1, {JsonConvert.SerializeObject(expected)}]");
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected[0], actual[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToTradeUpdates_Should_SubscribeSuccessfully()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));
            client.Start();

            // act
            var result = client.SubscribeToTradeUpdates("Test", data => { });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new TradesSubscriptionResponse()
            {
                Channel = "trades",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            }));
            result.Wait();

            // assert
            Assert.IsTrue(result.Result.Success);

            client.Stop();
        }

        [Test]
        public void SubscribingToTradeUpdates_Should_TriggerWithTradeUpdate()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
            }));

            client.Start();
            BitfinexTradeSimple[] expected = new[] { new BitfinexTradeSimple() };
            BitfinexTradeSimple[] actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);
            var sub = client.SubscribeToTradeUpdates("Test", data =>
            {
                actual = data;
                evnt.Set();
            });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new TradesSubscriptionResponse()
            {
                Channel = "trades",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test"
            }));
            sub.Wait();

            // act
            TestHelpers.InvokeWebsocket(client, $"[1, {JsonConvert.SerializeObject(expected)}]");
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected[0], actual[0]));

            client.Stop();
        }

        [TestCase("ou", BitfinexEventType.OrderUpdate)]
        [TestCase("on", BitfinexEventType.OrderNew)]
        [TestCase("oc", BitfinexEventType.OrderCancel)]
        public void SubscribingToOrderUpdates_Should_TriggerWithOrderUpdate(string updateType, BitfinexEventType eventType)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexOrder>(eventType, new BitfinexOrder() { StatusString = "ACTIVE" });
            BitfinexSocketEvent<BitfinexOrder[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToOrderUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexOrder() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToOrderUpdates_Should_TriggerWithOrderSnapshot()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexOrder[]>(BitfinexEventType.OrderSnapshot, new[] { new BitfinexOrder() { StatusString = "ACTIVE" } });
            BitfinexSocketEvent<BitfinexOrder[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToOrderUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, "os", new[] { new BitfinexOrder() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected, actual, "Data"));
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [TestCase("te", BitfinexEventType.TradeExecuted)]
        [TestCase("tu", BitfinexEventType.TradeExecutionUpdate)]
        public void SubscribingToTradeUpdates_Should_TriggerWithTradeUpdate(string updateType, BitfinexEventType eventType)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexTradeDetails>(eventType, new BitfinexTradeDetails());
            BitfinexSocketEvent<BitfinexTradeDetails[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToTradeUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexTradeDetails() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToWalletUpdates_Should_TriggerWithWalletUpdate()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexWallet>(BitfinexEventType.WalletUpdate, new BitfinexWallet());
            BitfinexSocketEvent<BitfinexWallet[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToWalletUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, "wu", new BitfinexWallet() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToWalletUpdates_Should_TriggerWithWalletSnapshot()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexWallet[]>(BitfinexEventType.WalletSnapshot, new[] { new BitfinexWallet() });
            BitfinexSocketEvent<BitfinexWallet[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToWalletUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, "ws", new[] { new BitfinexWallet() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [TestCase("pn", BitfinexEventType.PositionNew)]
        [TestCase("pu", BitfinexEventType.PositionUpdate)]
        [TestCase("pc", BitfinexEventType.PositionClose)]
        public void SubscribingToPositionUpdates_Should_TriggerWithPositionUpdates(string updateType, BitfinexEventType evntType)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexPosition>(evntType, new BitfinexPosition() { });
            BitfinexSocketEvent<BitfinexPosition[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToPositionUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexPosition() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToPositionUpdates_Should_TriggerWithPositionSnapshot()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexPosition[]>(BitfinexEventType.PositionSnapshot, new[] { new BitfinexPosition() });
            BitfinexSocketEvent<BitfinexPosition[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToPositionUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, "ps", new[] { new BitfinexPosition() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [TestCase("fcn", BitfinexEventType.FundingCreditsNew)]
        [TestCase("fcu", BitfinexEventType.FundingCreditsUpdate)]
        [TestCase("fcc", BitfinexEventType.FundingCreditsClose)]
        public void SubscribingToFundingCreditsUpdates_Should_TriggerWithFundingCreditsUpdates(string updateType, BitfinexEventType evntType)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexFundingCredit>(evntType, new BitfinexFundingCredit() { StatusString = "ACTIVE" });
            BitfinexSocketEvent<BitfinexFundingCredit[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingCreditsUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexFundingCredit() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        public void SubscribingToFundingCreditsUpdates_Should_TriggerWithFundingCreditsSnapshot()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexFundingCredit[]>(BitfinexEventType.FundingCreditsSnapshot, new[] { new BitfinexFundingCredit() { StatusString = "ACTIVE" } });
            BitfinexSocketEvent<BitfinexFundingCredit[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingCreditsUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, "fcs", new[] { new BitfinexFundingCredit() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [TestCase("fln", BitfinexEventType.FundingLoanNew)]
        [TestCase("flu", BitfinexEventType.FundingLoanUpdate)]
        [TestCase("flc", BitfinexEventType.FundingLoanClose)]
        public void SubscribingToFundingLoanUpdates_Should_TriggerWithFundingLoanUpdates(string updateType, BitfinexEventType evntType)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexFundingLoan>(evntType, new BitfinexFundingLoan() { StatusString = "ACTIVE" });
            BitfinexSocketEvent<BitfinexFundingLoan[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingLoansUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexFundingLoan() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToFundingLoanUpdates_Should_TriggerWithFundingLoanSnapshot()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexFundingLoan[]>(BitfinexEventType.FundingLoanSnapshot, new[] { new BitfinexFundingLoan() { StatusString = "ACTIVE" } });
            BitfinexSocketEvent<BitfinexFundingLoan[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingLoansUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, "fls", new[] { new BitfinexFundingLoan() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [TestCase("fon", BitfinexEventType.FundingOfferNew)]
        [TestCase("fou", BitfinexEventType.FundingOfferUpdate)]
        [TestCase("foc", BitfinexEventType.FundingOfferCancel)]
        public void SubscribingToFundingOfferUpdates_Should_TriggerWithFundingOfferUpdates(string updateType, BitfinexEventType evntType)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());

            var expected = new BitfinexSocketEvent<BitfinexFundingOffer>(evntType, new BitfinexFundingOffer() { StatusString = "ACTIVE" });
            BitfinexSocketEvent<BitfinexFundingOffer[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            var result = client.SubscribeToFundingOfferUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, updateType, new BitfinexFundingOffer() }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data, actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void SubscribingToFundingOfferUpdates_Should_TriggerWithFundingOfferSnapshot()
        {
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() },
                ApiCredentials = new ApiCredentials("Test", "Test")
            }));
            
            if(!StartAndWaitForAuthentication(client))
                Assert.Fail(sb.ToString());
            
            var expected = new BitfinexSocketEvent<BitfinexFundingOffer[]>(BitfinexEventType.FundingOfferSnapshot, new[] { new BitfinexFundingOffer() { StatusString = "ACTIVE" } });
            BitfinexSocketEvent<BitfinexFundingOffer[]> actual = null;
            ManualResetEvent evnt = new ManualResetEvent(false);

            // act
            client.SubscribeToFundingOfferUpdates(data =>
            {
                actual = data;
                evnt.Set();
            });

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new object[] { 0, "fos", new[] { new BitfinexFundingOffer() } }));
            evnt.WaitOne(1000);

            // assert
            Assert.IsTrue(expected.EventType == actual.EventType);
            Assert.IsTrue(TestHelpers.PublicInstancePropertiesEqual(expected.Data[0], actual.Data[0]));

            client.Stop();
        }

        [Test]
        public void UnsubscribingFromUpdates_Should_UnsubscribeSuccessfully()
        {
            // arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() }

            }));
            client.Start();

            var result = client.SubscribeToRawBookUpdates("Test", 10, data => { });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            }));
            result.Wait();

            // act
            var task = client.UnsubscribeFromChannel(result.Result.Data);
            TestHelpers.InvokeWebsocket(client, "{ \"event\": \"unsubscribed\", \"chanId\": 1, \"channel\": \"book\" }");
            task.Wait();

            // assert
            Assert.IsTrue(task.Result.Success);

            client.Stop();
        }

        [Test]
        public void UnsubscribingFromUpdatesNoResponse_Should_NotSucceed()
        {
            // arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                SubscribeResponseTimeout = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() }

            }));
            client.Start();

            var result = client.SubscribeToRawBookUpdates("Test", 10, data => { });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            }));
            result.Wait();

            // act
            var task = client.UnsubscribeFromChannel(result.Result.Data).Result;

            // assert
            Assert.IsFalse(task.Success);

            client.Stop();
        }

        [Test]
        public void UnsubscribingFromUpdates_Should_NoLongerReceiveUpdates()
        {
            // arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() }

            }));
            client.Start();

            BitfinexRawOrderBookEntry[] expected = new[] { new BitfinexRawOrderBookEntry() };
            BitfinexRawOrderBookEntry[] actual = null;

            var result = client.SubscribeToRawBookUpdates("Test", 10, data => { actual = data; });
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new BookSubscriptionResponse()
            {
                Channel = "book",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Pair = "Test",
                Frequency = "F0",
                Precision = "R0",
                Length = 10
            }));
            result.Wait();

            var task = client.UnsubscribeFromChannel(result.Result.Data);
            TestHelpers.InvokeWebsocket(client, "{ \"event\": \"unsubscribed\", \"chanId\": 1, \"channel\": \"book\" }");
            task.Wait();

            // act
            TestHelpers.InvokeWebsocket(client, $"[1, {JsonConvert.SerializeObject(expected)}]");
            Thread.Sleep(100);

            // assert
            Assert.IsTrue(result.Result.Success);
            Assert.IsTrue(task.Result.Success);
            Assert.IsTrue(actual == null);

            client.Stop();
        }

        [Test]
        public void WhenConnectionIsLost_Socket_ShouldReconnect()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() }

            }));
            client.Start();

            // Act
            var waitConnect = TestHelpers.WaitForConnect(client);
            TestHelpers.CloseWebsocket(client);
            if (!waitConnect.Result)
                Assert.IsTrue(false, sb.ToString());

            // Assert
            Assert.IsTrue(true, sb.ToString());
        }

        [Test]
        public void WhenNoDataReceived_Socket_ShouldReconnect()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                SocketReceiveTimeout = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() }

            }));

            client.Start();

            // Act
            var waitConnect = TestHelpers.WaitForConnect(client);
            if (!waitConnect.Result)
                Assert.IsTrue(false, sb.ToString());

            // Assert
            Assert.IsTrue(true, sb.ToString());
        }

        [Test]
        public void WhenReconnecting_Socket_ShouldResubscribe()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() }

            }));
            
            client.Start();
            if (!WaitForSubscription(client))
                Assert.IsTrue(false, sb.ToString());

            // Act
            var waitConnect = TestHelpers.WaitForConnect(client);
            var waitResub = TestHelpers.WaitForSend(client, 3000, "{\"key\":\"trade:5m:Test\",\"event\":\"subscribe\",\"channel\":\"candles\"}");
            TestHelpers.CloseWebsocket(client);
            if (!waitConnect.Result)
                Assert.IsTrue(false, sb.ToString());
            if (!waitResub.Result)
                Assert.IsTrue(false, sb.ToString());

            // Assert
            Assert.IsTrue(true, sb.ToString());
        }

        [Test]
        public void WhenResubscribingFails_Socket_ShouldReconnectAndTryAgain()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ReconnectionInterval = TimeSpan.FromMilliseconds(100),
                LogWriters = new List<TextWriter>() { sb, new DebugTextWriter() }

            }));

            // Sub first time
            client.Start();
            if(!WaitForSubscription(client))
                Assert.IsTrue(false, sb.ToString());

            // Close socket first time
            var waitConnect = TestHelpers.WaitForConnect(client);
            var waitResub = TestHelpers.WaitForSend(client);
            TestHelpers.CloseWebsocket(client);
            if(!waitConnect.Result)
                Assert.IsTrue(false, sb.ToString());
            if(!waitResub.Result)
                Assert.IsTrue(false, sb.ToString());

            // Close socket second time
            waitConnect = TestHelpers.WaitForConnect(client);
            waitResub = TestHelpers.WaitForSend(client, 3000, "{\"key\":\"trade:5m:Test\",\"event\":\"subscribe\",\"channel\":\"candles\"}");
            TestHelpers.CloseWebsocket(client);
            if (!waitConnect.Result)
                Assert.IsTrue(false, sb.ToString());
            if (!waitResub.Result)
                Assert.IsTrue(false, sb.ToString());

            // Assert
            Assert.IsTrue(true);
        }

        [Test]
        public void PlacingAnOrder_Should_SucceedIfSuccessResponse()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { sb },
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.IsTrue(false, "Failed to auth");

            var response = JsonConvert.SerializeObject(new BitfinexOrder()
            {
                ClientOrderId = 1234
            });

            // Act
            var waitSend = TestHelpers.WaitForSend(client);
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeLimit, "Test", 1, price: 1, clientOrderId: 1234);
            if (!waitSend.Result)
                Assert.IsTrue(false, "No place order send");

            TestHelpers.InvokeWebsocket(client, $"[0, \"on\", {response}]");
            placeTask.Wait();

            // Assert
            Assert.IsTrue(placeTask.Result.Success, sb.ToString());
        }

        [Test]
        public void PlacingAnOrder_Should_FailIfErrorResponse()
        {
            // Arrange
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));

            if (!StartAndWaitForAuthentication(client))
                Assert.IsTrue(false, "Failed to auth");

            var response = JsonConvert.SerializeObject(new BitfinexOrder()
            {
                ClientOrderId = 1234
            });

            // Act
            var waitSend = TestHelpers.WaitForSend(client);
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeLimit, "Test", 1, price: 1, clientOrderId: 1234);
            if (!waitSend.Result)
                Assert.IsTrue(false, "No place order send");

            TestHelpers.InvokeWebsocket(client, $"[0, \"n\", [0, \"on-req\", 0, 0, {response}, 0, \"error\", \"order placing failed\"]]");

            placeTask.Wait();

            // Assert
            Assert.IsFalse(placeTask.Result.Success);
            Assert.IsTrue(placeTask.Result.Error.Message.Contains("order placing failed"));
        }

        [Test]
        public void PlacingAnOrder_Should_FailIfNoResponse()
        {
            // Arrange
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test"),
                OrderActionConfirmationTimeout = TimeSpan.FromMilliseconds(100)
            }));
            if (!StartAndWaitForAuthentication(client))
                Assert.IsTrue(false, "Failed to auth");

            // Act
            var placeTask = client.PlaceOrderAsync(OrderType.ExchangeLimit, "Test", 1, price: 1, clientOrderId: 1234);

            // Assert
            Assert.IsFalse(placeTask.Result.Success);
        }

        [Test]
        public void PlacingAMarketOrder_Should_SucceedIfSuccessResponse()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { sb },
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test"),
                OrderActionConfirmationTimeout = TimeSpan.FromMilliseconds(100)
            }));
            if (!StartAndWaitForAuthentication(client))
                Assert.IsTrue(false, "Failed to auth");

            var response = JsonConvert.SerializeObject(new BitfinexOrder()
            {
                ClientOrderId = 1234,
                Status = OrderStatus.Executed,
                StatusString = "EXECUTED"
            });

            // Act
            var waitSend = TestHelpers.WaitForSend(client);
            var placeTask = client.PlaceOrderAsync(OrderType.Market, "Test", 1, price: 1, clientOrderId: 1234);
            if (!waitSend.Result)
                Assert.IsTrue(false, "No place order send");

            TestHelpers.InvokeWebsocket(client, $"[0, \"oc\", {response}]");

            placeTask.Wait();
            
            // Assert
            Assert.IsTrue(placeTask.Result.Success, sb.ToString());
        }

        [TestCase(OrderType.Market)]
        [TestCase(OrderType.ExchangeMarket)]
        public void PlacingAMarketOrder_Should_FailIfStatusNotExecuted(OrderType orderType)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { sb },
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test"),
                OrderActionConfirmationTimeout = TimeSpan.FromMilliseconds(100)
            }));
            if (!StartAndWaitForAuthentication(client))
                Assert.IsTrue(false, "Failed to auth");

            var response = JsonConvert.SerializeObject(new BitfinexOrder()
            {
                ClientOrderId = 1234,
                Status = OrderStatus.Canceled,
                Type = orderType
            });

            // Act
            var waitSend = TestHelpers.WaitForSend(client);
            var placeTask = client.PlaceOrderAsync(orderType, "Test", 1, price: 1, clientOrderId: 1234);
            if (!waitSend.Result)
                Assert.IsTrue(false, "No place order send");

            TestHelpers.InvokeWebsocket(client, $"[0, \"oc\", {response}]");

            placeTask.Wait();

            // Assert
            Assert.IsFalse(placeTask.Result.Success, sb.ToString());
        }

        [TestCase(OrderType.FillOrKill)]
        [TestCase(OrderType.ExchangeFillOrKill)]
        public void PlacingAFOKOrder_Should_FailIfReceiveCanceled(OrderType type)
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { sb },
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("Test", "Test")
            }));
            
            var response = JsonConvert.SerializeObject(new BitfinexOrder()
            {
                ClientOrderId = 1234,
                Type = type,
                Status = OrderStatus.Canceled
            });

            if (!StartAndWaitForAuthentication(client))
                Assert.IsTrue(false, "Failed to auth");

            // Act
            var waitSend = TestHelpers.WaitForSend(client);
            var placeTask = client.PlaceOrderAsync(type, "Test", 1, price: 1, clientOrderId: 1234);
            if(!waitSend.Result)
                Assert.IsTrue(false, "No place order send");

            TestHelpers.InvokeWebsocket(client, $"[0, \"oc\", {response}]");
            
            placeTask.Wait();

            // Assert
            Assert.IsTrue(placeTask.Result.Success, sb.ToString());
        }

        [Test]
        public void ReceivingAReconnectMessage_Should_ReconnectWebsocket()
        {
            // Arrange
            var sb = new StringWriter(new StringBuilder());
            var client = TestHelpers.PrepareSocketClient(() => Construct(new BitfinexSocketClientOptions()
            {
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { sb },
                ReconnectionInterval = TimeSpan.FromMilliseconds(100)
            }));

            client.Start();
            if(!WaitForSubscription(client))
                Assert.IsTrue(false, "Initial subscription failed");

            var waitConnect = TestHelpers.WaitForConnect(client);

            // Act
            TestHelpers.InvokeWebsocket(client, "{\"event\":\"info\", \"code\": 20051}");
            bool triggered = waitConnect.Result;

            // Assert
            Assert.IsTrue(triggered, sb.ToString());
        }

        private bool WaitForSubscription(BitfinexSocketClient client)
        {
            var sendWait = TestHelpers.WaitForSend(client);
            var result = client.SubscribeToCandleUpdates("Test", TimeFrame.FiveMinute, data => { });

            if (!sendWait.Result)
                return false;

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new CandleSubscriptionResponse()
            {
                Channel = "candles",
                Event = "subscribed",
                ChannelId = 1,
                Symbol = "Test",
                Key = "trade:" + JsonConvert.SerializeObject(TimeFrame.FiveMinute, new TimeFrameConverter(false)) + ":Test"
            }));
            result.Wait();
            return result.Result.Success;
        }

        private bool StartAndWaitForAuthentication(BitfinexSocketClient client)
        {
            var sendWait = TestHelpers.WaitForSend(client);
            client.Start();

            if (!sendWait.Result)
                return false;

            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new BitfinexAuthenticationResponse()
            {
                ChannelId = 0,
                Event = "auth",
                Status = "OK"
            }));
            return true;
        }


        private BitfinexSocketClient Construct(BitfinexSocketClientOptions options = null)
        {
            if (options != null)
                return new BitfinexSocketClient(options);
            return new BitfinexSocketClient();
        }
    }
}
